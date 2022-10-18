using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using McTools.Xrm.Connection;
using XrmMigrationUtility.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Services.Implementations
{
    internal sealed class TransferOperation : ITransferOperation
    {
        public List<ResultItem> ResultItems { get; set; }

        private IOrganizationService _sourceService;

        private IDataverseService _dataverseService;

        private readonly ResultItem _resultItem;

        private readonly ILogger _logger;

        private string _fetchPathText;

        private List<string> _entityNames;

        private string _organizationDataServiceUrl;

        private const int ERROR_CODE = -2147220685;

        private ObservableCollection<ConnectionDetail> _additionalConnectionDetails;

        public TransferOperation(ILogger logger)
        {
            _logger = logger;
            _resultItem = new ResultItem();
        }

        public void InitialiseFields(ConnectionDetails connectionDetails, List<string> entityNames, string fetchPathText)
        {
            _entityNames = entityNames;
            _fetchPathText = fetchPathText;
            _sourceService = connectionDetails.Service;
            _additionalConnectionDetails = connectionDetails.AdditionalConnectionDetails;
            _organizationDataServiceUrl = _additionalConnectionDetails[0].OrganizationDataServiceUrl;
            _dataverseService = new DataverseService(_sourceService, _additionalConnectionDetails[0].ServiceClient, _logger);
        }

        public void Transfer()
        {
            ResultItems = new List<ResultItem>();

            foreach (string entityName in _entityNames)
            {
                _logger.Log("Getting data of '" + entityName + "' from source instance");
                _resultItem.EntityName = entityName;

                string entityFetch = ConfigReader.GetQuery(entityName, out List<string> searchAttrs, _fetchPathText, out bool idExists);

                EntityCollection records = _dataverseService.GetAllRecords(entityFetch);

                _resultItem.SourceRecordCount = records.Entities.Count;
                _logger.Log("Records count is: " + records.Entities.Count);

                if (records?.Entities?.Count > 0)
                {
                    TransferData(records, searchAttrs, idExists);
                }
                else
                {
                    _logger.Log("Records count is zero or not found");
                }
                if (_resultItem == null)
                {
                    _logger.Log("Process Stopped. Aborting! ");
                    break;
                }
                ResultItems.Add(_resultItem);
            }
        }

        private void TransferData(EntityCollection records, List<string> searchAttrs, bool idExists)
        {
            _logger.Log("Transfering data to: " + _organizationDataServiceUrl);

            foreach (Entity record in records.Entities)
            {
                string primaryAttr = _dataverseService.GetEntityPrimaryField(record.LogicalName);
                string recordFirstValue = record.GetAttributeValue<string>(primaryAttr);
                string recordId = record.Id.ToString();
                _logger.Log("Record with id '" + recordId + "' and with name '" + recordFirstValue + "' is creating in target...");

                Entity newRecord = new Entity(record.LogicalName);
                newRecord.Attributes.AddRange(record.Attributes);
                if (!idExists)
                {
                    newRecord.Attributes.Remove(newRecord.LogicalName + "id");
                }

                try
                {
                    _dataverseService.MapSearchAttributes(newRecord, searchAttrs);

                    Guid createdRecordId = _dataverseService.CreateRecord(newRecord, false);
                    ++_resultItem.SuccessfullyGeneratedRecordCount;
                    _logger.Log($"Record is created with id {{{createdRecordId}}}");
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    if (ex.Detail.ErrorCode == ERROR_CODE) //DuplicateRecordsFound
                    {
                        _logger.Log("The record was not created because a duplicate of the current record already exists.");
                    }
                    else
                    {
                        _logger.Log(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(ex.Message);
                }
            }
        }
    }
}