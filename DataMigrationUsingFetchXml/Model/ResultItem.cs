namespace DataMigrationUsingFetchXml.Model
{
    internal sealed class ResultItem
    {
        public string DisplayName { get; set; }
        public string SchemaName { get; set; }
        public int SourceRecordCount { get; set; } = 0;
        public int ErroredRecordCount { get; set; } = 0;
        public int SuccessfullyGeneratedRecordCount { get; set; } = 0;
        public string SourceRecordCountWithSign { get; set; }
    }
}