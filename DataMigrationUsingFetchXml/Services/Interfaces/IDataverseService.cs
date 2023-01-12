using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface IDataverseService
    {
        void CreateRecord(Entity record, Entity matchedTargetRecord, int index);
        void MapSearchAttributes(Entity record, List<string> searchAttrs);
        string GetEntityPrimaryField(string entitySchemaName);
        (string logicalName, string displayName) GetEntityName(string fetchXml);
        IEnumerable<EntityCollection> GetAllRecords(string fetchQuery);
        Entity GetRecord(string entitySchemaName, string attributeSchemaName, string attributeValue);
        EntityCollection GetTargetMatchedRecords(Entity sourceRecord, string attributeSchemaName, string attributeType);
        string GetAttributeType(string logicalName, string entityLogicalName);
    }
}