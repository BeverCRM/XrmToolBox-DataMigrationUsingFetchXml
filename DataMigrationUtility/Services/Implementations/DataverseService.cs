﻿using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using Microsoft.Xrm.Tooling.Connector;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Services.Implementations
{
    internal sealed class DataverseService : IDataverseService
    {
        private readonly IOrganizationService _sourceService;
        private readonly CrmServiceClient _targetService;
        private readonly ILogger _logger;

        public DataverseService(IOrganizationService sourceService, CrmServiceClient targetService, ILogger logger)
        {
            _sourceService = sourceService;
            _targetService = targetService;
            _logger = logger;
        }

        public DataverseService(IOrganizationService service)
        {
            _sourceService = service;
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
                EntityCollection returnCollection = _sourceService.RetrieveMultiple(new FetchExpression(xml));
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
            //createRequest.Parameters.Add("SuppressDuplicateDetection", duplicateDetection);
            CreateResponse response = (CreateResponse)_targetService.Execute(createRequest);

            return response.id;
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
            EntityCollection records = _targetService.RetrieveMultiple(query);

            return records.Entities.FirstOrDefault();
        }

        public string GetDisplayName(string fetchXml)
        {
            EntityCollection returnCollection = _sourceService.RetrieveMultiple(new FetchExpression(fetchXml));

            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = returnCollection.Entities[0].LogicalName
            };
            RetrieveEntityResponse retrieveAccountEntityResponse = (RetrieveEntityResponse)_sourceService.Execute(retrieveEntityRequest);
            EntityMetadata AccountEntity = retrieveAccountEntityResponse.EntityMetadata;

            return AccountEntity.DisplayName.UserLocalizedLabel.Label;
        }

        public string GetLogicalName(string fetchXml)
        {
            EntityCollection returnCollection = _sourceService.RetrieveMultiple(new FetchExpression(fetchXml));

            return returnCollection.Entities[0].LogicalName;
        }
    }
}