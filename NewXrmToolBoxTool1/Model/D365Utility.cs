using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;

namespace NewXrmToolBoxTool1.Model
{
    public class D365Utility
    {
        private readonly CrmServiceClient _service;

        public D365Utility(CrmServiceClient service)
        {
            _service = service;
        }

        public EntityCollection GetRecords(string fetchQuery)
        {
            return _service.RetrieveMultiple(new FetchExpression(fetchQuery));
        }

        public EntityCollection GetAllRecords(string fetchQuery)
        {
            EntityCollection data = new EntityCollection();

            int fetchCount = 5000;
            int pageNumber = 1;
            string pagingCookie = null;

            while (true)
            {
                string xml = ConfigReader.CreateXml(fetchQuery, pagingCookie, pageNumber, fetchCount);
                EntityCollection returnCollection = _service.RetrieveMultiple(new FetchExpression(xml));
                data.Entities.AddRange(returnCollection.Entities);

                if (returnCollection.MoreRecords)
                {
                    ++pageNumber;
                    pagingCookie = returnCollection.PagingCookie;
                }
                else
                {
                    break;
                }
            }
            return data;
        }

        public Guid CreateRecord(Entity record, bool duplicateDetection = true)
        {
            CreateRequest create = new CreateRequest
            {
                Target = record
            };
            create.Parameters.Add("SuppressDuplicateDetection", duplicateDetection);
            CreateResponse response = (CreateResponse)_service.Execute(create);

            return response.id;
        }

        public void MapSearchAttributes(Entity record, List<string> searchAttrs, D365Utility sourceInstance, Logger logger)
        {
            foreach (string searchAttr in searchAttrs)
            {
                EntityReference refValue = record.GetAttributeValue<EntityReference>(searchAttr);

                if (refValue != null)
                {
                    string primaryField = GetEntityPrimaryField(refValue.LogicalName);
                    Entity refEntity = GetRecordByName(refValue.LogicalName, primaryField, refValue.Name);

                    if (refEntity != null)
                    {
                        record[searchAttr] = refEntity.ToEntityReference();
                    }
                    else
                    {
                        logger.Log("Can't find the '" + refValue.LogicalName + "' entity record with name '" + refValue.Name);
                        logger.Log($"Creating a new record of '{refValue.LogicalName}' with name '{refValue.Name}'...");

                        EntityReference newLookupRecordRef = CreateNewLookupRecord(refValue, primaryField, sourceInstance);
                        record[searchAttr] = newLookupRecordRef;
                        logger.Log($"'{refValue.LogicalName}' record created successfully with id {{{newLookupRecordRef.Id}}}");
                    }
                }
            }
        }

        private Entity GetRecordByName(string entitySchemaName, string entityAttrbiute, string value)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entitySchemaName,
                ColumnSet = new ColumnSet(null),
                Criteria =
                {
                    FilterOperator=LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(entityAttrbiute, ConditionOperator.Equal,value)
                    }
                }
            };
            EntityCollection records = _service.RetrieveMultiple(query);
            Entity record = records?.Entities.FirstOrDefault();

            return record;
        }

        public string GetEntityPrimaryField(string entity)
        {
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entity
            };

            RetrieveEntityResponse retrieveEntityEntityResponse = (RetrieveEntityResponse)_service.Execute(retrieveEntityRequest);
            EntityMetadata entityMetadata = retrieveEntityEntityResponse.EntityMetadata;

            return entityMetadata.PrimaryNameAttribute;
        }

        public EntityReference CreateNewLookupRecord(EntityReference lookup, string primaryFieldName, D365Utility sourceInstance)
        {
            var newLookupRecord = new Entity(lookup.LogicalName);

            if (lookup.LogicalName == "account" || lookup.LogicalName == "contact")
            {
                Entity sourceLookupRecord = sourceInstance._service.Retrieve(lookup.LogicalName, lookup.Id, new ColumnSet(true));
                foreach (var attr in sourceLookupRecord.Attributes)
                {
                    if (sourceLookupRecord.Attributes.Contains(attr.Key) && (sourceLookupRecord[attr.Key] is EntityReference || sourceLookupRecord[attr.Key] is Guid))
                        continue;

                    //if (!sourceLookupRecord.TryGetAttributeValue(attr.Key, out EntityReference value) && !sourceLookupRecord.TryGetAttributeValue(attr.Key, out Guid id))
                    newLookupRecord.Attributes.Add(attr);
                }
            }
            else
            {
                newLookupRecord.Attributes[primaryFieldName] = lookup.Name;
            }
            newLookupRecord.Id = CreateRecord(newLookupRecord);

            return newLookupRecord.ToEntityReference();
        }
    }
}