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
using XrmMigrationUtility.Model.Interfaces;
using XrmMigrationUtility.Services.Interfaces;
using XrmMigrationUtility.Model.Implementations;

namespace XrmMigrationUtility
{
    internal partial class DataMigrationUtilityControl : MultipleConnectionsPluginControlBase
    {
        private ILogger logger;

        private string logsPath;

        private Settings mySettings;

        private List<string> entityNames;

        private string FetchXmlFolderPath { get; set; }

        private readonly string defaultPath = Environment.CurrentDirectory;

        public DataMigrationUtilityControl()
        {
            InitializeComponent();
        }

        private void GetFetchFileNames()
        {
            try
            {
                if (!string.IsNullOrEmpty(FetchXmlFolderPath) || !string.IsNullOrWhiteSpace(FetchXmlFolderPath))
                {
                    List<string> FetchXmlFileNames;
                    entityNames = new List<string>();
                    FetchXmlFileNames = Directory.GetFiles(FetchXmlFolderPath).Select(fileName => fileName.Replace(FetchXmlFolderPath + @"\", "")).ToList();
                    TextBoxFetchFiles.Text = null;
                    foreach (string item in FetchXmlFileNames)
                    {
                        if (item.Contains(".xml"))
                        {
                            TextBoxFetchFiles.Text += item + ", ";
                            entityNames.Add(item.Replace(".xml", ""));
                        }
                    }
                    if (entityNames.Count >= 1)
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
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
            TxtLogsPath.Text = defaultPath;
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataMigrationUtilityControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
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
                    logsPath = fbd.SelectedPath;
                }
            }
        }

        private void BtnBrowseFetch_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select Fetch path" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    FetchXmlFolderPath = fbd.SelectedPath;
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
                FetchXmlFolderPath = TxtFetchPath.Text;
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
                logsPath = TxtLogsPath.Text;
            }
            else
            {
                logsPath = defaultPath;
                TxtLogsPath.Text = logsPath;
            }
        }

        private void BtnSelectTargetInstance_Click(object sender, EventArgs e)
        {
            AddAdditionalOrganization();
        }

        private void BtnTransferData_Click(object sender, EventArgs e)
        {
            TxtLogs.Text = string.Empty;
            logger = Injection.GetLoggerInstance(TxtLogs, logsPath);

            if (AdditionalConnectionDetails.Count < 1)
            {
                MessageBox.Show("Add an organization for data transfer! ");
                return;
            }
            logger.Log("Transfer is started. ");
            logger.Log($"entities count: {entityNames.Count}");
            logger.Log($"Log folder path: {TxtLogsPath.Text}");
            logger.Log($"Fetch folder path: {TxtFetchPath.Text}");

            List<IResultItem> resultItem = null;
            WorkAsync(new WorkAsyncInfo
            {
                Message = null,
                Work = (worker, args) =>
                {
                    try
                    {
                        ChangeToolsState(false);
                        TransferOperation transferOperation = new TransferOperation(Service, AdditionalConnectionDetails, logger, entityNames, TxtFetchPath);
                        transferOperation.Transfer();

                        resultItem = transferOperation.ResultItem;
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex.Message);
                        logger.Log($"[trace log] {ex.StackTrace}");
                    }
                    finally
                    {
                        ChangeToolsState(true);
                        logger.Log("Result: ");
                        foreach (ResultItem r in resultItem)
                            logger.Log($"{r.EntityName}, {r.SourceRecordCount } (Source Records), {r.SuccessfullyGeneratedRecordCount } (Generated Records)");

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