namespace DataMigrationUsingFetchXml.Model
{
    internal sealed class ResultItem
    {
        public string DisplayName { get; set; }
        public string SchemaName { get; set; }
        public int SourceRecordCount { get; set; } = 0;
        public string SourceRecordCountWithSign { get; set; }
        public int ErroredRecordCount { get; set; } = 0;
        public int CreatedRecordCount { get; set; } = 0;
        public int SkippedRecordCount { get; set; } = 0;
        public int UpdatedRecordCount { get; set; } = 0;
        public int DeletedRecordCount { get; set; } = 0;
    }
}