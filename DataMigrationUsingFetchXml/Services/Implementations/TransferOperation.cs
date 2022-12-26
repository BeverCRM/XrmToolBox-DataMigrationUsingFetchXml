using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.ComponentModel;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;
using DataMigrationUsingFetchXml.Services.Interfaces;
using DataMigrationUsingFetchXml.Forms.Popup;

namespace DataMigrationUsingFetchXml.Services.Implementations
{
    internal sealed class TransferOperation : ITransferOperation
    {
        public List<ResultItem> ResultItems { get; set; }
        public List<string> DisplayNames { get; set; }

        private string _organizationServiceUrl;

        private ResultItem _resultItem;

        private readonly ILogger _logger;

        private IDataverseService _dataverseService;

        private const int DUPPLICATE_RECORDS_FOUND_ERROR_CODE = -2_147_220_685;

        public TransferOperation(ILogger logger)
        {
            _logger = logger;
        }

        public void SetConnectionDetails(ConnectionDetails connectionDetails)
        {
            _organizationServiceUrl = connectionDetails.AdditionalConnectionDetails[0].OrganizationDataServiceUrl;
            _dataverseService = new DataverseService(connectionDetails.Service, connectionDetails.AdditionalConnectionDetails[0].ServiceClient, _logger);
        }

        public void Transfer(List<string> fetchXmls, List<int> tableIndexesForTransfer, BackgroundWorker worker, DoWorkEventArgs args)
        {
            ResultItems = new List<ResultItem>();
            int index = 0;

            foreach (string fetchXml in fetchXmls)
            {
                string primaryKeyName = ConfigReader.GetFetchXmlPrimaryKey(fetchXml);
                if (primaryKeyName == null && MatchedAction.CheckedRadioButtonNumbers[index] == 1 || MatchedAction.CheckedRadioButtonNumbers[index] == 2)
                {
                    _logger.LogError("FetchXML must have primary key for deleting or updating.");
                }
                else
                {
                    ConfigReader.SetPaginationAttributes(fetchXml);
                    _resultItem = new ResultItem();
                    List<string> searchAttrs = ConfigReader.GetPrimaryFields(fetchXml, out bool idExists);

                    _logger.LogInfo("Getting data of '" + DisplayNames[tableIndexesForTransfer[index]] + "' from source instance");
                    _logger.LogInfo("Transfering data to: " + _organizationServiceUrl);

                    foreach (EntityCollection records in _dataverseService.GetAllRecords(fetchXml))
                    {
                        _resultItem.DisplayName = DisplayNames[tableIndexesForTransfer[index]];
                        _resultItem.SchemaName = records.EntityName;
                        _resultItem.SourceRecordCount += records.Entities.Count;
                        _resultItem.SourceRecordCountWithSign = _resultItem.SourceRecordCount.ToString();

                        if (records.MoreRecords)
                        {
                            _resultItem.SourceRecordCountWithSign += '+';
                        }
                        _logger.LogInfo("Records count is: " + _resultItem.SourceRecordCountWithSign);

                        if (records.Entities.Count > 0)
                        {
                            foreach (Entity record in records.Entities)
                            {
                                if (worker.CancellationPending)
                                {
                                    ResultItems.Add(_resultItem);
                                    args.Cancel = true;

                                    return;
                                }

                                TransferData(record, searchAttrs, idExists, index);
                                worker.ReportProgress(-1, _resultItem);
                            }
                        }
                        else
                        {
                            _logger.LogError("Records count is zero or not found");
                        }

                        if (!records.MoreRecords)
                        {
                            ResultItems.Add(_resultItem);
                        }
                    }
                }
                index++;
            }
        }

        private void TransferData(Entity record, List<string> searchAttrs, bool idExists, int index)
        {
            string primaryAttr = _dataverseService.GetEntityPrimaryField(record.LogicalName);
            string recordName = record.GetAttributeValue<string>(primaryAttr);
            string recordId = record.Id.ToString();

            _logger.LogInfo("Record with id '" + recordId + "' and with name '" + recordName + "' is creating in target...");
            Entity newRecord = new Entity(record.LogicalName);
            newRecord.Attributes.AddRange(record.Attributes);

            if (!idExists)
            {
                newRecord.Attributes.Remove(newRecord.LogicalName + "id");
            }
            try
            {
                _dataverseService.MapSearchAttributes(newRecord, searchAttrs);
                _dataverseService.CreateRecord(newRecord, index);
                ++_resultItem.SuccessfullyGeneratedRecordCount;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                ++_resultItem.ErroredRecordCount;
                if (ex.Detail.ErrorCode == DUPPLICATE_RECORDS_FOUND_ERROR_CODE)
                {
                    _logger.LogError("The record was not created because a duplicate of the current record already exists.");
                }
                else
                {
                    _logger.LogError(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}