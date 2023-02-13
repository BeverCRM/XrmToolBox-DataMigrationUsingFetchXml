using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface IDataverseService
    {
        void CreateMatchedRecordInTarget(Entity record, EntityCollection matchedTargetRecords, ResultItem resultItem, int index);
        void MapSearchAttributes(Entity record, List<string> searchAttrs);
        (string logicalName, string displayName) GetEntityName(string fetchXml);
        IEnumerable<EntityCollection> GetAllRecords(string fetchQuery);
        Entity GetRecord(string entitySchemaName, string attributeSchemaName, string attributeValue);
        EntityCollection GetTargetMatchedRecords(Entity sourceRecord, string attributeSchemaName, string attributeType);
        string GetAttributeType(string logicalName, string entityLogicalName);
        void IsFetchXmlExpressionValid(string fetchXml);
    }
}