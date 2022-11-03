using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Windows.Forms;
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

        private ResultItem _resultItem;

        private readonly ILogger _logger;

        private List<string> _displayNames;

        private string _organizationDataServiceUrl;

        private const int ERROR_CODE = -2147220685;

        private int errorCount;

        private ObservableCollection<ConnectionDetail> _additionalConnectionDetails;

        private System.Windows.Forms.Label _lblInfo;
        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.Label _lblError;
        public bool KeepRunning { get; set; } = true;

        public TransferOperation(ILogger logger)
        {
            _logger = logger;
        }

        public void InitialiseFields(ConnectionDetails connectionDetails, System.Windows.Forms.Label lblInfo, System.Windows.Forms.Label lblTitle,
            System.Windows.Forms.Label lblError, List<string> displayNames)
        {
            _sourceService = connectionDetails.Service;
            _additionalConnectionDetails = connectionDetails.AdditionalConnectionDetails;
            _organizationDataServiceUrl = _additionalConnectionDetails[0].OrganizationDataServiceUrl;
            _dataverseService = new DataverseService(_sourceService, _additionalConnectionDetails[0].ServiceClient, _logger);
            _lblInfo = lblInfo;
            _lblTitle = lblTitle;
            _lblError = lblError;
            _displayNames = displayNames;
        }

        public void Transfer(List<string> fetchXmls, List<int> tableIndexesForTransfer, RichTextBox richTextBoxLogs)
        {
            ResultItems = new List<ResultItem>();
            int index = -1;

            foreach (string fetchXml in fetchXmls)
            {
                _resultItem = new ResultItem();
                _lblTitle.Text = $"Migrating {_displayNames[tableIndexesForTransfer[++index]]} records";

                List<string> searchAttrs = ConfigReader.GetPrimaryFields(fetchXml, out bool idExists);
                EntityCollection records = _dataverseService.GetAllRecords(fetchXml);

                _logger.Log("Getting data of '" + records.Entities[0].LogicalName + "' from source instance");
                _resultItem.SourceRecordCount = records.Entities.Count;
                _resultItem.EntityName = records.Entities[0].LogicalName;
                _logger.Log("Records count is: " + records.Entities.Count);

                if (records?.Entities?.Count > 0)
                {
                    _logger.Log("Transfering data to: " + _organizationDataServiceUrl);
                    foreach (Entity record in records.Entities)
                    {
                        if (!KeepRunning)
                        {
                            break;
                        }
                        TransferData(record, searchAttrs, idExists);
                        _lblInfo.Text = $"{_resultItem.SuccessfullyGeneratedRecordCount} of {records.Entities.Count} is imported";
                        if (errorCount > 0)
                        {
                            _lblError.Text = $"{errorCount} of {records.Entities.Count} is errored";
                        }
                        richTextBoxLogs.ScrollToCaret();
                    }
                    if (fetchXmls.Count == index + 1 || !KeepRunning)
                    {
                        _lblInfo.Text = $"{_resultItem.SuccessfullyGeneratedRecordCount} of {records.Entities.Count} {_displayNames[tableIndexesForTransfer[index]]} is imported";
                        if (errorCount > 0)
                        {
                            _resultItem.ErroredRecordsCount = errorCount;
                            _lblError.Text = $"{errorCount} of {records.Entities.Count} {_displayNames[tableIndexesForTransfer[index]]} is errored";
                        }
                    }
                    errorCount = 0;
                    if (!KeepRunning)
                    {
                        ResultItems.Add(_resultItem);
                        break;
                    }
                }
                else
                {
                    _logger.Log("Records count is zero or not found", true);
                }
                if (_resultItem == null)
                {
                    _logger.Log("Process Stopped. Aborting! ", true);
                    break;
                }
                ResultItems.Add(_resultItem);
            }
        }

        private void TransferData(Entity record, List<string> searchAttrs, bool idExists)
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
                errorCount++;
                if (ex.Detail.ErrorCode == ERROR_CODE) //DuplicateRecordsFound
                {
                    _logger.Log("The record was not created because a duplicate of the current record already exists.", true);
                }
                else
                {
                    _logger.Log(ex.Message, true);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, true);
            }
        }
    }
}