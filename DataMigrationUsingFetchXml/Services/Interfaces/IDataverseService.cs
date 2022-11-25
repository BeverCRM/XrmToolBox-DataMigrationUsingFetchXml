using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface IDataverseService
    {
        EntityCollection GetAllRecords(string fetchQuery);

        Guid CreateRecord(Entity record, bool duplicateDetection = true);

        void MapSearchAttributes(Entity record, List<string> searchAttrs);

        string GetEntityPrimaryField(string entitySchemaName);

        (string logicalName, string displayName) GetEntityName(string fetchXml);
    }
}