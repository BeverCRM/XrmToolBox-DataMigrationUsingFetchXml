using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;
using DataMigrationUsingFetchXml.Forms.Popup;
using DataMigrationUsingFetchXml.Services.Interfaces;

namespace DataMigrationUsingFetchXml.Services.Implementations
{
    internal sealed class DataverseService : IDataverseService
    {
        private readonly IOrganizationService _sourceService;
        private readonly IOrganizationService _targetService;
        private readonly ILogger _logger;

        public DataverseService(IOrganizationService sourceService, IOrganizationService targetService, ILogger logger)
        {
            _sourceService = sourceService;
            _targetService = targetService;
            _logger = logger;
        }

        public DataverseService(IOrganizationService service)
        {
            _sourceService = service;
        }

        public IEnumerable<EntityCollection> GetAllRecords(string fetchQuery)
        {
            EntityCollection returnCollection;
            do
            {
                string xml = PaginationDetails.ContainsTopAttribute
                    ? fetchQuery
                    : ConfigReader.CreateXml(fetchQuery, PaginationDetails.PagingCookie);

                returnCollection = _sourceService.RetrieveMultiple(new FetchExpression(xml));

                ++PaginationDetails.PageNumber;
                PaginationDetails.PagingCookie = returnCollection.PagingCookie;

                yield return returnCollection;
            } while (returnCollection.MoreRecords);
        }

        public void CreateRecord(Entity record, Entity matchedTargetRecord, int index)
        {
            Guid recordId = new Guid();
            foreach (var item in record.Attributes.Values)
                if (Guid.TryParse(item.ToString(), out recordId))
                    break;

            if (matchedTargetRecord == null)
            {
                CreateRequest createRequest = new CreateRequest { Target = record };
                CreateResponse response = (CreateResponse)_targetService.Execute(createRequest);
                _logger.LogInfo($"Record is created with id {{{response.id}}}");
            }
            else if (MatchedAction.CheckedRadioButtonNumbers[index] == 1)
            {
                _targetService.Delete(matchedTargetRecord.LogicalName, matchedTargetRecord.Id);
                _logger.LogInfo($"Record is deleted with id {{{matchedTargetRecord.Id}}}");
                _targetService.Create(record);
                _logger.LogInfo($"Record is created with id {{{recordId}}}");
            }
            else if (MatchedAction.CheckedRadioButtonNumbers[index] == 2)
            {
                Entity newRecord = new Entity(record.LogicalName);
                newRecord.Attributes.AddRange(record.Attributes);
                newRecord[record.LogicalName + "id"] = matchedTargetRecord[record.LogicalName + "id"];
                _targetService.Update(newRecord);
                _logger.LogInfo($"Record is updated with id {{{recordId}}}");
            }
            else
            {
                CreateRequest createRequest = new CreateRequest { Target = record };
                createRequest.Parameters.Add("SuppressDuplicateDetection", false);
                CreateResponse response = (CreateResponse)_targetService.Execute(createRequest);
                _logger.LogInfo($"Record is created with id {{{response.id}}}");
            }
        }

        public void MapSearchAttributes(Entity record, List<string> searchAttrs)
        {
            foreach (string searchAttr in searchAttrs)
            {
                EntityReference refValue = record.GetAttributeValue<EntityReference>(searchAttr);

                if (refValue != null)
                {
                    string primaryField = GetEntityPrimaryField(refValue.LogicalName);
                    Entity refEntity = GetRecord(refValue.LogicalName, primaryField, refValue.Name);

                    if (refEntity != null)
                    {
                        record[searchAttr] = refEntity.ToEntityReference();
                    }
                    else
                    {
                        //_logger.LogError("Can't find the '" + refValue.LogicalName + "' entity record with name '" + refValue.Name);
                        _logger.LogInfo($"Creating a new record of '{refValue.LogicalName}' with name '{refValue.Name}'...");
                    }
                }
            }
        }

        public string GetEntityPrimaryField(string entitySchemaName)
        {
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entitySchemaName
            };
            var retrieveEntityResponse = (RetrieveEntityResponse)_targetService.Execute(retrieveEntityRequest);

            return retrieveEntityResponse.EntityMetadata.PrimaryNameAttribute;
        }

        public Entity GetRecord(string entitySchemaName, string attributeSchemaName, string attributeValue)
        {
            if (DateTime.TryParse(attributeValue, out DateTime date))
            {
                attributeValue = date.ToLocalTime().ToString();
            }

            QueryExpression query = new QueryExpression
            {
                EntityName = entitySchemaName,
                ColumnSet = new ColumnSet(attributeSchemaName),
                Criteria =
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, attributeValue)
                    }
                }
            };
            EntityCollection records = _targetService.RetrieveMultiple(query);

            return records.Entities.FirstOrDefault();
        }

        public (string logicalName, string displayName) GetEntityName(string fetchXml)
        {
            EntityCollection returnCollection = _sourceService.RetrieveMultiple(new FetchExpression(fetchXml));
            if (returnCollection.Entities.Count == 0)
            {
                throw new Exception($"Records not found: '{returnCollection.EntityName}'");
            }
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = returnCollection.Entities[0].LogicalName
            };
            RetrieveEntityResponse retrieveAccountEntityResponse = (RetrieveEntityResponse)_sourceService.Execute(retrieveEntityRequest);
            EntityMetadata AccountEntity = retrieveAccountEntityResponse.EntityMetadata;

            return (AccountEntity.LogicalName, AccountEntity.DisplayName.UserLocalizedLabel.Label);
        }
    }
}