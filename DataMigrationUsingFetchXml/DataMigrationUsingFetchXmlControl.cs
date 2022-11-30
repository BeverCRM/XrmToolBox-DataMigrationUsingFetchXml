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

        private readonly Popup _popup;

        private readonly ILogger _logger;

        private readonly List<string> _displayNames;

        private readonly ITransferOperation _transferOperation;

        private readonly string _defaultPath = Environment.CurrentDirectory;

        public DataMigrationUsingFetchXmlControl(ILogger logger, ITransferOperation transferOperation)
        {
            InitializeComponent();
            _logger = logger;
            _transferOperation = transferOperation;
            _popup = new Popup();
            _displayNames = new List<string>();
            //CheckForIllegalCrossThreadCalls = false;
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
                    richTextBoxLogs.Text = string.Empty;
                    InitializeTransferOperation();

                    if (AdditionalConnectionDetails.Count < 1)
                    {
                        MessageBox.Show("Add an organization for data transfer! ");
                        return;
                    }
                    _logger.LogInfo("Transfer is started. ");
                    _logger.LogInfo($"Log folder path: {TxtLogsPath.Text}");
                    ChangeToolsState(false);
                    BtnTransferData.Enabled = true;

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
                            if (!CheckArgsResult(args)) return;
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
                    fetchXmls.Add(_popup.FetchXmls[row.Index]);
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
            _popup.SetTextBoxText(string.Empty);
            PopupDialog();
        }

        private void PopupDialog(int rowIndex = -1)
        {
            if (_popup.ShowDialog() == DialogResult.OK)
            {
                IDataverseService dataverseService = new DataverseService(Service);
                ChangeToolsState(false);
                string fetch = _popup.GetTextBoxText();

                if (rowIndex != -1 && fetch == _popup.FetchXmls[rowIndex])
                {
                    ChangeToolsState(true);
                    return;
                }
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading...",
                    Work = (worker, args) =>
                    {
                        try
                        {
                            (string logicalName, string displayName) = dataverseService.GetEntityName(fetch);

                            if (rowIndex != -1)
                            {
                                _popup.FetchXmls[rowIndex] = fetch;
                                FetchDataGridView.Invoke(new MethodInvoker(delegate
                                {
                                    fetchXmlDataBindingSource[rowIndex] = new FetchXmlData()
                                    {
                                        DisplayName = displayName,
                                        SchemaName = logicalName
                                    };
                                }));
                                _displayNames[rowIndex] = displayName;
                            }
                            else
                            {
                                _displayNames.Add(displayName);
                                FetchDataGridView.Invoke(new MethodInvoker(delegate
                                {
                                    fetchXmlDataBindingSource.Add(new FetchXmlData()
                                    {
                                        DisplayName = displayName,
                                        SchemaName = logicalName
                                    });
                                }));
                                FetchDataGridView.Invoke((MethodInvoker)(() => FetchDataGridView.Rows[FetchDataGridView.Rows.Count - 1].Cells[0].Value = true));
                            }
                        }
                        catch (Exception ex)
                        {
                            if (rowIndex == -1)
                            {
                                _popup.FetchXmls.RemoveAt(_popup.FetchXmls.Count - 1);
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
            if (FetchDataGridView.Columns[e.ColumnIndex].Name == "Remove")
            {
                if (MessageBox.Show("Are you sure you want to delete this item?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    fetchXmlDataBindingSource.RemoveAt(e.RowIndex);
                    _popup.FetchXmls.RemoveAt(e.RowIndex);
                    _displayNames.RemoveAt(e.RowIndex);
                }
            }
            if (FetchDataGridView.Columns[e.ColumnIndex].Name == "Edit")
            {
                _popup.IsEdit = true;
                _popup.EditIndex = e.RowIndex;
                _popup.SetTextBoxText(_popup.FetchXmls[e.RowIndex]);
                PopupDialog(e.RowIndex);
                _popup.IsEdit = false;
            }
        }

        private void RichTextBoxLogs_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}