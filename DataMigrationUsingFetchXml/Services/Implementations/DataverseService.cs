using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using DataMigrationUsingFetchXml.Model;
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
            Entity currentUserSettings = _targetService.RetrieveMultiple(userSettingsQuery).Entities.FirstOrDefault();

            return currentUserSettings?.GetAttributeValue<EntityReference>("transactioncurrencyid");
        }

        private Entity GetBaseCurrency()
        {
            string fetchXml = $@"
                <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='transactioncurrency'>
                    <attribute name='transactioncurrencyid' />
                    <filter type='and'>
                      <condition attribute='exchangerate' operator='eq' value='1' />
                    </filter>
                  </entity>
                </fetch>";

            return _targetService.RetrieveMultiple(new FetchExpression(fetchXml)).Entities.FirstOrDefault();
        }

        private string GetCurrencyCode(Guid currencyId)
        {
            Entity currencyCode = _sourceService.Retrieve("transactioncurrency", currencyId, new ColumnSet("isocurrencycode"));

            return currencyCode.GetAttributeValue<string>("isocurrencycode");
        }

        private void SetRecordTransactionCurrency(Entity sourceRecord)
        {
            if (sourceRecord.Attributes.ContainsKey("transactioncurrencyid"))
            {
                if (!ConfigReader.CurrentFetchXml.Contains("transactioncurrencyid"))
                {
                    EntityReference defaultCurrency = GetDefaultTransactionCurrency();

                    if (defaultCurrency != null)
                    {
                        sourceRecord["transactioncurrencyid"] = defaultCurrency;
                    }
                    else
                    {
                        sourceRecord["transactioncurrencyid"] = GetBaseCurrency().ToEntityReference();
                    }

                    return;
                }

                string currencyCode = GetCurrencyCode(sourceRecord.GetAttributeValue<EntityReference>("transactioncurrencyid").Id);

                QueryExpression query = new QueryExpression("transactioncurrency")
                {
                    ColumnSet = new ColumnSet("isocurrencycode"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression("isocurrencycode", ConditionOperator.Equal, currencyCode)
                        }
                    }
                };

                EntityCollection transactionCurrencyCollection = _targetService.RetrieveMultiple(query);

                if (transactionCurrencyCollection.Entities.Count > 0)
                {
                    sourceRecord["transactioncurrencyid"] = transactionCurrencyCollection.Entities.FirstOrDefault().ToEntityReference();
                }
                else
                {
                    throw new Exception($"transactioncurrency With Id = {sourceRecord.GetAttributeValue<EntityReference>("transactioncurrencyid").Id} Does Not Exist");
                }
            }
        }

        private void SetSourceRecordBlankFieldsToDefaultValue(Entity sourceRecord, Entity matchedTargetRecord)
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

        private bool CheckForInactiveRecord(Entity sourceRecord, EntityCollection matchedTargetRecords, int radioButtonNumber)
        {
            return (matchedTargetRecords.Entities.Count == 0
                || radioButtonNumber != (byte)MatchedActionForRecord.Upsert)
                && sourceRecord.GetAttributeValue<OptionSetValue>("statecode")?.Value == 1; //1 is statecode inactive value.
        }

        private void CreateRecord(Entity sourceRecord, ResultItem resultItem, string sourceRecordId)
        {
            try
            {
                _targetService.Create(sourceRecord);
                resultItem.CreatedRecordCount++;
                _logger.LogInfo($"Created the record with Id {{{sourceRecordId}}} in the target instance with Id {{{sourceRecord.GetAttributeValue<Guid>(sourceRecord.LogicalName + "id")}}}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot create record with Id {{{sourceRecordId}}}. {ex.Message}");
            }
        }

        private void DeleteAndCreateRecords(Entity sourceRecord, EntityCollection matchedTargetRecords, ResultItem resultItem, string sourceRecordId)
        {
            foreach (var matchedTargetRecord in matchedTargetRecords.Entities)
            {
                _targetService.Delete(matchedTargetRecord.LogicalName, matchedTargetRecord.Id);
                resultItem.DeletedRecordCount++;
                _logger.LogWarning($"Deleted the record with Id {{{matchedTargetRecord.Id}}} from the target instance.");
            }

            CreateRecord(sourceRecord, resultItem, sourceRecordId);
        }

        private void UpdateRecords(Entity sourceRecord, EntityCollection matchedTargetRecords, ResultItem resultItem)
        {
            foreach (var matchedTargetRecord in matchedTargetRecords.Entities)
            {
                SetSourceRecordBlankFieldsToDefaultValue(sourceRecord, matchedTargetRecord);
                Entity newRecord = new Entity(sourceRecord.LogicalName);
                newRecord.Attributes.AddRange(sourceRecord.Attributes);
                newRecord[sourceRecord.LogicalName + "id"] = matchedTargetRecord[sourceRecord.LogicalName + "id"];
                _targetService.Update(newRecord);
                resultItem.UpdatedRecordCount++;
                _logger.LogInfo($"Updated the record with Id {{{matchedTargetRecord.Id}}} in the target instance.");
            }
        }

        public void CreateMatchedRecordInTarget(Entity sourceRecord, EntityCollection matchedTargetRecords, ResultItem resultItem, int checkedMatchedActionRadioButtonNumber)
        {
            int statusValue = -1;
            SetRecordTransactionCurrency(sourceRecord);
            string sourceRecordId = sourceRecord.GetAttributeValue<Guid>(sourceRecord.LogicalName + "id").ToString();

            if (ConfigReader.GetFetchXmlPrimaryKey() == null)
            {
                sourceRecord[sourceRecord.LogicalName + "id"] = Guid.NewGuid();
            }

            bool checkForInactiveRecord = CheckForInactiveRecord(sourceRecord, matchedTargetRecords, checkedMatchedActionRadioButtonNumber);

            if (checkForInactiveRecord && sourceRecord.Attributes.ContainsKey("statuscode"))
            {
                statusValue = sourceRecord.GetAttributeValue<OptionSetValue>("statuscode").Value;
                sourceRecord.Attributes.Remove("statuscode");
            }

            if (matchedTargetRecords.Entities.Count == 0 || checkedMatchedActionRadioButtonNumber == (byte)MatchedActionForRecord.Create)
            {
                CreateRecord(sourceRecord, resultItem, sourceRecordId);
            }
            else if (checkedMatchedActionRadioButtonNumber == (byte)MatchedActionForRecord.DeleteAndCreate)
            {
                DeleteAndCreateRecords(sourceRecord, matchedTargetRecords, resultItem, sourceRecordId);
            }
            else if (checkedMatchedActionRadioButtonNumber == (byte)MatchedActionForRecord.Upsert)
            {
                UpdateRecords(sourceRecord, matchedTargetRecords, resultItem);
            }

            if (checkForInactiveRecord)
            {
                if (statusValue != -1) // -1 means that record doesn't contain statuscode.
                {
                    sourceRecord["statuscode"] = new OptionSetValue(statusValue);
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

        private Entity GetRecord(string entitySchemaName, string attributeSchemaName, string attributeValue)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entitySchemaName,
                ColumnSet = new ColumnSet(null),
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

        private bool CheckAttributeForSimpleType(string attributeType)
        {
            string[] simpleTypes = new string[] { "Double", "Memo", "Integer", "String", "BigInt", "Boolean", "EntityName", "Decimal", "Uniqueidentifier" };

            return simpleTypes.Contains(attributeType);
        }

        private bool CheckAttributeTypeForEntityReference(string attributeType)
        {
            string[] entityReferenceTypes = new string[] { "Lookup", "Customer", "Owner" };

            return entityReferenceTypes.Contains(attributeType);
        }

        private bool CheckAttributeTypeForOptionSet(string attributeType)
        {
            string[] optionSetTypes = new string[] { "Picklist", "Status", "State" };

            return optionSetTypes.Contains(attributeType);
        }

        public EntityCollection GetTargetMatchedRecords(Entity sourceRecord, string attributeSchemaName, string attributeType)
        {
            FilterExpression filterExpression = new FilterExpression();

            if (CheckAttributeForSimpleType(attributeType))
            {
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, sourceRecord[attributeSchemaName]?.ToString()));
            }
            else if (attributeType == "DateTime" && DateTime.TryParse(sourceRecord[attributeSchemaName].ToString(), out DateTime date))
            {
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, date.ToLocalTime().ToString()));
            }
            else if (CheckAttributeTypeForEntityReference(attributeType))
            {
                EntityReference refValue = sourceRecord.GetAttributeValue<EntityReference>(attributeSchemaName);

                if (refValue == null)
                {
                    return new EntityCollection();
                }
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, refValue.Id.ToString()));
            }
            else if (attributeType == "Virtual")
            {
                OptionSetValueCollection optionSets = sourceRecord.GetAttributeValue<OptionSetValueCollection>(attributeSchemaName);
                int[] osValues = new int[] { };

                foreach (var optionSet in optionSets)
                {
                    osValues = osValues.Concat(new int[] { optionSet.Value }).ToArray();
                }
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.In, osValues));
            }
            else if (attributeType == "Money")
            {
                Money money = sourceRecord.GetAttributeValue<Money>(attributeSchemaName);
                filterExpression.Conditions.Add(new ConditionExpression(attributeSchemaName, ConditionOperator.Equal, money.Value));
            }
            else if (CheckAttributeTypeForOptionSet(attributeType))
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
            var attributeResponse = (RetrieveAttributeResponse)_targetService.Execute(attributeRequest);

            return attributeResponse.AttributeMetadata.AttributeType.ToString();
        }

        public void ThrowExceptionIfFetchXmlIsInvalid(string fetchXml)
        {
            var validateFetchXmlExpressionRequest = new FetchXmlToQueryExpressionRequest
            {
                FetchXml = fetchXml
            };
            _sourceService.Execute(validateFetchXmlExpressionRequest);
        }
    }
}