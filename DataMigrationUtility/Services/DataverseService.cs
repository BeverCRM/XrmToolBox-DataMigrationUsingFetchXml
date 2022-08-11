using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using Microsoft.Xrm.Tooling.Connector;

namespace XrmMigrationUtility.Services
{
    internal sealed class DataverseService
    {
        private readonly CrmServiceClient _service;

        public DataverseService(CrmServiceClient service)
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
            CreateRequest createRequest = new CreateRequest
            {
                Target = record
            };
            createRequest.Parameters.Add("SuppressDuplicateDetection", duplicateDetection);
            CreateResponse response = (CreateResponse)_service.Execute(createRequest);

            return response.id;
        }

        public void MapSearchAttributes(Entity record, List<string> searchAttrs, Logger logger)
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
                        logger.Log("Can't find the '" + refValue.LogicalName + "' entity record with name '" + refValue.Name);
                        logger.Log($"Creating a new record of '{refValue.LogicalName}' with name '{refValue.Name}'...");

                        EntityReference newLookupRecordRef = CreateNewLookupRecord(refValue, primaryField);
                        record[searchAttr] = newLookupRecordRef;
                        logger.Log($"'{refValue.LogicalName}' record created successfully with id {{{newLookupRecordRef.Id}}}");
                    }
                }
            }
        }

        private Entity GetRecord(string attributeSchemaName, string entityAttrbiute, string attributeValue)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = attributeSchemaName,
                ColumnSet = new ColumnSet(null),
                Criteria =
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(entityAttrbiute, ConditionOperator.Equal, attributeValue)
                    }
                }
            };
            EntityCollection records = _service.RetrieveMultiple(query);
            Entity record = records.Entities.FirstOrDefault();

            return record;
        }

        public string GetEntityPrimaryField(string entitySchemaName)
        {
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entitySchemaName
            };
            var retrieveEntityResponse = (RetrieveEntityResponse)_service.Execute(retrieveEntityRequest);

            return retrieveEntityResponse.EntityMetadata.PrimaryNameAttribute;
        }

        private EntityReference CreateNewLookupRecord(EntityReference lookup, string primaryFieldName)
        {
            var newLookupRecord = new Entity(lookup.LogicalName);

            newLookupRecord.Attributes[primaryFieldName] = lookup.Name;

            newLookupRecord.Id = CreateRecord(newLookupRecord);

            return newLookupRecord.ToEntityReference();
        }
    }
}