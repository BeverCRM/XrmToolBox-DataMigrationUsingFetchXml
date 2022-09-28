using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Tooling.Connector;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface IDataverseService
    {
        CrmServiceClient Service { get; set; }

        ILogger Logger { get; set; }

        EntityCollection GetAllRecords(string fetchQuery);

        Guid CreateRecord(Entity record, bool duplicateDetection = true);

        void MapSearchAttributes(Entity record, List<string> searchAttrs);

        string GetEntityPrimaryField(string entitySchemaName);
    }
}