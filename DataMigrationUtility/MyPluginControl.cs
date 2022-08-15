﻿using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Threading;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk.Query;
using XrmToolBox.Extensibility;
using System.Collections.Generic;
using XrmMigrationUtility.Services;
using System.Collections.Specialized;
using Microsoft.Xrm.Tooling.Connector;

namespace XrmMigrationUtility
{
    internal partial class MyPluginControl : MultipleConnectionsPluginControlBase
    {
        private string FetchXmlFolderPath { get; set; }

        private Settings mySettings;

        private Logger logger;

        private string logsPath;

        private const string DEFAULT_PATH = "D:\\";

        private const int ERROR_CODE = -2147220685;

        public MyPluginControl()
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
                    FetchXmlFileNames = Directory.GetFiles(FetchXmlFolderPath).Select(fileName => fileName.Replace(FetchXmlFolderPath + @"\", "").Replace(".xml", "")).ToList();
                    ListBoxTxtFetch.Items.Clear();
                    foreach (string item in FetchXmlFileNames)
                    {
                        ListBoxTxtFetch.Items.Add(item);
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show($"Input Valid Path!: {ex.Message}");
                TxtFetchPath.Text = null;
                //ListBoxTxtFetch.Items.Clear();
            }
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
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
            TxtLogsPath.Text = DEFAULT_PATH;
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbSample_Click(object sender, EventArgs e)
        {
            // The ExecuteMethod method handles connecting to an
            // organization if XrmToolBox is not yet connected
            ExecuteMethod(GetAccounts);
        }

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
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
        }

        protected override void ConnectionDetailsUpdated(NotifyCollectionChangedEventArgs e)
        {
            // update the list box with the connections
            ListBoxOrganizations.DataSource = null;
            ListBoxOrganizations.DataSource = AdditionalConnectionDetails.ToList();
            ListBoxOrganizations.DisplayMember = "ConnectionName";
            ListBoxOrganizations.ValueMember = "ConnectionId";
            BtnAddOrganization.Enabled = false;
        }

        private void BtnBrowseLogs_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select Logs path!" })
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
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select Fetch path!" })
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
                ListBoxTxtFetch.Items.Clear();
            }
        }

        private void BtnAddOrganization_Click(object sender, EventArgs e)
        {
            if (ListBoxOrganizations.Items.Count <= 1)
            {
                AddAdditionalOrganization();
            }
        }

        private void BtnRemoveOrganization_Click(object sender, EventArgs e)
        {
            var conn = ListBoxOrganizations.SelectedItem as ConnectionDetail;
            if (conn != null)
            {
                RemoveAdditionalOrganization(conn);
            }
            if (ListBoxOrganizations.Items.Count == 0)
            {
                BtnAddOrganization.Enabled = true;
            }
        }

        private void ListBoxOrganizations_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionDetail conn = ListBoxOrganizations.SelectedItem as ConnectionDetail;
            BtnRemoveOrganization.Enabled = (conn != null);
        }

        private void TxtLogsPathLeave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtLogsPath.Text))
            {
                logsPath = TxtLogsPath.Text;
            }
            else
            {
                logsPath = DEFAULT_PATH;
                TxtLogsPath.Text = logsPath;
            }
        }

        private List<string> GetEntities()
        {
            int count = 1;
            List<string> entities = new List<string>();
            foreach (var item in ListBoxTxtFetch.Items)
            {
                entities.Add(item.ToString());
                logger.Log($"{count}: Entity name -> {item}");
                count++;
            }
            logger.Log($"entities count: {entities.Count}");
            logger.Log($"Log folder path: {TxtLogsPath.Text}");
            logger.Log($"Fetch folder path: {TxtFetchPath.Text}");

            return entities;
        }

        private void BtnTransferData_Click(object sender, EventArgs e)
        {
            if (ListBoxOrganizations.Items.Count < 1)
            {
                MessageBox.Show("Add an organization for data transfer! ");
                return;
            }

            TxtLogs.Text = string.Empty;
            logger = new Logger(TxtLogs, logsPath);
            logger.Log("Transfer is started. ");

            List<ResultItem> result = new List<ResultItem>();
            List<string> entities = GetEntities();

            try
            {
                DataverseService D365source = new DataverseService((CrmServiceClient)Service);

                bool stop = false;

                foreach (string entity in entities)
                {
                    if (stop)
                    {
                        logger.Log("Process Stopped. Aborting! ");
                        break;
                    }
                    logger.Log("Getting data of '" + entity + "' from source instance");

                    ResultItem currentResult = GetCurrentResult(D365source, entity, out stop);

                    result.Add(currentResult);
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
                logger.Log($"[trace log] {ex.StackTrace}");
            }
            finally
            {
                logger.Log("Result: ");
                foreach (ResultItem r in result)
                    logger.Log($"{r.EntityName}, {r.SourceRecordCount } (Source Records), {r.SuccessfullyGeneratedRecordCount } (Generated Records)");
            }
        }

        private ResultItem GetCurrentResult(DataverseService D365source, string entity, out bool stop)
        {
            stop = false;
            ResultItem currentResult = new ResultItem(entity);
            List<string> searchAttrs = new List<string>();
            string fetchPath = TxtFetchPath.Text;
            string entityFetch = ConfigReader.GetQuery(entity, out searchAttrs, fetchPath);
            EntityCollection records = D365source.GetAllRecords(entityFetch);
            currentResult.SourceRecordCount = records.Entities.Count;
            logger.Log("Records count is: " + records.Entities.Count);

            if (records?.Entities?.Count > 0)
            {
                stop = TransferData(records, currentResult, searchAttrs);
            }
            else
            {
                logger.Log("Records count is zero or not found");
            }

            return currentResult;
        }

        private bool TransferData(EntityCollection records, ResultItem currentResult, List<string> searchAttrs)
        {
            bool stop = false;
            foreach (ConnectionDetail detail in AdditionalConnectionDetails)
            {
                if (stop) break;

                DataverseService d365Target = new DataverseService(detail.ServiceClient);
                logger.Log("Transfering data to: " + detail.OrganizationDataServiceUrl);

                foreach (Entity record in records.Entities)
                {
                    if (stop) break;

                    string primaryAttr = d365Target.GetEntityPrimaryField(record.LogicalName);
                    string recordFirstValue = record.GetAttributeValue<string>(primaryAttr);
                    string recordId = record.Id.ToString();
                    logger.Log("Record with id '" + recordId + "' and with name '" + recordFirstValue + "' is creating in target...");

                    Entity newRecord = new Entity(record.LogicalName);
                    newRecord.Attributes.AddRange(record.Attributes);
                    newRecord.Attributes.Remove(newRecord.LogicalName + "id");

                    try
                    {
                        d365Target.MapSearchAttributes(newRecord, searchAttrs, logger);

                        Guid createdRecordId = d365Target.CreateRecord(newRecord, false);
                        ++currentResult.SuccessfullyGeneratedRecordCount;
                        logger.Log($"Record is created with id {{{createdRecordId}}}");
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        // || (record.LogicalName == "salesorder" && ex.Message.Contains("Cannot insert duplicate key"))
                        if (ex.Detail.ErrorCode == ERROR_CODE) // DuplicateRecordsFound
                        {
                            logger.Log("The record was not created because a duplicate of the current record already exists.");
                        }
                        else
                        {
                            logger.Log(ex.Message);
                            stop = StopProcess();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex.Message);
                        stop = StopProcess();
                    }
                }
            }
            return stop;
        }

        private bool StopProcess()
        {
            bool stop = false;
            string message = "Do you want to continue?";
            logger.Log(message);

            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, "Something Wrong! ", buttons);

            if (result == DialogResult.No)
            {
                stop = true;
                logger.Log("No: ");
                return stop;
            }
            logger.Log("Yes: ");
            return stop;
        }
    }
}