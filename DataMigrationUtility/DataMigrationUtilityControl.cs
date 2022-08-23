using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using XrmToolBox.Extensibility;
using System.Collections.Generic;
using XrmMigrationUtility.Services;
using System.Collections.Specialized;
using Microsoft.Xrm.Tooling.Connector;
using XrmMigrationUtility.Model.Interfaces;
using XrmMigrationUtility.Services.Interfaces;
using XrmMigrationUtility.Model.Implementations;
using XrmMigrationUtility.Services.Implementations;

namespace XrmMigrationUtility
{
    internal partial class DataMigrationUtilityControl : MultipleConnectionsPluginControlBase
    {
        private ILogger logger;

        private string logsPath;

        private Settings mySettings;

        private List<string> entityNames;

        private const int ERROR_CODE = -2147220685;

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
                    FetchXmlFileNames = Directory.GetFiles(FetchXmlFolderPath).Select(fileName => fileName.Replace(FetchXmlFolderPath + @"\", "")).ToList();//.Replace(".xml", "")).ToList();
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
            ListBoxOrganizations.DataSource = null;
            ListBoxOrganizations.DataSource = AdditionalConnectionDetails.ToList();
            ListBoxOrganizations.DisplayMember = "ConnectionName";
            ListBoxOrganizations.ValueMember = "ConnectionId";
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
                //ListBoxTxtFetch.Items.Clear();
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
            if (ListBoxOrganizations.Items.Count >= 1)
            {
                AdditionalConnectionDetails.Clear();
            }
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
            logger.Log($"entities count: {entityNames.Count}");
            logger.Log($"Log folder path: {TxtLogsPath.Text}");
            logger.Log($"Fetch folder path: {TxtFetchPath.Text}");

            List<IResultItem> resultItem = new List<IResultItem>();
            WorkAsync(new WorkAsyncInfo
            {
                Message = null,
                Work = (worker, args) =>
                {
                    try
                    {
                        ChangeToolsState(false);
                        IDataverseService d365source = new DataverseService((CrmServiceClient)Service);
                        foreach (string entity in entityNames)
                        {
                            logger.Log("Getting data of '" + entity + "' from source instance");

                            IResultItem currentResult = GetCurrentResult(d365source, entity);
                            if (currentResult == null)
                            {
                                logger.Log("Process Stopped. Aborting! ");
                                break;
                            }
                            resultItem.Add(currentResult);
                        }
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
            ListBoxOrganizations.Enabled = state;
        }

        private IResultItem GetCurrentResult(IDataverseService d365source, string entity)
        {
            bool stop = false;
            IResultItem currentResult = new ResultItem(entity);

            string fetchPath = TxtFetchPath.Text;
            string entityFetch = ConfigReader.GetQuery(entity, out List<string> searchAttrs, fetchPath, out bool idExists);

            EntityCollection records = d365source.GetAllRecords(entityFetch);

            currentResult.SourceRecordCount = records.Entities.Count;
            logger.Log("Records count is: " + records.Entities.Count);

            if (records?.Entities?.Count > 0)
            {
                TransferData(records, currentResult, searchAttrs, out stop, idExists);
            }
            else
            {
                logger.Log("Records count is zero or not found");
            }
            if (stop)
            {
                logger.Log("Process Stopped. Aborting!");
                return null;
            }

            return currentResult;
        }

        private void TransferData(EntityCollection records, IResultItem currentResult, List<string> searchAttrs, out bool stop, bool idExists)
        {
            stop = false;

            foreach (ConnectionDetail detail in AdditionalConnectionDetails)
            {
                if (stop) break;

                IDataverseService d365Target = new DataverseService(detail.ServiceClient);
                logger.Log("Transfering data to: " + detail.OrganizationDataServiceUrl);
                //MessageBox.Show(detail.OrganizationUrlName);//target name
                foreach (Entity record in records.Entities)
                {
                    if (stop) break;

                    string primaryAttr = d365Target.GetEntityPrimaryField(record.LogicalName);
                    string recordFirstValue = record.GetAttributeValue<string>(primaryAttr);
                    string recordId = record.Id.ToString();
                    logger.Log("Record with id '" + recordId + "' and with name '" + recordFirstValue + "' is creating in target...");

                    Entity newRecord = new Entity(record.LogicalName);
                    newRecord.Attributes.AddRange(record.Attributes);
                    if (idExists)
                    {
                        newRecord.Attributes.Remove(newRecord.LogicalName + "id");
                    }

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
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex.Message);
                    }
                }
            }
        }
    }
}