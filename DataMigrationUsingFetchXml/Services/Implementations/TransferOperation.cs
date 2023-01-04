using System;
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

        private const int DUPPLICATE_RECORDS_FOUND_ERROR_CODE = -2_147_220_685;

        private Entity _matchedTargetRecord;

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
                index++;
            }
        }

        private bool CheckMatchingRecords(Entity record, int index, bool idExists, List<string> searchAttrs)
        {
            List<string> attributeNames = MatchingCriteria.finalAttributeNamesResult[index];
            List<string> logicalOperatorNames = MatchingCriteria.finalLogicalOperatorsResult[index];

            if (attributeNames.Count == 0 && !idExists)
            {
                return false;
            }
            if ((attributeNames.Count == 0 || attributeNames.Count == 1) && idExists)
            {
                _matchedTargetRecord = _dataverseService.GetRecord(record.LogicalName, record.LogicalName + "id", record.Id.ToString());
                return _matchedTargetRecord != null;
            }
            else if (attributeNames.Count > 0)
            {
                if (!logicalOperatorNames.Contains("OR"))
                {
                    for (int i = 0; i < attributeNames.Count; i++)
                    {
                        if (!CheckRecordInTarget(record, attributeNames[i], searchAttrs))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else if (!logicalOperatorNames.Contains("And"))
                {
                    for (int i = 0; i < attributeNames.Count; i++)
                    {
                        if (CheckRecordInTarget(record, attributeNames[i], searchAttrs))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    if (logicalOperatorNames[0] == "And")
                    {
                        bool checkAnd = true;
                        for (int i = 0, j = 0; i < attributeNames.Count; i++, j = i - 1)
                        {
                            if (logicalOperatorNames[j] == "And" && checkAnd)
                            {
                                checkAnd = CheckRecordInTarget(record, attributeNames[i], searchAttrs);
                            }
                            else if (logicalOperatorNames[j] == "OR")
                            {
                                if (checkAnd)
                                {
                                    return true;
                                }
                                else
                                {
                                    checkAnd = CheckRecordInTarget(record, attributeNames[i], searchAttrs);
                                }
                            }
                        }
                        return checkAnd;
                    }
                    else
                    {
                        bool checkOr = false;
                        for (int i = 0, j = 0; i < attributeNames.Count; i++, j = i - 1)
                        {
                            if (logicalOperatorNames[j] == "OR" && !checkOr)
                            {
                                checkOr = CheckRecordInTarget(record, attributeNames[i], searchAttrs);
                            }
                            else if (logicalOperatorNames[j] == "And")
                            {
                                if (!checkOr)
                                {
                                    return false;
                                }
                                else
                                {
                                    checkOr = CheckRecordInTarget(record, attributeNames[i], searchAttrs);
                                }
                            }
                        }
                        return checkOr;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        private bool CheckRecordInTarget(Entity record, string attributeName, List<string> searchAttrs)
        {
            try
            {
                _matchedTargetRecord = _dataverseService.GetRecord(record.LogicalName, attributeName, record[attributeName].ToString());
                return _matchedTargetRecord != null;
            }
            catch (Exception)
            {
                try//for lookup
                {
                    if (!searchAttrs.Contains(attributeName))
                    {
                        throw new Exception();
                    }
                    EntityReference refValue = record.GetAttributeValue<EntityReference>(attributeName);
                    if (refValue == null)
                    {
                        return false;
                    }
                    _matchedTargetRecord = _dataverseService.GetRecord(record.LogicalName, attributeName, refValue.Id.ToString());
                    return _matchedTargetRecord != null;
                }
                catch (Exception)//for optionSet
                {
                    if (record.Attributes.Contains(attributeName))
                    {
                        int value = ((OptionSetValue)record[attributeName]).Value;
                        _matchedTargetRecord = _dataverseService.GetRecord(record.LogicalName, attributeName, value.ToString());
                        return _matchedTargetRecord != null;
                    }
                    return false;
                }
            }
        }

        private void TransferData(Entity record, List<string> searchAttrs, bool idExists, int index)
        {
            string primaryAttr = _dataverseService.GetEntityPrimaryField(record.LogicalName);
            string recordName = record.GetAttributeValue<string>(primaryAttr);
            bool checkMatchingRecords = CheckMatchingRecords(record, index, idExists, searchAttrs);

            if (!checkMatchingRecords)
            {
                _matchedTargetRecord = null;
            }
            if (checkMatchingRecords && MatchedAction.CheckedRadioButtonNumbers[index] == 4)
            {
                ++_resultItem.ErroredRecordCount;
                _logger.LogError("Can't create matched record.");
            }
            else
            {
                _logger.LogInfo("Record with id '" + record.Id + "' and with name '" + recordName + "' is creating in target...");
                Entity newRecord = new Entity(record.LogicalName);
                newRecord.Attributes.AddRange(record.Attributes);

                //if (!idExists)
                //{
                //    newRecord.Attributes.Remove(newRecord.LogicalName + "id");
                //}
                try
                {
                    _dataverseService.MapSearchAttributes(newRecord, searchAttrs);
                    _dataverseService.CreateRecord(newRecord, _matchedTargetRecord, index);
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
            //else//recordy match chi exel
            //{
            //    ++_resultItem.ErroredRecordCount;
            //    _logger.LogError("This record has not been matched.");
            //}
        }
    }
}