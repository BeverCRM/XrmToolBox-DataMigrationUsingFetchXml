using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using Microsoft.Xrm.Sdk;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using XrmToolBox.Extensibility;
using XrmMigrationUtility.Model;
using System.Collections.Generic;
using System.Collections.Specialized;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility
{
    internal partial class DataMigrationUtilityControl : MultipleConnectionsPluginControlBase
    {
        private string _logsPath;

        private Settings _mySettings;

        private List<string> _entityNames;

        private string _fetchXmlFolderPath;

        private readonly ILogger _logger;

        private readonly ITransferOperation _transferOperation;

        private readonly string _defaultPath = Environment.CurrentDirectory;

        public DataMigrationUtilityControl(ILogger logger, ITransferOperation transferOperation)
        {
            _logger = logger;
            _transferOperation = transferOperation;
            InitializeComponent();
        }

        private void GetFetchFileNames()
        {
            try
            {
                if (!string.IsNullOrEmpty(_fetchXmlFolderPath) || !string.IsNullOrWhiteSpace(_fetchXmlFolderPath))
                {
                    List<string> FetchXmlFileNames;
                    _entityNames = new List<string>();
                    FetchXmlFileNames = Directory.GetFiles(_fetchXmlFolderPath).Select(fileName => fileName.Replace(_fetchXmlFolderPath + @"\", "")).ToList();
                    TextBoxFetchFiles.Text = null;
                    foreach (string item in FetchXmlFileNames)
                    {
                        if (item.Contains(".xml"))
                        {
                            TextBoxFetchFiles.Text += item + ", ";
                            _entityNames.Add(item.Replace(".xml", ""));
                        }
                    }
                    if (_entityNames.Count >= 1)
                    {
                        TextBoxFetchFiles.Text = TextBoxFetchFiles.Text.Remove(TextBoxFetchFiles.Text.Length - 2);
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show($"Input Valid Path!: {ex.Message}");
                TxtFetchPath.Text = null;
            }
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
                    LblIsTargetFilled.Text = detail.ConnectionName;
                    LblIsTargetFilled.ForeColor = Color.Green;
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
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select Logs path" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    TxtLogsPath.Text = fbd.SelectedPath;
                    _logsPath = fbd.SelectedPath;
                }
            }
        }

        private void BtnBrowseFetch_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select Fetch path" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _fetchXmlFolderPath = fbd.SelectedPath;
                    TxtFetchPath.Text = fbd.SelectedPath;
                }
            }
            if (TxtFetchPath.Text != string.Empty)
            {
                GetFetchFileNames();
            }
        }

        private void TxtFetchPathLeave(object sender, EventArgs e)
        {
            if (TxtFetchPath.Text != string.Empty)
            {
                _fetchXmlFolderPath = TxtFetchPath.Text;
                GetFetchFileNames();
            }
            else
            {
                TextBoxFetchFiles.Text = null;
            }
        }

        private void TxtLogsPathLeave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtLogsPath.Text))
            {
                _logsPath = TxtLogsPath.Text;
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
            ConnectionDetails additionalDetails = new ConnectionDetails();
            TxtLogs.Text = string.Empty;

            _logger.SetTxtLogs(TxtLogs);
            _logger.SetLogsPath(_logsPath);
            additionalDetails.AdditionalConnectionDetails = AdditionalConnectionDetails;
            additionalDetails.Service = Service;
            _transferOperation.InitialiseFields(additionalDetails, _entityNames, TxtFetchPath.Text);

            if (AdditionalConnectionDetails.Count < 1)
            {
                MessageBox.Show("Add an organization for data transfer! ");
                return;
            }
            _logger.Log("Transfer is started. ");
            _logger.Log($"entities count: {_entityNames.Count}");
            _logger.Log($"Log folder path: {TxtLogsPath.Text}");
            _logger.Log($"Fetch folder path: {TxtFetchPath.Text}");

            List<ResultItem> resultItems = null;
            WorkAsync(new WorkAsyncInfo
            {
                Message = null,
                Work = (worker, args) =>
                {
                    try
                    {
                        ChangeToolsState(false);
                        _transferOperation.Transfer();

                        resultItems = _transferOperation.ResultItems;
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(ex.Message);
                        _logger.Log($"[trace log] {ex.StackTrace}");
                    }
                    finally
                    {
                        ChangeToolsState(true);
                        _logger.Log("Result: ");
                        foreach (ResultItem resultItem in resultItems)
                            _logger.Log($"{resultItem.EntityName}, {resultItem.SourceRecordCount } (Source Records), {resultItem.SuccessfullyGeneratedRecordCount } (Generated Records)");

                        MessageBox.Show("Transfer Completed");
                    }
                }
            });
        }

        private void ChangeToolsState(bool state)
        {
            TxtLogsPath.Enabled = state;
            TxtFetchPath.Enabled = state;
            BtnBrowseLogs.Enabled = state;
            BtnBrowseFetch.Enabled = state;
            BtnTransferData.Enabled = state;
            BtnSelectTargetInstance.Enabled = state;
            TextBoxFetchFiles.Enabled = state;
            BtnClearLogs.Enabled = state;
        }

        private void BtnClearLogs_Click(object sender, EventArgs e)
        {
            TxtLogs.Text = null;
        }
    }
}