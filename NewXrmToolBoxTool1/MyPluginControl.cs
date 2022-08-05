using System;
using System.IO;
using System.Data;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk.Query;
using XrmToolBox.Extensibility;
using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Specialized;
using NewXrmToolBoxTool1.Model;
using Microsoft.Xrm.Tooling.Connector;
using System.ServiceModel;
using Microsoft.VisualBasic;

namespace NewXrmToolBoxTool1
{
    public partial class MyPluginControl : MultipleConnectionsPluginControlBase
    {
        public string FetchXmlFolderPath { get; set; }
        public List<string> FetchXmlFileNames { get; set; }

        private Settings mySettings;

        Logger logger;

        string logsPath;

        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void GetFetchFileNames()
        {
            try
            {
                FetchXmlFileNames = Directory.GetFiles(FetchXmlFolderPath).Select(fileName => fileName.Replace(FetchXmlFolderPath + @"\", "").Replace(".xml", "")).ToList();
                ListBoxTxtFetch.Items.Clear();
                foreach (string item in FetchXmlFileNames)
                {
                    ListBoxTxtFetch.Items.Add(item);
                    //ListBoxTxtFetch.Text += Environment.NewLine;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Input Valid Path!");
                TxtFetchPath.Text = null;
                ListBoxTxtFetch.Items.Clear();
            }
        }

        //private void LogsFunction()
        //{
        //    logger = new Logger(TxtLogs);

        //    logger.Log("First");

        //    //await RetrieveInstances.GetData(logger);

        //    WhoAmIResponse whoAmI = (WhoAmIResponse)Service.Execute(new WhoAmIRequest());
        //    Guid userId = whoAmI.UserId;

        //    var User = Service.Retrieve("systemuser", userId, new ColumnSet("fullname"));
        //    string fullName = User["fullname"].ToString();
        //    logger.Log("fullName: " + fullName);
        //    Entity user = Service.Retrieve("systemuser", userId, new ColumnSet(new String[] { "internalemailaddress" }));
        //    string username = user["internalemailaddress"].ToString();

        //    logger.Log("Second");



        //    logger.Log("33333");
        //}

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            //logger = new Logger(TxtLogs);
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
            //LogsFunction();
            logsPath = "D:\\XrmToolbox";
            TxtLogsPath.Text = logsPath;
            //logger.Log("Load!");
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
            //textBoxCollEvents.Text += $"Action: {e.Action.ToString()}, NewStartingIndex: {e.NewStartingIndex}," +
            //    $" OldStartingIndex: {e.OldStartingIndex}, Item Count: {AdditionalConnectionDetails.Count} {Environment.NewLine}";

            // update the list box with the connections
            ListBoxOrganizations.DataSource = null;
            ListBoxOrganizations.DataSource = AdditionalConnectionDetails.ToList();
            ListBoxOrganizations.DisplayMember = "ConnectionName";
            ListBoxOrganizations.ValueMember = "ConnectionId";
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
            //logger.Log("BtnOpenFetchPath_Click");
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
            //logger.Log("txtFetchLeave");
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

        private void TxtLogsPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtFetchPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtLogs_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnAddOrganization_Click(object sender, EventArgs e)
        {
            AddAdditionalOrganization();
        }

        private void BtnRemoveOrganization_Click(object sender, EventArgs e)
        {
            var conn = ListBoxOrganizations.SelectedItem as ConnectionDetail;
            if (conn != null)
            {
                RemoveAdditionalOrganization(conn);
            }
        }

        private void TextBoxOrganizationData_TextChanged(object sender, EventArgs e)
        {

        }

        private void ListBoxOrganizations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var conn = ListBoxOrganizations.SelectedItem as ConnectionDetail;
            BtnRemoveOrganization.Enabled = (conn != null);
        }

        private void ListBoxTxtFetch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TxtLogsPathLeave(object sender, EventArgs e)
        {
            logsPath = TxtLogsPath.Text;
        }

        private void BtnTransferData_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(ListBoxOrganizations.SelectedItem.ToString());//name
            //MessageBox.Show(ListBoxOrganizations.SelectedValue.ToString());//Id

            //foreach (ConnectionDetail detail in AdditionalConnectionDetails)
            //{
            //    var resp = detail.GetCrmServiceClient().Execute(new WhoAmIRequest()) as WhoAmIResponse;

            //    //TxtLogs.Text += $"0: {detail.ImpersonatedUserName}," +
            //    //    $"  1: {detail.OrganizationFriendlyName}, 2: {detail.OrganizationServiceUrl}, 3: {detail.OrganizationUrlName}," +
            //    //    $" 4: {detail.EnvironmentId}, 5: {detail.OrganizationUrlName}, 6: {detail.OriginalUrl}," +
            //    //    $" 7: {detail.OrganizationDataServiceUrl}, 8: {detail.WebApplicationUrl}, 9: {detail.ReplyUrl}{Environment.NewLine}";//7
            //    logger.Log("ListBoxOrganizations" + ListBoxOrganizations.SelectedItem.ToString());
            //    logger.Log("detail.OrganizationUrlName" + detail.OrganizationUrlName);
            //    //if (ListBoxOrganizations.SelectedItem.ToString().ToLower() == detail.OrganizationUrlName.ToLower())
            //    //{

            //    //D365Utility d365Utility = new D365Utility(detail.ServiceClient);
            //    //MessageBox.Show("0");
            //    //List<string> entities = ConfigReader.GetMultipleNodes("entity");
            //    //MessageBox.Show("1");
            //    //foreach (var entity in entities)
            //    //{
            //    //    MessageBox.Show(entity);
            //    //}
            //    //}
            //}

            if (ListBoxOrganizations.Items.Count < 1)
            {
                MessageBox.Show("Add an organization for data transfer! ");
                return;
            }

            TxtLogs.Text = string.Empty;
            logger = new Logger(TxtLogs, logsPath);

            logger.Log($"[{DateTime.Now.ToString("G")}]: Transfer is started. ");

            int count = 1;
            List<string> entities = new List<string>();
            foreach (var item in ListBoxTxtFetch.Items)
            {
                entities.Add(item.ToString());
                logger.Log($"{count}: Entity name -> {item}");
                count++;
            }

            logger.Log($"Log folder path: {TxtLogsPath.Text}");
            logger.Log($"Fetch folder path: {TxtFetchPath.Text}");

            logger.Log($"entities count: {entities.Count}");

            List<ResultItem> result = new List<ResultItem>();

            try
            {
                //logger.Log("Information from config file is retrieved.");
                //logger.Log("Connecting to source instance...");

                D365Utility D365source = new D365Utility((CrmServiceClient)Service);

               // logger.Log("Connected to source instance: ");

                bool stop = false;

                foreach (string entity in entities)
                {
                    if (stop)
                    {
                        logger.Log("Process Stopped. Aborting !");
                        break;
                    }

                    var currentResult = new ResultItem(entity);

                    logger.Log("Getting data of '" + entity + "' from source instance");
                    List<string> searchAttrs = new List<string>();
                    string fetchPath = TxtFetchPath.Text;
                    logger.Log($"FetchPath: {fetchPath}");
                    logger.Log(fetchPath + "\\" + entity + ".xml");
                    string entityFetch = ConfigReader.GetQuery(entity, out searchAttrs, fetchPath);
                    logger.Log($"entityFetch: {entityFetch}");
                    EntityCollection records = D365source.GetAllRecords(entityFetch);
                    currentResult.NumberOfSourceRecords = records.Entities.Count;
                    logger.Log("Records count is: " + records.Entities.Count);

                    if (records?.Entities?.Count > 0)
                    {
                        foreach (ConnectionDetail detail in AdditionalConnectionDetails)
                        {
                            if (stop) break;

                            D365Utility d365Target = new D365Utility(detail.ServiceClient);
                            logger.Log("Transfering data to: " + detail.OrganizationDataServiceUrl);

                            foreach (Entity record in records.Entities)
                            {
                                if (stop) break;

                                string primaryAttr = d365Target.GetEntityPrimaryField(record.LogicalName);
                                string recordFirstValue = record.GetAttributeValue<string>(primaryAttr);
                                string recordId = record.Id.ToString();
                                logger.Log("Record with id '" + recordId + "' and with name '" + recordFirstValue + "' is creating in target...");

                                var newRecord = new Entity(record.LogicalName);
                                newRecord.Attributes.AddRange(record.Attributes);
                                newRecord.Attributes.Remove(newRecord.LogicalName + "id");

                                try
                                {
                                    d365Target.MapSearchAttributes(newRecord, searchAttrs, D365source, logger);

                                    Guid createdRecordId = d365Target.CreateRecord(newRecord, false);
                                    ++currentResult.NumberOfGeneratedRecords;
                                    logger.Log($"Record is created with id {{{createdRecordId}}}");
                                }
                                catch (FaultException<OrganizationServiceFault> ex)
                                {
                                    // || (record.LogicalName == "salesorder" && ex.Message.Contains("Cannot insert duplicate key"))
                                    if (ex.Detail.ErrorCode == -2147220685) // DuplicateRecordsFound
                                    {
                                        logger.Log("The record was not created because a duplicate of the current record already exists.");
                                    }
                                    else
                                    {
                                        logger.Log(ex.Message);
                                        stop = StopProcess(logger);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Log(ex.Message);
                                    stop = StopProcess(logger);
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Log("Records count is zero or not found");
                    }
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
                    logger.Log($"{r.EntityName}, {r.NumberOfSourceRecords} (Source Records), {r.NumberOfGeneratedRecords} (Generated Records)");
            }
        }

        private bool StopProcess(Logger logger)
        {
            bool stop = false;
            logger.Log("Do you want to continue? (Y/N)");

            var pressedKey = Interaction.InputBox("Do you want to continue? (Y/N)");

            while (pressedKey == "n" || pressedKey == "N")
            {
                stop = true;
                return stop;
            }

            return stop;
        }
    }
}