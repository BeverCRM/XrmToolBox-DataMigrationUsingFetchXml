using System;
using System.IO;
using System.Drawing;
using Microsoft.Xrm.Sdk;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk.Query;
using XrmToolBox.Extensibility;
using XrmMigrationUtility.Model;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Specialized;
using Microsoft.WindowsAPICodePack.Dialogs;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility
{
    internal partial class DataMigrationUtilityControl : MultipleConnectionsPluginControlBase
    {
        private string _logsPath;

        private Settings _mySettings;

        private readonly Popup _popup;

        private readonly ILogger _logger;

        private readonly ITransferOperation _transferOperation;

        private readonly List<string> _displayNames;

        private readonly string _defaultPath = Environment.CurrentDirectory;

        public DataMigrationUtilityControl(ILogger logger, ITransferOperation transferOperation)
        {
            _logger = logger;
            _transferOperation = transferOperation;
            InitializeComponent();
            _popup = new Popup();
            _displayNames = new List<string>();
        }

        private void DataMigrationUtilityControl_Load(object sender, EventArgs e)
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
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataMigrationUtilityControl_OnCloseTool(object sender, EventArgs e)
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
                InitializeLog(richTextBoxLogs);
                _transferOperation.KeepRunning = true;
                List<string> fetchXmls = new List<string>();
                List<int> tableIndexesForTransfer = new List<int>();

                foreach (DataGridViewRow row in FetchDataGridView.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == "True")
                    {
                        fetchXmls.Add(_popup.FetchXmls[row.Index]);
                        tableIndexesForTransfer.Add(row.Index);
                    }
                    else
                        continue;
                }
                if (fetchXmls.Count > 0)
                {
                    SetLoadingDetails(true);
                    LblErrorText.Visible = true;
                    LblInfo.Visible = true;
                    richTextBoxLogs.Text = "";

                    ConnectionDetails connectionDetails = new ConnectionDetails
                    {
                        AdditionalConnectionDetails = AdditionalConnectionDetails,
                        Service = Service
                    };
                    _transferOperation.InitialiseFields(connectionDetails, LblInfo, LblTitle, LblErrorText, _displayNames);

                    if (AdditionalConnectionDetails.Count < 1)
                    {
                        MessageBox.Show("Add an organization for data transfer! ");
                        return;
                    }
                    _logger.Log("Transfer is started. ");
                    _logger.Log($"Log folder path: {TxtLogsPath.Text}");
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = null,
                        Work = (worker, args) =>
                        {
                            try
                            {
                                ChangeToolsState(false);
                                _transferOperation.Transfer(fetchXmls, tableIndexesForTransfer, richTextBoxLogs);
                            }
                            catch (Exception ex)
                            {
                                _logger.Log(ex.Message, true);
                                _logger.Log($"[trace log] {ex.StackTrace}", true);
                            }
                            finally
                            {
                                ChangeToolsState(true);
                                _logger.Log("Result: ");
                                foreach (ResultItem resultItem in _transferOperation.ResultItems)
                                {
                                    if (resultItem.ErroredRecordsCount > 0)
                                        _logger.Log($"{resultItem.EntityName}, {resultItem.SourceRecordCount } (Source Records), {resultItem.SuccessfullyGeneratedRecordCount } (Migrated Records), {resultItem.ErroredRecordsCount} (Errօred Records)");
                                    else
                                        _logger.Log($"{resultItem.EntityName}, {resultItem.SourceRecordCount } (Source Records), {resultItem.SuccessfullyGeneratedRecordCount } (Migrated Records)");
                                }
                                fetchXmls.Clear();
                                SetLoadingDetails(false);
                                if (_transferOperation.KeepRunning)
                                    MessageBox.Show("Data Migration Completed.", "Data Migration Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                else
                                    MessageBox.Show("Migration is Stopped.", "Migration is Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Select Entity For Data Transfering. ", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select Target Instance. ", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetLoadingDetails(bool visible)
        {
            if (visible == true)
            {
                LblInfo.Text = "Loading...";
            }
            LblLoading.Visible = visible;
            LblTitle.Visible = visible;
            pictureBoxStop.Visible = visible;
        }

        private void InitializeLog(RichTextBox TxtLogs)
        {
            TxtLogs.Text = string.Empty;
            richTextBoxLogs.Text = string.Empty;
            _logger.SetTxtLogs(TxtLogs);
            _logger.SetLogsPath(_logsPath);
        }

        private void ChangeToolsState(bool state)
        {
            TxtLogsPath.Enabled = state;
            BtnBrowseLogs.Enabled = state;
            BtnTransferData.Enabled = state;
            BtnSelectTargetInstance.Enabled = state;
            FetchDataGridView.Enabled = state;
            pictureBoxAdd.Enabled = state;
        }

        private void PictureBoxRecBin_Click(object sender, EventArgs e)
        {
            richTextBoxLogs.Text = null;
        }

        private void PictureBoxAdd_Click(object sender, EventArgs e)
        {
            InitializeLog(richTextBoxLogs);
            _popup.TextBoxFetch.Text = string.Empty;
            PopupDialog();
        }

        private void PopupDialog(int rowIndex = -1)
        {
            if (_popup.ShowDialog() == DialogResult.OK)
            {
                string fetch = _popup.TextBoxFetch.Text;

                if (rowIndex != -1 && fetch == _popup.FetchXmls[rowIndex])
                    return;

                EntityCollection returnCollection = Service.RetrieveMultiple(new FetchExpression(fetch));

                RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
                {
                    EntityFilters = EntityFilters.All,
                    LogicalName = returnCollection.Entities[0].LogicalName
                };
                RetrieveEntityResponse retrieveAccountEntityResponse = (RetrieveEntityResponse)Service.Execute(retrieveEntityRequest);
                EntityMetadata AccountEntity = retrieveAccountEntityResponse.EntityMetadata;

                if (rowIndex != -1)
                {
                    fetchXmlDataBindingSource[rowIndex] = new FetchXmlData()
                    { DisplayName = AccountEntity.DisplayName.UserLocalizedLabel.Label, SchemaName = returnCollection.Entities[0].LogicalName };
                    _displayNames[rowIndex] = AccountEntity.DisplayName.UserLocalizedLabel.Label;
                }
                else
                {
                    _displayNames.Add(AccountEntity.DisplayName.UserLocalizedLabel.Label);
                    fetchXmlDataBindingSource.Add(new FetchXmlData()
                    { DisplayName = AccountEntity.DisplayName.UserLocalizedLabel.Label, SchemaName = returnCollection.Entities[0].LogicalName });
                }
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
                _popup.isEdit = true;
                _popup.editIndex = e.RowIndex;
                _popup.TextBoxFetch.Text = _popup.FetchXmls[e.RowIndex];
                PopupDialog(e.RowIndex);
                _popup.isEdit = false;
            }
        }

        private void PictureBoxStop_Click(object sender, EventArgs e)
        {
            _transferOperation.KeepRunning = false;
        }
    }
}