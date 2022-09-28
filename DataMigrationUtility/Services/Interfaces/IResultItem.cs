namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface IResultItem
    {
        string EntityName { get; set; }

        int SourceRecordCount { get; set; }

        int SuccessfullyGeneratedRecordCount { get; set; }
    }
}