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
                    : ConfigReader.CreatePaginationAttributes(fetchQuery, PaginationDetails.PagingCookie);

                returnCollection = _sourceService.RetrieveMultiple(new FetchExpression(xml));

                ++PaginationDetails.PageNumber;
                PaginationDetails.PagingCookie = returnCollection.PagingCookie;

                yield return returnCollection;
            } while (returnCollection.MoreRecords);
        }

        private void SetRecordTransactionCurrency(Entity record)
        {
            if (record.Attributes.ContainsKey("transactioncurrencyid"))
            {
                string currencyName = (record["transactioncurrencyid"] as EntityReference).Name;

                QueryExpression query = new QueryExpression
                {
                    EntityName = "transactioncurrency",
                    ColumnSet = new ColumnSet(true)
                };
                EntityCollection transactioncurrencies = _targetService.RetrieveMultiple(query);

                foreach (var item in transactioncurrencies.Entities)
                {
                    if ((item.Attributes["currencyname"] as string) == currencyName)
                    {
                        (record["transactioncurrencyid"] as EntityReference).Id = item.Id;
                        return;
                    }
                }
                record["transactioncurrencyid"] = transactioncurrencies.Entities.FirstOrDefault().ToEntityReference();
            }
        }

        public void CreateMatchedRecordInTarget(Entity record, Entity matchedTargetRecord, int index)
        {
            SetRecordTransactionCurrency(record);
            string recordName;
            string recordId = record[record.LogicalName + "id"].ToString();

            if (MatchedAction.CheckedRadioButtonNumbers[index] == 3 || matchedTargetRecord == null)
            {
                recordName = GetRecordName(record);
            }
            else
            {
                recordName = GetRecordName(matchedTargetRecord);
            }

            if (matchedTargetRecord == null)
            {
                _logger.LogInfo("Record with id '" + recordId + "' and with name '" + recordName + "' is creating in target...");
                CreateRequest createRequest = new CreateRequest { Target = record };
                CreateResponse response = (CreateResponse)_targetService.Execute(createRequest);
                _logger.LogInfo($"Record is created with id {{{response.id}}}");

                return;
            }

            if (MatchedAction.CheckedRadioButtonNumbers[index] == 1)
            {
                _targetService.Delete(matchedTargetRecord.LogicalName, matchedTargetRecord.Id);
                _logger.LogInfo($"Record is deleted with id {{{matchedTargetRecord.Id}}}");
                _targetService.Create(record);
                _logger.LogInfo($"Record is created with id {{{recordId}}}");
            }
            else if (MatchedAction.CheckedRadioButtonNumbers[index] == 2)
            {
                _logger.LogInfo("Record with id '" + matchedTargetRecord.Id + "' and with name '" + recordName + "' is updating in target...");
                Entity newRecord = new Entity(record.LogicalName);
                newRecord.Attributes.AddRange(record.Attributes);
                newRecord[record.LogicalName + "id"] = matchedTargetRecord[record.LogicalName + "id"];
                _targetService.Update(newRecord);
                _logger.LogInfo($"Record is updated with id {{{matchedTargetRecord.Id}}}");
            }
            else
            {
                _logger.LogInfo("Record with id '" + recordId + "' and with name '" + recordName + "' is creating in target...");
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
                        _logger.LogError("Can't find the '" + refValue.LogicalName + "' entity record with name '" + refValue.Name);
                    }
                }
            }
        }

        private string GetRecordName(Entity record)
        {
            string primaryAttr = GetEntityPrimaryField(record.LogicalName);

            return record.GetAttributeValue<string>(primaryAttr);
        }

        private string GetEntityPrimaryField(string entitySchemaName)
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

            return _targetService.RetrieveMultiple(query)?.Entities.FirstOrDefault();
        }

        public EntityCollection GetTargetMatchedRecords(Entity sourceRecord, string attributeSchemaName, string attributeType)
        {
            FilterExpression filterExpression = new FilterExpression();

            if (attributeType == "Double" || attributeType == "Integer" || attributeType == "String" || attributeType == "BigInt"
                || attributeType == "Boolean" || attributeType == "EntityName" || attributeType == "Decimal" || attributeType == "Uniqueidentifier")
            {
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, sourceRecord[attributeSchemaName].ToString()));
            }
            else if (attributeType == "DateTime" && DateTime.TryParse(sourceRecord[attributeSchemaName].ToString(), out DateTime date))
            {
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, date.ToLocalTime().ToString()));
            }
            else if (attributeType == "Lookup" || attributeType == "Customer" || attributeType == "Owner")
            {
                EntityReference refValue = sourceRecord.GetAttributeValue<EntityReference>(attributeSchemaName);
                if (refValue == null)
                {
                    return null;
                }
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, refValue.Id.ToString()));
            }
            else if (attributeType == "Virtual")
            {
                OptionSetValueCollection optionSets = (OptionSetValueCollection)sourceRecord[attributeSchemaName];
                int[] osValues = new int[] { };

                foreach (var optionSet in optionSets)
                {
                    osValues = osValues.Concat(new int[] { optionSet.Value }).ToArray();
                }
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.In, osValues));
            }
            else if (attributeType == "Money")
            {
                Money money = (Money)sourceRecord[attributeSchemaName];

                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, money.Value.ToString()));
            }
            else if (attributeType == "Picklist" || attributeType == "Status" || attributeType == "State")
            {
                int value = ((OptionSetValue)sourceRecord[attributeSchemaName]).Value;
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, value));
            }
            else
            {
                return null;
            }
            QueryExpression query = new QueryExpression();
            query.EntityName = sourceRecord.LogicalName;
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddFilter(filterExpression);

            return _targetService.RetrieveMultiple(query);
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

        public string GetAttributeType(string logicalName, string entityLogicalName)
        {
            try
            {
                var attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityLogicalName,
                    LogicalName = logicalName,
                    RetrieveAsIfPublished = true
                };
                var attributeResponse = (RetrieveAttributeResponse)_sourceService.Execute(attributeRequest);

                return attributeResponse.AttributeMetadata.AttributeType.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}