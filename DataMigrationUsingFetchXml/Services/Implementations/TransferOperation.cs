using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.ComponentModel;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;
using DataMigrationUsingFetchXml.Forms.Popup;
using DataMigrationUsingFetchXml.Services.Interfaces;

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

        private EntityCollection _matchedTargetRecords;

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
            string newFetchXml;

            foreach (string fetchXml in fetchXmls)
            {
                newFetchXml = fetchXml;
                ConfigReader.CurrentFetchXml = newFetchXml;
                List<string> attributeNames = ConfigReader.GetAttributesNames();

                if (attributeNames.Contains("statuscode") && !attributeNames.Contains("statecode"))
                {
                    newFetchXml = ConfigReader.CreateElementInFetchXml(newFetchXml, "statecode");
                }
                ConfigReader.CurrentFetchXml = newFetchXml;
                ConfigReader.SetPaginationAttributes();
                _resultItem = new ResultItem();
                List<string> searchAttrs = ConfigReader.GetPrimaryFields(out bool idExists);

                _logger.LogInfo($"Selected Matching Action: {MatchedAction.SelectedActionDescription[MatchedAction.CheckedRadioButtonNumbers[tableIndexesForTransfer[index]]]}");
                _logger.LogInfo("Getting data of '" + DisplayNames[tableIndexesForTransfer[index]] + "' from source instance");
                _logger.LogInfo("Transfering data to: " + _organizationServiceUrl);

                foreach (EntityCollection records in _dataverseService.GetAllRecords(newFetchXml))
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
                            TransferData(record, searchAttrs, idExists, tableIndexesForTransfer[index]);
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
                index++;
            }
        }

        private bool CheckMatchingRecords(Entity record, int rowIndex, bool idExists)
        {
            List<string> attributeNames = MatchingCriteria.SelectedAttributeNames[rowIndex];
            List<string> logicalOperatorNames = MatchingCriteria.SelectedLogicalOperators[rowIndex];
            EntityCollection finalMatchcdRecords = new EntityCollection();
            EntityCollection matchcdRecords = new EntityCollection();

            if (attributeNames.Count == 0 && !idExists)
            {
                return false;
            }

            if ((attributeNames.Count == 0 || attributeNames.Count == 1) && idExists)
            {
                return CheckRecordInTargetByAttributeType(record, record.LogicalName + "id");
            }
            else if (attributeNames.Count == 1)
            {
                return CheckRecordInTargetByAttributeType(record, attributeNames[0]);
            }
            else if (attributeNames.Count > 0)
            {
                bool isThereMatchingRecordInTarget = false;
                if (logicalOperatorNames[0] == "And")
                {
                    isThereMatchingRecordInTarget = true;
                }

                for (int i = 0, j = 0; i < attributeNames.Count; i++, j = i - 1)
                {
                    if (logicalOperatorNames[j] == "And" && isThereMatchingRecordInTarget)
                    {
                        if (_matchedTargetRecords != null)
                        {
                            foreach (var item in _matchedTargetRecords.Entities)
                            {
                                matchcdRecords.Entities.Add(item);
                            }
                        }
                        isThereMatchingRecordInTarget = CheckRecordInTargetByAttributeType(record, attributeNames[i]);

                        if (isThereMatchingRecordInTarget && matchcdRecords.Entities.Count > 0)
                        {
                            foreach (var item in _matchedTargetRecords.Entities)
                            {
                                Entity entity = matchcdRecords.Entities.Where(x => x.Id == item.Id).FirstOrDefault();
                                if (entity != null)
                                {
                                    finalMatchcdRecords.Entities.Add(entity);
                                }
                            }
                            _matchedTargetRecords.Entities.Clear();

                            foreach (var item in finalMatchcdRecords.Entities)
                            {
                                _matchedTargetRecords.Entities.Add(item);
                            }
                            if (_matchedTargetRecords.Entities.Count == 0)
                            {
                                _matchedTargetRecords = null;
                                isThereMatchingRecordInTarget = false;
                            }
                            finalMatchcdRecords.Entities.Clear();
                            matchcdRecords.Entities.Clear();
                        }
                    }
                    else if (logicalOperatorNames[j] == "OR")
                    {
                        if (isThereMatchingRecordInTarget)
                        {
                            return true;
                        }
                        else
                        {
                            isThereMatchingRecordInTarget = CheckRecordInTargetByAttributeType(record, attributeNames[i]);
                        }
                    }
                }
                return isThereMatchingRecordInTarget;
            }
            else
            {
                return false;
            }
        }

        private bool CheckRecordInTargetByAttributeType(Entity record, string attributeName)
        {
            if (record.Attributes.Contains(attributeName))
            {
                string attributeType = _dataverseService.GetAttributeType(attributeName, record.LogicalName);
                _matchedTargetRecords = _dataverseService.GetTargetMatchedRecords(record, attributeName, attributeType);

                if (_matchedTargetRecords == null || _matchedTargetRecords.Entities.Count == 0)
                {
                    _matchedTargetRecords = null;
                    return false;
                }

                return true;
            }

            return false;
        }

        private void TransferData(Entity record, List<string> searchAttrs, bool idExists, int rowIndex)
        {
            bool checkMatchingRecords = false;
            if (MatchedAction.CheckedRadioButtonNumbers[rowIndex] != 1)
            {
                checkMatchingRecords = CheckMatchingRecords(record, rowIndex, idExists);
            }

            if (checkMatchingRecords && MatchedAction.CheckedRadioButtonNumbers[rowIndex] == 4)
            {
                ++_resultItem.SkippedRecordCount;
                _logger.LogInfo($"Skipped the record with Id {{{record.GetAttributeValue<Guid>(record.LogicalName + "id")}}} as it already exists in the target instance.");
            }
            else
            {
                Entity newRecord = new Entity(record.LogicalName);
                newRecord.Attributes.AddRange(record.Attributes);

                try
                {
                    _dataverseService.MapSearchAttributes(newRecord, searchAttrs);
                    if (_matchedTargetRecords != null)
                    {
                        _dataverseService.CreateMatchedRecordInTarget(newRecord, _matchedTargetRecords, _resultItem, rowIndex);
                    }
                    else
                    {
                        _dataverseService.CreateMatchedRecordInTarget(newRecord, null, _resultItem, rowIndex);
                    }
                }
                catch (Exception ex)
                {
                    ++_resultItem.ErroredRecordCount;
                    _logger.LogError(ex.Message);
                }
            }
            _matchedTargetRecords = null;
        }
    }
}