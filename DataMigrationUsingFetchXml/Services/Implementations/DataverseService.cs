using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
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

        public IEnumerable<EntityCollection> GetAllRecords(string fetchXml)
        {
            EntityCollection returnCollection;
            do
            {
                string xml = PaginationDetails.ContainsTopAttribute
                    ? fetchXml
                    : ConfigReader.CreatePaginationAttributes(PaginationDetails.PagingCookie);

                returnCollection = _sourceService.RetrieveMultiple(new FetchExpression(xml));

                ++PaginationDetails.PageNumber;
                PaginationDetails.PagingCookie = returnCollection.PagingCookie;

                yield return returnCollection;

            } while (returnCollection.MoreRecords);
        }

        private EntityReference GetDefaultTransactionCurrency()
        {
            var userSettingsQuery = new QueryExpression("usersettings")
            {
                ColumnSet = new ColumnSet("transactioncurrencyid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
                    }
                }
            };
            var currentUserSettings = _targetService.RetrieveMultiple(userSettingsQuery).Entities[0].ToEntity<Entity>();

            if (currentUserSettings.Attributes.ContainsKey("transactioncurrencyid"))
            {
                return currentUserSettings.GetAttributeValue<EntityReference>("transactioncurrencyid");
            }

            return null;
        }

        private void SetRecordTransactionCurrency(Entity sourceRecord)
        {
            if (sourceRecord.Attributes.ContainsKey("transactioncurrencyid"))
            {
                EntityReference defaultCurrency = GetDefaultTransactionCurrency();

                if (!ConfigReader.CurrentFetchXml.Contains("transactioncurrencyid"))
                {
                    sourceRecord["transactioncurrencyid"] = defaultCurrency ?? throw new Exception("Can not find default transaction currency.");
                    return;
                }

                string sourceRecordCurrencyName = sourceRecord.GetAttributeValue<EntityReference>("transactioncurrencyid").Name;
                EntityReference transactionCurrency = null;

                if (sourceRecordCurrencyName != null)
                {
                    QueryExpression query = new QueryExpression("transactioncurrency")
                    {
                        ColumnSet = new ColumnSet(),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                            {
                                new ConditionExpression("currencyname", ConditionOperator.Equal, sourceRecordCurrencyName)
                            }
                        }
                    };
                    EntityCollection transactionCurrencyCollection = _targetService.RetrieveMultiple(query);

                    if (transactionCurrencyCollection != null && transactionCurrencyCollection.Entities.Count > 0)
                    {
                        transactionCurrency = transactionCurrencyCollection.Entities.FirstOrDefault().ToEntityReference();
                    }
                }

                if (transactionCurrency != null)
                {
                    sourceRecord.GetAttributeValue<EntityReference>("transactioncurrencyid").Id = transactionCurrency.Id;
                }
            }
        }

        private void SetSourceRecordUnfilledFieldsDefaultValue(Entity sourceRecord, Entity matchedTargetRecord)
        {
            List<string> fetchXmlAttributeNames = ConfigReader.GetAttributesNames();

            foreach (var item in matchedTargetRecord.Attributes)
            {
                if (fetchXmlAttributeNames.Contains(item.Key) && !sourceRecord.Attributes.ContainsKey(item.Key))
                {
                    sourceRecord.Attributes.Add(item.Key, null);
                }
            }
        }

        public void CreateMatchedRecordInTarget(Entity sourceRecord, EntityCollection matchedTargetRecords, ResultItem resultItem, int index)
        {
            int statusValue = -1;
            SetRecordTransactionCurrency(sourceRecord);
            string sourceRecordId = sourceRecord.GetAttributeValue<Guid>(sourceRecord.LogicalName + "id").ToString();

            if (ConfigReader.GetFetchXmlPrimaryKey() == null)
            {
                sourceRecord[sourceRecord.LogicalName + "id"] = Guid.NewGuid();
            }

            string sourceRecordIdInTarget = sourceRecord.GetAttributeValue<Guid>(sourceRecord.LogicalName + "id").ToString();

            bool checkForInactiveRecord = ((matchedTargetRecords == null || MatchedAction.CheckedRadioButtonNumbers[index] != 3) &&
                sourceRecord.Attributes.ContainsKey("statecode") && sourceRecord.GetAttributeValue<OptionSetValue>("statecode").Value == 1);

            if (checkForInactiveRecord && sourceRecord.Attributes.ContainsKey("statuscode"))
            {
                statusValue = sourceRecord.GetAttributeValue<OptionSetValue>("statuscode").Value;
                sourceRecord.Attributes.Remove("statuscode");
            }

            if (matchedTargetRecords == null)
            {
                CreateRequest createRequest = new CreateRequest { Target = sourceRecord };
                _targetService.Execute(createRequest);
                resultItem.CreatedRecordCount++;
                _logger.LogInfo($"Created the record with Id {{{sourceRecordId}}} in the target instance with Id {{{sourceRecordIdInTarget}}}.");
            }
            else if (MatchedAction.CheckedRadioButtonNumbers[index] == 2)
            {
                foreach (var matchedTargetRecord in matchedTargetRecords.Entities)
                {
                    _targetService.Delete(matchedTargetRecord.LogicalName, matchedTargetRecord.Id);
                    resultItem.DeletedRecordCount++;
                    _logger.LogWarning($"Deleted the record with Id {{{matchedTargetRecord.Id}}} from the target instance.");
                }

                _targetService.Create(sourceRecord);
                resultItem.CreatedRecordCount++;
                _logger.LogInfo($"Created the record with Id {{{sourceRecordId}}} in the target instance with Id {{{sourceRecordIdInTarget}}}.");
            }
            else if (MatchedAction.CheckedRadioButtonNumbers[index] == 3)
            {
                foreach (var matchedTargetRecord in matchedTargetRecords.Entities)
                {
                    SetSourceRecordUnfilledFieldsDefaultValue(sourceRecord, matchedTargetRecord);
                    Entity newRecord = new Entity(sourceRecord.LogicalName);
                    newRecord.Attributes.AddRange(sourceRecord.Attributes);
                    newRecord[sourceRecord.LogicalName + "id"] = matchedTargetRecord[sourceRecord.LogicalName + "id"];
                    _targetService.Update(newRecord);
                    resultItem.UpdatedRecordCount++;
                    _logger.LogInfo($"Updated the record with Id {{{matchedTargetRecord.Id}}} in the target instance.");
                }
            }
            else
            {
                CreateRequest createRequest = new CreateRequest { Target = sourceRecord };
                createRequest.Parameters.Add("SuppressDuplicateDetection", false);
                _targetService.Execute(createRequest);
                resultItem.CreatedRecordCount++;
                _logger.LogInfo($"Created the record with Id {{{sourceRecordId}}} in the target instance with Id {{{sourceRecordIdInTarget}}}");
            }

            if (checkForInactiveRecord)
            {
                if (statusValue != -1)
                {
                    sourceRecord.Attributes.Add("statuscode", new OptionSetValue(statusValue));
                }

                _targetService.Update(sourceRecord);
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

            return _targetService.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        public EntityCollection GetTargetMatchedRecords(Entity sourceRecord, string attributeSchemaName, string attributeType)
        {
            FilterExpression filterExpression = new FilterExpression();

            if (attributeType == "Double" || attributeType == "Memo" || attributeType == "Integer" || attributeType == "String" || attributeType == "BigInt"
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
                throw new Exception($"Cannot find the type of {attributeSchemaName}");
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
            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityLogicalName,
                LogicalName = logicalName,
                RetrieveAsIfPublished = true
            };

            var attributeResponse = (RetrieveAttributeResponse)_sourceService.Execute(attributeRequest);

            return attributeResponse.AttributeMetadata.AttributeType.ToString();
        }

        public void IsFetchXmlExpressionValid(string fetchXml)
        {
            var validateFetchXmlExpressionRequest = new FetchXmlToQueryExpressionRequest
            {
                FetchXml = fetchXml
            };

            _sourceService.Execute(validateFetchXmlExpressionRequest);
        }
    }
}