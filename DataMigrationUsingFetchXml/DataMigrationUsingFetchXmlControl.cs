using System;
using System.IO;
using System.Drawing;
using Microsoft.Xrm.Sdk;
using System.Windows.Forms;
using System.ComponentModel;
using McTools.Xrm.Connection;
using XrmToolBox.Extensibility;
using System.Collections.Generic;
using System.Collections.Specialized;
using DataMigrationUsingFetchXml.Model;
using Microsoft.WindowsAPICodePack.Dialogs;
using DataMigrationUsingFetchXml.Forms.Popup;
using DataMigrationUsingFetchXml.Services.Interfaces;
using DataMigrationUsingFetchXml.Services.Implementations;

namespace DataMigrationUsingFetchXml
{
    internal partial class DataMigrationUsingFetchXmlControl : MultipleConnectionsPluginControlBase
    {
        private string _logsPath;

        private Settings _mySettings;

        private readonly FetchXmlPopup _fetchXmlpopup;
        private readonly MatchedAction _matchedAction;
        private readonly MatchingCriteria _matchingCriteria;

        private readonly ILogger _logger;

        private readonly List<string> _displayNames;

        private readonly ITransferOperation _transferOperation;

        private readonly string _defaultPath = Environment.CurrentDirectory;

        public DataMigrationUsingFetchXmlControl(ILogger logger, ITransferOperation transferOperation)
        {
            InitializeComponent();
            _logger = logger;
            _transferOperation = transferOperation;
            _fetchXmlpopup = new FetchXmlPopup();
            _matchingCriteria = new MatchingCriteria();
            _matchedAction = new MatchedAction();
            _displayNames = new List<string>();
        }

        private void DataMigrationUsingFetchXmlControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out _mySettings))
            {
                _mySettings = new Settings();
                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
            TxtLogsPath.Text = _defaultPath;
            _logsPath = _defaultPath;
            richTextBoxLogs.HideSelection = false;

            //CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataMigrationUsingFetchXmlControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), _mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            if (_mySettings != null && detail != null)
            {
                _mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }

            if (actionName == "AdditionalOrganization")
            {
                SetConnectionLabel(detail, "Target");
                AdditionalConnectionDetails.Clear();
                AdditionalConnectionDetails.Add(detail);
            }
            else
            {
                SetConnectionLabel(detail, "Source");
            }
        }

        private void SetConnectionLabel(ConnectionDetail detail, string serviceType)
        {
            switch (serviceType)
            {
                case "Source":
                    LblSourceText.Text = detail.ConnectionName;
                    LblSourceText.ForeColor = Color.Green;
                    LblSource.ForeColor = Color.Green;
                    LblSource.Visible = true;
                    LblSourceText.Visible = true;
                    break;

                case "Target":
                    LblTargetText.Text = detail.ConnectionName;
                    LblTargetText.ForeColor = Color.Green;
                    LblTarget.ForeColor = Color.Green;
                    LblTarget.Visible = true;
                    LblTargetText.Visible = true;
                    break;

                default:
                    throw new ArgumentException("Invalid input parameter.", nameof(serviceType));
            }
        }

        protected override void ConnectionDetailsUpdated(NotifyCollectionChangedEventArgs e)
        {
            // update the list box with the connections
        }

        private void BtnBrowseLogs_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    TxtLogsPath.Text = dialog.FileName;
                    _logsPath = dialog.FileName;
                }
            }
        }

        private void TxtLogsPathLeave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtLogsPath.Text))
            {
                if (Directory.Exists(TxtLogsPath.Text))
                {
                    _logsPath = TxtLogsPath.Text;
                }
                else
                {
                    MessageBox.Show($"Can't find {TxtLogsPath.Text}. Check the spelling and try again.", "Incorrect path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logsPath = _defaultPath;
                    TxtLogsPath.Text = _logsPath;
                }
            }
            else
            {
                _logsPath = _defaultPath;
                TxtLogsPath.Text = _logsPath;
            }
        }

        private void BtnSelectTargetInstance_Click(object sender, EventArgs e)
        {
            AddAdditionalOrganization();
        }

        private void BtnTransferData_Click(object sender, EventArgs e)
        {
            if (AdditionalConnectionDetails.Count > 0)
            {
                if (BtnTransferData.Text == "Cancel")
                {
                    CancelWorker();
                    BtnTransferData.Enabled = false;
                    BtnTransferData.Text = "Transfer Data";
                    return;
                }
                InitializeLog();
                (List<string> fetchXmls, List<int> tableIndexesForTransfer) = GetFetchDataGridViewData();

                if (fetchXmls.Count > 0)
                {
                    ResultItem lastResultItem = null;
                    BtnTransferData.Text = "Cancel";

                    SetLoadingDetails(true);
                    InitializeTransferOperation();
                    ChangeToolsState(false);

                    richTextBoxLogs.Text = string.Empty;
                    BtnTransferData.Enabled = true;
                    _logger.LogInfo("Transfer is started. ");
                    _logger.LogInfo($"Log folder path: {TxtLogsPath.Text}");

                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "_",
                        MessageHeight = 0,
                        MessageWidth = 0,
                        IsCancelable = true,
                        Work = (worker, args) =>
                        {
                            try
                            {
                                _transferOperation.Transfer(fetchXmls, tableIndexesForTransfer, worker, args);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message);
                                _logger.LogError($"[trace log] {ex.StackTrace}");
                            }
                        },
                        ProgressChanged = args =>
                        {
                            ResultItem resultItem = (ResultItem)args.UserState;
                            ChangeLabelText(resultItem);
                            lastResultItem = resultItem;
                        },
                        PostWorkCallBack = args =>
                        {
                            ChangeToolsState(true);
                            BtnTransferData.Text = "Transfer Data";
                            PrintResultItem();
                            fetchXmls.Clear();
                            SetLoadingDetails(false);
                            SetLastLabelTextByResultItem(lastResultItem);

                            if (!CheckArgsResult(args))
                            {
                                return;
                            }
                            MessageBox.Show("Data Migration Completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Select at least one row from FetchXML table for data transfer. ", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select Target Instance. ", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private (List<string> fetchXmls, List<int> tableIndexesForTransfer) GetFetchDataGridViewData()
        {
            List<string> fetchXmls = new List<string>();
            List<int> tableIndexesForTransfer = new List<int>();

            foreach (DataGridViewRow row in FetchDataGridView.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == "True")
                {
                    fetchXmls.Add(_fetchXmlpopup.FetchXmls[row.Index]);
                    tableIndexesForTransfer.Add(row.Index);
                }
            }
            return (fetchXmls, tableIndexesForTransfer);
        }

        private void SetLastLabelTextByResultItem(ResultItem lastResultItem)
        {
            if (lastResultItem != null)
            {
                LblInfo.Text = $"{lastResultItem.SuccessfullyGeneratedRecordCount} of {lastResultItem.SourceRecordCountWithSign} {lastResultItem.DisplayName} is imported";
                if (lastResultItem.ErroredRecordCount > 0)
                {
                    LblError.Text = $"{lastResultItem.ErroredRecordCount} of {lastResultItem.SourceRecordCountWithSign} {lastResultItem.DisplayName} is errored";
                }
            }
            else
            {
                LblInfo.Text = string.Empty;
            }
        }

        private bool CheckArgsResult(RunWorkerCompletedEventArgs args)
        {
            if (args.Cancelled)
            {
                MessageBox.Show("Migration is Stopped.", "Migration is Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else if (args.Error != null)
            {
                MessageBox.Show(args.Error.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
                return true;
        }

        private void PrintResultItem()
        {
            _logger.LogInfo("Result: ");
            if (_transferOperation.ResultItems != null)
            {
                foreach (ResultItem resultItem in _transferOperation.ResultItems)
                {
                    _logger.LogInfo($"{resultItem.SchemaName}, {resultItem.SourceRecordCountWithSign} (Source Records)," +
                        $" {resultItem.SuccessfullyGeneratedRecordCount} (Migrated Records), {resultItem.ErroredRecordCount} (Errօred Records)");
                }
            }
        }

        private void ChangeLabelText(ResultItem resultItem)
        {
            LblTitle.Text = $"Migrating {resultItem.DisplayName} records";
            LblInfo.Text = $"{resultItem.SuccessfullyGeneratedRecordCount} of {resultItem.SourceRecordCountWithSign} is imported";

            if (resultItem.ErroredRecordCount > 0)
            {
                LblError.Text = $"{resultItem.ErroredRecordCount} of {resultItem.SourceRecordCountWithSign} is errored";
            }
        }

        private void InitializeTransferOperation()
        {
            ConnectionDetails connectionDetails = new ConnectionDetails(Service, AdditionalConnectionDetails);
            _transferOperation.DisplayNames = _displayNames;
            _transferOperation.SetConnectionDetails(connectionDetails);
        }

        private void SetLoadingDetails(bool visible)
        {
            if (visible == true)
            {
                LblInfo.Text = "Loading...";
                LblError.Visible = visible;
                LblInfo.Visible = visible;
                LblError.Text = string.Empty;
            }
            LblLoading.Visible = visible;
            LblTitle.Visible = visible;
        }

        private void InitializeLog()
        {
            _logger.SetTxtLogs(richTextBoxLogs);
            _logger.SetLogsPath(_logsPath);
        }

        private void ChangeToolsState(bool state)
        {
            pictureBoxRecBin.Enabled = state;
            TxtLogsPath.Enabled = state;
            BtnBrowseLogs.Enabled = state;
            BtnSelectTargetInstance.Enabled = state;
            FetchDataGridView.Enabled = state;
            pictureBoxAdd.Enabled = state;
            BtnTransferData.Enabled = state;
        }

        private void PictureBoxRecBin_Click(object sender, EventArgs e)
        {
            richTextBoxLogs.Text = null;
            LblInfo.Text = string.Empty;
            LblError.Text = string.Empty;
        }

        private void PictureBoxAdd_Click(object sender, EventArgs e)
        {
            InitializeLog();
            _fetchXmlpopup.SetTextBoxText(string.Empty);
            FetchXmlPopupDialog();

            CustomiseFetchDataGridViewSizeBasedOnRowHeight(22);
        }

        private void FetchXmlPopupDialog(int rowIndex = -1)
        {
            if (_fetchXmlpopup.ShowDialog() == DialogResult.OK)
            {
                IDataverseService dataverseService = new DataverseService(Service);
                ChangeToolsState(false);

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading...",
                    Work = (worker, args) =>
                    {
                        try
                        {
                            string fetch = _fetchXmlpopup.GetTextBoxText();

                            if (rowIndex != -1 && fetch == _fetchXmlpopup.FetchXmls[rowIndex])
                            {
                                return;
                            }
                            (string logicalName, string displayName) = dataverseService.GetEntityName(fetch);
                            _matchingCriteria.CreateLayoutPanels(fetch, rowIndex);

                            FetchDataGridView.Invoke(new MethodInvoker(delegate
                            {
                                if (rowIndex != -1)
                                {
                                    _fetchXmlpopup.FetchXmls[rowIndex] = fetch;
                                    fetchXmlDataBindingSource[rowIndex] = new FetchXmlData()
                                    {
                                        DisplayName = displayName,
                                        SchemaName = logicalName
                                    };
                                    _displayNames[rowIndex] = displayName;
                                }
                                else
                                {
                                    _displayNames.Add(displayName);
                                    fetchXmlDataBindingSource.Add(new FetchXmlData()
                                    {
                                        DisplayName = displayName,
                                        SchemaName = logicalName
                                    });
                                    FetchDataGridView.Rows[FetchDataGridView.Rows.Count - 1].Cells[0].Value = true;
                                    FetchDataGridView.Rows[FetchDataGridView.Rows.Count - 1].Cells[4].Value = 4;
                                    MatchedAction.CheckedRadioButtonNumbers.Add(4);
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            if (rowIndex == -1)
                            {
                                _fetchXmlpopup.FetchXmls.RemoveAt(_fetchXmlpopup.FetchXmls.Count - 1);
                            }
                            MessageBox.Show($"{ex.Message}.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    },
                    PostWorkCallBack = args =>
                    {
                        ChangeToolsState(true);
                    }
                });
            }
        }

        private void FetchDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (FetchDataGridView.Columns[e.ColumnIndex].Name == "Remove")
                {
                    if (MessageBox.Show("Are you sure you want to delete this item?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        fetchXmlDataBindingSource.RemoveAt(e.RowIndex);
                        _fetchXmlpopup.FetchXmls.RemoveAt(e.RowIndex);
                        _displayNames.RemoveAt(e.RowIndex);
                        MatchedAction.CheckedRadioButtonNumbers.RemoveAt(e.RowIndex);
                        _matchingCriteria.RemoveLayoutPanelData(e.RowIndex);

                        CustomiseFetchDataGridViewSizeBasedOnRowHeight(0);
                        //if (fetchXmlDataBindingSource.Count < 8 && FetchDataGridView.Width > 518)
                        //{
                        //    FetchDataGridView.Width -= 18;
                        //}
                    }
                }
                if (FetchDataGridView.Columns[e.ColumnIndex].Name == "Edit")
                {
                    _fetchXmlpopup.IsEdit = true;
                    _fetchXmlpopup.EditIndex = e.RowIndex;
                    _fetchXmlpopup.SetTextBoxText(_fetchXmlpopup.FetchXmls[e.RowIndex]);
                    FetchXmlPopupDialog(e.RowIndex);
                    _fetchXmlpopup.IsEdit = false;
                }
                if (FetchDataGridView.Columns[e.ColumnIndex].Name == "MatchingCriteria")
                {
                    Invoke((MethodInvoker)delegate
                    {
                        _matchingCriteria.SetLayoutPanelData(e.RowIndex);
                        _matchingCriteria.ShowDialog();
                    });
                }
                if (FetchDataGridView.Columns[e.ColumnIndex].Name == "ActionIfMatched")
                {
                    _matchedAction.CheckRadioButton(e.RowIndex);
                    _matchedAction.RowIndex = e.RowIndex;
                    if (_matchedAction.ShowDialog() == DialogResult.OK)
                    {
                        FetchDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = MatchedAction.CheckedRadioButtonNumbers[e.RowIndex];
                    }
                }
            }
        }

        private void RichTextBoxLogs_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void FetchDataGridView_RowHeightChanged(object sender, DataGridViewRowEventArgs e)
        {
            CustomiseFetchDataGridViewSizeBasedOnRowHeight(0);
        }

        private void CustomiseFetchDataGridViewSizeBasedOnRowHeight(int height)
        {
            foreach (DataGridViewRow row in FetchDataGridView.Rows)
            {
                height += row.Height;
            }

            if (height > FetchDataGridView.Height - 23 && FetchDataGridView.Width < 520)
            {
                FetchDataGridView.Width += 18;
            }
            else if (height < FetchDataGridView.Height - 23 && FetchDataGridView.Width > 518)
            {
                FetchDataGridView.Width -= 18;
            }
        }
    }
}