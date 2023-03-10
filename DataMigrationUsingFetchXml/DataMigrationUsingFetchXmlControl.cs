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
        private readonly List<string> _displayNames;
        private readonly string _defaultPath = Environment.CurrentDirectory;

        private readonly FetchXmlPopup _fetchXmlpopup;
        private readonly MatchedAction _matchedAction;
        private readonly MatchingCriteria _matchingCriteria;

        private readonly ILogger _logger;
        private readonly ITransferOperation _transferOperation;

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
                    LblSource.Visible = true;
                    LblSourceText.Visible = true;
                    break;

                case "Target":
                    LblTargetText.Text = detail.ConnectionName;
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
            if (AdditionalConnectionDetails.Count <= 0)
            {
                MessageBox.Show(Localization_MessageBoxText.SelectTargetInstance, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (BtnTransferData.Text == "Cancel")
            {
                CancelWorker();
                BtnTransferData.Enabled = false;
                BtnTransferData.Text = "Transfer Data";

                return;
            }

            InitializeLog();
            (List<string> fetchXmls, List<int> tableIndexesForTransfer) = GetFetchDataGridViewData();

            if (fetchXmls.Count <= 0)
            {
                MessageBox.Show(Localization_MessageBoxText.SelectRowForTransfer, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                    _transferOperation.Transfer(fetchXmls, tableIndexesForTransfer, worker, args);
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
                    LogResultItems();
                    fetchXmls.Clear();
                    SetLoadingDetails(false);
                    SetTotalDetailsOfAllEntities();

                    if (!CheckWorkerResult(args))
                    {
                        UncheckCompletedFetchXmlRows(_transferOperation.CurrentIndexForTransfer);

                        return;
                    }
                    UncheckCompletedFetchXmlRows(++_transferOperation.CurrentIndexForTransfer);

                    MessageBox.Show(Localization_MessageBoxText.DataMigrationCompleted, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        private void UncheckCompletedFetchXmlRows(int countOfCompletedRows)
        {
            for (int i = 0; i < countOfCompletedRows; i++)
            {
                FetchDataGridView.Rows[i].Cells[0].Value = false;
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

        private void SetTotalDetailsOfAllEntities()
        {
            if (_transferOperation.ResultItems != null)
            {
                int totalRecordsCount = 0,
                    totalCreatedRecordsCount = 0,
                    totalUpdatedRecordsCount = 0,
                    totalDeletedRecordsCount = 0,
                    totalErroredRecordsCount = 0,
                    totalSkippedRecordsCount = 0;

                foreach (ResultItem resultItem in _transferOperation.ResultItems)
                {
                    totalRecordsCount += resultItem.SourceRecordCount;
                    totalCreatedRecordsCount += resultItem.CreatedRecordCount;
                    totalUpdatedRecordsCount += resultItem.UpdatedRecordCount;
                    totalDeletedRecordsCount += resultItem.DeletedRecordCount;
                    totalErroredRecordsCount += resultItem.ErroredRecordCount;
                    totalSkippedRecordsCount += resultItem.SkippedRecordCount;
                }

                LblRecordCount.Text = $"Total number of Records: {totalRecordsCount}";
                LblCreated.Text = $"Total created Records: {totalCreatedRecordsCount}";
                LblErrored.Text = $"Total errored Records: {totalErroredRecordsCount}";
                LblForActions.ForeColor = Color.Gold;
                LblForActions.Text = $"Total deleted Records: {totalDeletedRecordsCount}";
                LblUpdate.Text = $"Total updated Records: {totalUpdatedRecordsCount}";
                LblSkipped.Text = $"Total skipped Records: {totalSkippedRecordsCount}";
            }
        }

        private bool CheckWorkerResult(RunWorkerCompletedEventArgs args)
        {
            if (args.Cancelled)
            {
                MessageBox.Show(Localization_MessageBoxText.MigrationIsStopped, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }

            if (args.Error != null)
            {
                _logger.LogError(args.Error.Message);
                _logger.LogError($"[trace log] {args.Error.StackTrace}");
                MessageBox.Show(args.Error.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }

        private void LogResultItems()
        {
            _logger.LogInfo("Result: ");

            if (_transferOperation.ResultItems != null)
            {
                foreach (ResultItem resultItem in _transferOperation.ResultItems)
                {
                    _logger.LogInfo($"{resultItem.SchemaName}, {resultItem.SourceRecordCountWithSign} (Source Records)," +
                        $" {resultItem.CreatedRecordCount} (Created Records), {resultItem.ErroredRecordCount} (Errօred Records), {resultItem.SkippedRecordCount} (Warning Records)," +
                        $" {resultItem.UpdatedRecordCount} (Updated Records), {resultItem.DeletedRecordCount} (Deleted Records)");
                }
            }
        }

        private void ChangeLabelText(ResultItem resultItem)
        {
            LblTitle.Text = $"Migrating {resultItem.DisplayName} records";
            LblRecordCount.Text = $"Records Count: {resultItem.SourceRecordCountWithSign}";

            LblCreated.Text = $"Created Records: {resultItem.CreatedRecordCount}";
            LblErrored.Text = $"Errored Records: {resultItem.ErroredRecordCount}";

            if (MatchedAction.CheckedRadioButtonNumbers[_transferOperation.CurrentIndexForTransfer] == (byte)MatchedActionForRecord.DeleteAndCreate)
            {
                LblForActions.ForeColor = Color.Gold;
                LblForActions.Text = $"Deleted Records: {resultItem.DeletedRecordCount}";
            }
            else if (MatchedAction.CheckedRadioButtonNumbers[_transferOperation.CurrentIndexForTransfer] == (byte)MatchedActionForRecord.Upsert)
            {
                LblForActions.ForeColor = Color.Black;
                LblForActions.Text = $"Updated Records: {resultItem.UpdatedRecordCount}";
            }
            else if (MatchedAction.CheckedRadioButtonNumbers[_transferOperation.CurrentIndexForTransfer] == (byte)MatchedActionForRecord.DoNotCreate)
            {
                LblForActions.ForeColor = Color.Black;
                LblForActions.Text = $"Skipped Records: {resultItem.SkippedRecordCount}";
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
                LblCreated.Text = "Loading...";
                LblErrored.Visible = visible;
                LblCreated.Visible = visible;
                LblForActions.Visible = visible;
                LblRecordCount.Visible = visible;
                LblUpdate.Visible = visible;
                LblSkipped.Visible = visible;
                LblErrored.Text = string.Empty;
                LblForActions.Text = string.Empty;
                LblRecordCount.Text = string.Empty;
                LblSkipped.Text = string.Empty;
                LblUpdate.Text = string.Empty;
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
            LblCreated.Text = string.Empty;
            LblErrored.Text = string.Empty;
            LblRecordCount.Text = string.Empty;
            LblForActions.Text = string.Empty;
            LblSkipped.Text = string.Empty;
            LblUpdate.Text = string.Empty;
        }

        private void PictureBoxAdd_Click(object sender, EventArgs e)
        {
            if (Service == null)
            {
                MessageBox.Show(Localization_MessageBoxText.ConnectToTheSourceInstance, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                InitializeLog();
                _fetchXmlpopup.SetTextBoxText(string.Empty);
                FetchXmlPopupDialog();
                CustomiseFetchDataGridViewSizeBasedOnRowHeight(22);
            }
        }

        private void FetchXmlPopupDialog(int rowIndex = -1)
        {
            IDataverseService dataverseService = new DataverseService(Service);
            _fetchXmlpopup.SetDataverseService(dataverseService);
            _fetchXmlpopup.SetMatchingCriteria(_matchingCriteria);

            if (_fetchXmlpopup.ShowDialog() == DialogResult.OK)
            {
                ChangeToolsState(false);

                string fetch = _fetchXmlpopup.GetTextBoxText();

                if (rowIndex != -1 && fetch == _fetchXmlpopup.FetchXmls[rowIndex])
                {
                    ChangeToolsState(true);
                    return;
                }

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading...",
                    Work = (worker, args) =>
                    {
                        (string logicalName, string displayName) = dataverseService.GetEntityName(fetch);

                        _matchingCriteria.CreateLayoutPanels(rowIndex);

                        FetchDataGridView.Invoke(new MethodInvoker(delegate
                        {
                            if (rowIndex != -1)
                            {
                                _fetchXmlpopup.FetchXmls[rowIndex] = fetch;

                                fetchXmlDataBindingSource[rowIndex] = new FetchXmlDataBindingSourceData()
                                {
                                    DisplayName = displayName,
                                    SchemaName = logicalName
                                };
                                _displayNames[rowIndex] = displayName;
                            }
                            else
                            {
                                _displayNames.Add(displayName);

                                fetchXmlDataBindingSource.Add(new FetchXmlDataBindingSourceData()
                                {
                                    DisplayName = displayName,
                                    SchemaName = logicalName
                                });
                                FetchDataGridView.Rows[FetchDataGridView.Rows.Count - 1].Cells[0].Value = true;
                                FetchDataGridView.Rows[FetchDataGridView.Rows.Count - 1].Cells[3].Value = "Create";
                                MatchedAction.CheckedRadioButtonNumbers.Add((byte)MatchedActionForRecord.Create);
                            }
                        }));
                    },
                    PostWorkCallBack = args =>
                    {
                        if (args.Error != null)
                        {
                            if (rowIndex == -1)
                            {
                                _fetchXmlpopup.FetchXmls.RemoveAt(_fetchXmlpopup.FetchXmls.Count - 1);
                            }
                            MessageBox.Show($"{args.Error.Message}.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        ChangeToolsState(true);
                    }
                });
            }
        }

        private void FetchDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (FetchDataGridView.Columns[e.ColumnIndex].Name == "Remove" &&
                    MessageBox.Show(Localization_MessageBoxText.DeleteItem, "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    fetchXmlDataBindingSource.RemoveAt(e.RowIndex);
                    _fetchXmlpopup.FetchXmls.RemoveAt(e.RowIndex);
                    _displayNames.RemoveAt(e.RowIndex);

                    MatchedAction.CheckedRadioButtonNumbers.RemoveAt(e.RowIndex);
                    _matchingCriteria.RemoveLayoutPanelData(e.RowIndex);

                    CustomiseFetchDataGridViewSizeBasedOnRowHeight(0);
                }
                else if (FetchDataGridView.Columns[e.ColumnIndex].Name == "Edit")
                {
                    _fetchXmlpopup.IsEdit = true;
                    _fetchXmlpopup.EditIndex = e.RowIndex;
                    _fetchXmlpopup.SetTextBoxText(_fetchXmlpopup.FetchXmls[e.RowIndex]);
                    FetchXmlPopupDialog(e.RowIndex);
                    _fetchXmlpopup.IsEdit = false;
                }
                else if (FetchDataGridView.Columns[e.ColumnIndex].Name == "MatchingCriteria")
                {
                    Invoke((MethodInvoker)delegate
                    {
                        if (MatchedAction.CheckedRadioButtonNumbers[e.RowIndex] != (byte)MatchedActionForRecord.Create)
                        {
                            _matchingCriteria.SetLayoutPanelData(e.RowIndex);
                            _matchingCriteria.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show(Localization_MessageBoxText.DisableMatchingCriteriaForActionCreate, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    });
                }
                else if (FetchDataGridView.Columns[e.ColumnIndex].Name == "ActionIfMatched")
                {
                    _matchedAction.CheckRadioButton(e.RowIndex);
                    _matchedAction.RowIndex = e.RowIndex;

                    if (_matchedAction.ShowDialog() == DialogResult.OK)
                    {
                        FetchDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = MatchedAction.SelectedActionShortDescription[MatchedAction.CheckedRadioButtonNumbers[e.RowIndex] - 1];
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
            int dataGridViewInitialWidth = 518;
            int dataGridViewScrollWidth = 18;
            int dataGridViewRowHeight = 23;

            foreach (DataGridViewRow row in FetchDataGridView.Rows)
            {
                height += row.Height;
            }

            if (height > FetchDataGridView.Height - dataGridViewRowHeight && FetchDataGridView.Width <= dataGridViewInitialWidth)
            {
                FetchDataGridView.Width += dataGridViewScrollWidth;
            }
            else if (height < FetchDataGridView.Height - dataGridViewRowHeight && FetchDataGridView.Width > dataGridViewInitialWidth)
            {
                FetchDataGridView.Width -= dataGridViewScrollWidth;
            }
        }

        private void PictureBoxBever_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://marketplace.bevercrm.com/about");
        }
    }
}