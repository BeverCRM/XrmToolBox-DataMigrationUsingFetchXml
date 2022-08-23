namespace XrmMigrationUtility.Model.Interfaces
{
    public interface IResultItem
    {
        string EntityName { get; set; }

        int SourceRecordCount { get; set; }

        int SuccessfullyGeneratedRecordCount { get; set; }
    }
}