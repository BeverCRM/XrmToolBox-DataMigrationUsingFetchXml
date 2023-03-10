namespace DataMigrationUsingFetchXml.Model
{
    internal sealed class ResultItem
    {
        public string DisplayName { get; set; }
        public string SchemaName { get; set; }
        public int SourceRecordCount { get; set; }
        public string SourceRecordCountWithSign { get; set; }
        public int ErroredRecordCount { get; set; }
        public int CreatedRecordCount { get; set; }
        public int SkippedRecordCount { get; set; }
        public int UpdatedRecordCount { get; set; }
        public int DeletedRecordCount { get; set; }
    }
}