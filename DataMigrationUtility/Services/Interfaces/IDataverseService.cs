using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services.Interfaces
{
    public interface IDataverseService
    {
        EntityCollection GetAllRecords(string fetchQuery);

        Guid CreateRecord(Entity record, bool duplicateDetection = true);

        void MapSearchAttributes(Entity record, List<string> searchAttrs, ILogger logger);

        string GetEntityPrimaryField(string entitySchemaName);
    }
}
