namespace XrmMigrationUtility.Services
{
    internal sealed class ResultItem
    {
        public string EntityName { get; set; }
        public int SourceRecordCount { get; set; }
        public int SuccessfullyGeneratedRecordCount { get; set; }

        public ResultItem(string name, int numberOfSource = 0, int numberOfTarget = 0)
        {
            EntityName = name;
            SourceRecordCount = numberOfSource;
            SuccessfullyGeneratedRecordCount  = numberOfTarget;
        }
    }
}
