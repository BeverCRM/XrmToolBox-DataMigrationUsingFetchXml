using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface IDataverseService
    {
        void CreateMatchedRecordInTarget(Entity record, EntityCollection matchedTargetRecords, ResultItem resultItem, int checkedMatchedActionRadioButtonNumber);
        void MapSearchAttributes(Entity record, List<string> searchAttrs);
        (string logicalName, string displayName) GetEntityName(string fetchXml);
        IEnumerable<EntityCollection> GetAllRecords(string fetchQuery);
        EntityCollection GetTargetMatchedRecords(Entity sourceRecord, string attributeSchemaName, string attributeType);
        string GetAttributeType(string logicalName, string entityLogicalName);
        void ThrowExceptionIfFetchXmlIsInvalid(string fetchXml);
    }
}