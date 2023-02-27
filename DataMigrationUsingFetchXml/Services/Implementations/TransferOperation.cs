using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
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
        public int CurrentIndexForTransfer { get; set; }

        private EntityCollection _matchedTargetRecords;
        private string _organizationServiceUrl;
        private ResultItem _resultItem;

        private readonly ILogger _logger;
        private IDataverseService _dataverseService;

        public TransferOperation(ILogger logger)
        {
            _matchedTargetRecords = new EntityCollection();
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
            string newFetchXml;
            int index = 0;

            foreach (string fetchXml in fetchXmls)
            {
                newFetchXml = fetchXml;
                ConfigReader.CurrentFetchXml = newFetchXml;
                CurrentIndexForTransfer = tableIndexesForTransfer[index];
                List<string> attributeNames = ConfigReader.GetAttributesNames();

                if (attributeNames.Contains("statuscode") && !attributeNames.Contains("statecode"))
                {
                    newFetchXml = ConfigReader.CreateElementInFetchXml(newFetchXml, "statecode");
                }

                ConfigReader.CurrentFetchXml = newFetchXml;
                ConfigReader.SetPaginationAttributes();
                ConfigReader.FindAttributeNamesWithTargetAttributeInFetchXml();

                _resultItem = new ResultItem();
                List<string> searchAttrs = ConfigReader.GetPrimaryFields(out bool idExists);

                _logger.LogInfo($"Selected Matching Action: {MatchedAction.SelectedActionDescription[MatchedAction.CheckedRadioButtonNumbers[CurrentIndexForTransfer]]}");
                _logger.LogInfo("Getting data of '" + DisplayNames[CurrentIndexForTransfer] + "' from source instance");
                _logger.LogInfo("Transfering data to: " + _organizationServiceUrl);

                foreach (EntityCollection records in _dataverseService.GetAllRecords(newFetchXml))
                {
                    _resultItem.DisplayName = DisplayNames[CurrentIndexForTransfer];
                    _resultItem.SchemaName = records.EntityName;
                    _resultItem.SourceRecordCount += records.Entities.Count;
                    _resultItem.SourceRecordCountWithSign = _resultItem.SourceRecordCount.ToString();

                    if (records.MoreRecords)
                    {
                        _resultItem.SourceRecordCountWithSign += '+';
                    }
                    _logger.LogInfo("Records count is: " + _resultItem.SourceRecordCountWithSign);

                    if (records.Entities.Count == 0)
                    {
                        _logger.LogError("Records count is zero or not found");
                        break;
                    }

                    foreach (Entity record in records.Entities)
                    {
                        if (worker.CancellationPending)
                        {
                            ResultItems.Add(_resultItem);
                            args.Cancel = true;

                            return;
                        }
                        TransferData(record, searchAttrs, idExists, CurrentIndexForTransfer);
                        worker.ReportProgress(-1, _resultItem);
                    }
                }
                ResultItems.Add(_resultItem);
                index++;
            }
        }

        private void ChangeSourceRecordAttrNameIfThereIsTargetAttrName(Entity sourceRecord, List<string> attributeNames)
        {
            foreach (KeyValuePair<string, string> item in ConfigReader.SourceAndTargetAttributeNames)
            {
                if (attributeNames != null && attributeNames.Contains(item.Key))
                {
                    attributeNames.Remove(item.Key);
                    attributeNames.Add(item.Value);
                }

                if (sourceRecord.Attributes.ContainsKey(item.Key))
                {
                    object recordValue = sourceRecord.GetAttributeValue<object>(item.Key);
                    sourceRecord.Attributes.Remove(item.Key);
                    sourceRecord.Attributes.Add(item.Value, recordValue);
                }
            }
        }

        private bool CheckMatchingRecords(Entity record, int rowIndex, bool idExists)
        {
            List<string> logicalOperatorNames = MatchingCriteria.SelectedLogicalOperators[rowIndex];
            List<string> attributeNames = MatchingCriteria.SelectedAttributeNames[rowIndex];
            List<EntityCollection> allMatchedTargetRecords = new List<EntityCollection>();
            EntityCollection finalMatchcdRecords = new EntityCollection();
            EntityCollection matchcdRecords = new EntityCollection();

            ChangeSourceRecordAttrNameIfThereIsTargetAttrName(record, attributeNames);

            if (attributeNames.Count == 0 && !idExists)
            {
                return false;
            }

            if (attributeNames.Count == 1)
            {
                return CheckRecordInTargetByAttributeType(record, attributeNames[0]);
            }

            if (attributeNames.Count == 0 && idExists)
            {
                return CheckRecordInTargetByAttributeType(record, record.LogicalName + "id");
            }

            bool isThereMatchingRecordInTarget = logicalOperatorNames[0] == "And" || logicalOperatorNames[0] == "AND";

            for (int i = 0, j = 0; i < attributeNames.Count; i++, j = i - 1)
            {
                foreach (var item in _matchedTargetRecords.Entities)
                {
                    matchcdRecords.Entities.Add(item);
                }

                if ((logicalOperatorNames[j] == "And" || logicalOperatorNames[j] == "AND") && isThereMatchingRecordInTarget)
                {
                    isThereMatchingRecordInTarget = CheckRecordInTargetByAttributeType(record, attributeNames[i]);
                }
                else if (logicalOperatorNames[j] == "OR" || logicalOperatorNames[j] == "Or")
                {
                    if (_matchedTargetRecords.Entities.Count > 0)
                    {
                        allMatchedTargetRecords.Add(_matchedTargetRecords);
                    }
                    isThereMatchingRecordInTarget = CheckRecordInTargetByAttributeType(record, attributeNames[i]);
                }

                if (isThereMatchingRecordInTarget && matchcdRecords.Entities.Count > 0 && (logicalOperatorNames[j] == "And" || logicalOperatorNames[j] == "AND"))
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
                        isThereMatchingRecordInTarget = false;
                    }

                    finalMatchcdRecords.Entities.Clear();
                    matchcdRecords.Entities.Clear();
                }
            }
            FillMatchedTargetRecords(allMatchedTargetRecords);

            return isThereMatchingRecordInTarget;
        }

        private void FillMatchedTargetRecords(List<EntityCollection> allMatchedTargetRecords)
        {
            if (_matchedTargetRecords.Entities.Count == 0 && allMatchedTargetRecords.Count > 0)
            {
                _matchedTargetRecords = allMatchedTargetRecords[0];
            }

            foreach (var matchedTargetRecords in allMatchedTargetRecords)
            {
                foreach (var matchedTargetRecord in matchedTargetRecords.Entities)
                {
                    Entity entity = _matchedTargetRecords.Entities.Where(x => x.Id == matchedTargetRecord.Id).FirstOrDefault();

                    if (entity == null)
                    {
                        _matchedTargetRecords.Entities.Add(matchedTargetRecord);
                    }
                }
            }
        }

        private bool CheckRecordInTargetByAttributeType(Entity record, string attributeName)
        {
            if (!record.Attributes.Contains(attributeName))
            {
                return false;
            }

            string attributeType = _dataverseService.GetAttributeType(attributeName, record.LogicalName);
            _matchedTargetRecords = _dataverseService.GetTargetMatchedRecords(record, attributeName, attributeType);

            if (_matchedTargetRecords.Entities.Count == 0)
            {
                return false;
            }

            return true;
        }

        private void TransferData(Entity record, List<string> searchAttrs, bool idExists, int rowIndex)
        {
            bool checkMatchingRecords = false;
            _matchedTargetRecords.Entities.Clear();

            if (MatchedAction.CheckedRadioButtonNumbers[rowIndex] != (byte)MatchedActionForRecord.Create)
            {
                try
                {
                    checkMatchingRecords = CheckMatchingRecords(record, rowIndex, idExists);
                }
                catch (Exception ex)
                {
                    ++_resultItem.ErroredRecordCount;
                    _logger.LogError($"Cannot transfer record with id {{{record.GetAttributeValue<Guid>(record.LogicalName + "id")}}}.Error Message: {ex.Message}");
                    return;
                }
            }
            else
            {
                ChangeSourceRecordAttrNameIfThereIsTargetAttrName(record, null);
            }

            if (checkMatchingRecords && MatchedAction.CheckedRadioButtonNumbers[rowIndex] == (byte)MatchedActionForRecord.DoNotCreate)
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
                    _dataverseService.CreateMatchedRecordInTarget(newRecord, _matchedTargetRecords, _resultItem, MatchedAction.CheckedRadioButtonNumbers[rowIndex]);
                }
                catch (Exception ex)
                {
                    ++_resultItem.ErroredRecordCount;
                    _logger.LogError(ex.Message);
                }
            }
        }
    }
}