using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using System.Collections.Generic;
using XrmMigrationUtility.Services;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Tooling.Connector;
using XrmMigrationUtility.Model.Interfaces;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Model
{
    public class TransferOperation
    {
        public List<IResultItem> ResultItem { get; private set; }

        private readonly IOrganizationService service;

        private readonly ObservableCollection<ConnectionDetail> additionalConnectionDetails;

        private readonly ILogger logger;

        private readonly List<string> entityNames;

        private readonly TextBox txtFetchPath;

        private IResultItem currentResult;

        private const int ERROR_CODE = -2147220685;

        public TransferOperation(IOrganizationService service, ObservableCollection<ConnectionDetail> additionalConnectionDetails,
            ILogger logger, List<string> entityNames, TextBox txtFetchPath)
        {
            this.service = service;
            this.logger = logger;
            this.additionalConnectionDetails = additionalConnectionDetails;
            this.entityNames = entityNames;
            this.txtFetchPath = txtFetchPath;
        }

        public void Transfer()
        {
            ResultItem = new List<IResultItem>();
            foreach (string entityName in entityNames)
            {
                logger.Log("Getting data of '" + entityName + "' from source instance");
                currentResult = Injection.GetResultItemInstance(entityName);

                string fetchPath = txtFetchPath.Text;
                string entityFetch = ConfigReader.GetQuery(entityName, out List<string> searchAttrs, fetchPath, out bool isIdExist);

                IDataverseService d365source = Injection.GetDataverseServiceInstance((CrmServiceClient)service);
                EntityCollection records = d365source.GetAllRecords(entityFetch);

                currentResult.SourceRecordCount = records.Entities.Count;
                logger.Log("Records count is: " + records.Entities.Count);

                if (records?.Entities?.Count > 0)
                {
                    TransferData(records, searchAttrs, isIdExist);
                }
                else
                {
                    logger.Log("Records count is zero or not found");
                }
                if (currentResult == null)
                {
                    logger.Log("Process Stopped. Aborting! ");
                    break;
                }
                ResultItem.Add(currentResult);
            }
        }

        private void TransferData(EntityCollection records, List<string> searchAttrs, bool isIdExist)
        {
            bool stop = false;

            foreach (ConnectionDetail detail in additionalConnectionDetails)
            {
                if (stop) break;

                IDataverseService d365Target = Injection.GetDataverseServiceInstance(detail.ServiceClient);
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
                    if (!isIdExist)
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
                        if (ex.Detail.ErrorCode == ERROR_CODE) //DuplicateRecordsFound
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