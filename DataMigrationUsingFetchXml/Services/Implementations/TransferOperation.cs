using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DataMigrationUsingFetchXml.Model;
using DataMigrationUsingFetchXml.Services.Interfaces;

namespace DataMigrationUsingFetchXml.Services.Implementations
{
    internal sealed class TransferOperation : ITransferOperation
    {
        public List<ResultItem> ResultItems { get; set; }
        public List<string> DisplayNames { get; set; }
        public bool KeepRunning { get; set; } = true;

        private ResultItem _resultItem;

        private readonly ILogger _logger;

        private const int ERROR_CODE = -2147220685;

        private string _organizationDataServiceUrl;
        
        private IOrganizationService _sourceService;

        private IDataverseService _dataverseService;

        private System.Windows.Forms.Label _lblInfo;
        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.Label _lblError;

        private ObservableCollection<ConnectionDetail> _additionalConnectionDetails;

        public TransferOperation(ILogger logger)
        {
            _logger = logger;
        }

        public void SetConnectionDetails(ConnectionDetails connectionDetails)
        {
            _sourceService = connectionDetails.Service;
            _additionalConnectionDetails = connectionDetails.AdditionalConnectionDetails;
            _organizationDataServiceUrl = _additionalConnectionDetails[0].OrganizationDataServiceUrl;
            _dataverseService = new DataverseService(_sourceService, _additionalConnectionDetails[0].ServiceClient, _logger);
        }

        public void SetLabel(System.Windows.Forms.Label lblInfo, System.Windows.Forms.Label lblTitle, System.Windows.Forms.Label lblError)
        {
            _lblInfo = lblInfo;
            _lblTitle = lblTitle;
            _lblError = lblError;
        }

        public void Transfer(List<string> fetchXmls, List<int> tableIndexesForTransfer, RichTextBox richTextBoxLogs)
        {
            ResultItems = new List<ResultItem>();
            int index = 0;

            foreach (string fetchXml in fetchXmls)
            {
                _resultItem = new ResultItem();
                _lblTitle.Text = $"Migrating {DisplayNames[tableIndexesForTransfer[index]]} records";

                List<string> searchAttrs = ConfigReader.GetPrimaryFields(fetchXml, out bool idExists);
                EntityCollection records = _dataverseService.GetAllRecords(fetchXml);

                _logger.LogInfo("Getting data of '" + records.Entities[0].LogicalName + "' from source instance");
                _resultItem.EntityName = records.Entities[0].LogicalName;
                _logger.LogInfo("Transfering data to: " + _organizationDataServiceUrl);
                do
                {
                    _resultItem.SourceRecordCount += records.Entities.Count;
                    _logger.LogInfo("Records count is: " + records.Entities.Count);

                    if (records?.Entities?.Count > 0)
                    {
                        foreach (Entity record in records.Entities)
                        {
                            TransferData(record, searchAttrs, idExists);
                            _lblInfo.Text = $"{_resultItem.SuccessfullyGeneratedRecordCount} of {_resultItem.SourceRecordCount} is imported";
                            if (_resultItem.ErroredRecordCount > 0)
                            {
                                _lblError.Text = $"{_resultItem.ErroredRecordCount} of {_resultItem.SourceRecordCount} is errored";
                            }
                            if (!KeepRunning)
                            {
                                _lblInfo.Text = $"{_resultItem.SuccessfullyGeneratedRecordCount} of {_resultItem.SourceRecordCount} {DisplayNames[tableIndexesForTransfer[index]]} is imported";
                                if (_resultItem.ErroredRecordCount > 0)
                                {
                                    _lblError.Text = $"{_resultItem.ErroredRecordCount} of {_resultItem.SourceRecordCount} {DisplayNames[tableIndexesForTransfer[index]]} is errored";
                                }
                                ResultItems.Add(_resultItem);
                                return;
                            }
                            richTextBoxLogs.SelectionStart = richTextBoxLogs.Text.Length;
                            richTextBoxLogs.ScrollToCaret();
                        }
                    }
                    else
                    {
                        _logger.LogError("Records count is zero or not found");
                    }

                    if (_resultItem == null)
                    {
                        _logger.LogError("Process Stopped. Aborting! ");
                        break;
                    }

                    if (!records.MoreRecords)
                    {
                        ResultItems.Add(_resultItem);
                        break;
                    }
                    records = _dataverseService.GetAllRecords(fetchXml);
                } while (true);

                if (fetchXmls.Count == index + 1)
                {
                    if (_resultItem.ErroredRecordCount > 0)
                    {
                        _lblError.Text = $"{_resultItem.ErroredRecordCount} of {_resultItem.SourceRecordCount} {DisplayNames[tableIndexesForTransfer[index]]} is errored";
                    }
                    _lblInfo.Text = $"{_resultItem.SuccessfullyGeneratedRecordCount} of {_resultItem.SourceRecordCount} {DisplayNames[tableIndexesForTransfer[index]]} is imported";
                }
                index++;
            }
        }

        private void TransferData(Entity record, List<string> searchAttrs, bool idExists)
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
                Guid createdRecordId = _dataverseService.CreateRecord(newRecord, false);
                ++_resultItem.SuccessfullyGeneratedRecordCount;
                _logger.LogInfo($"Record is created with id {{{createdRecordId}}}");
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                ++_resultItem.ErroredRecordCount;
                if (ex.Detail.ErrorCode == ERROR_CODE) //DuplicateRecordsFound
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