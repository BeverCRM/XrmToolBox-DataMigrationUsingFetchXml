using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface IDataverseService
    {
        void CreateRecord(Entity record, int index);
        void MapSearchAttributes(Entity record, List<string> searchAttrs);
        string GetEntityPrimaryField(string entitySchemaName);
        (string logicalName, string displayName) GetEntityName(string fetchXml);
        IEnumerable<EntityCollection> GetAllRecords(string fetchQuery);
    }
}