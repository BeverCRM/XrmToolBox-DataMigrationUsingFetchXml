using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface IDataverseService
    {
        EntityCollection GetAllRecords(string fetchQuery);

        Guid CreateRecord(Entity record, bool duplicateDetection = true);

        void MapSearchAttributes(Entity record, List<string> searchAttrs);

        string GetEntityPrimaryField(string entitySchemaName);

        string GetDisplayName(string fetchXml);

        string GetLogicalName(string fetchXml);
    }
}