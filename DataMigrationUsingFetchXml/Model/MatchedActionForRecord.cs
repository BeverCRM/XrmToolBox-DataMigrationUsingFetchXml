namespace DataMigrationUsingFetchXml.Model
{
    internal enum MatchedActionForRecord
    {
        Create = 1,
        DeleteAndCreate = 2,
        Upsert = 3,
        DoNotCreate = 4
    }
}