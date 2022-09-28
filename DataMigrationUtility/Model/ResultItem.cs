using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Model
{
    internal sealed class ResultItem : IResultItem
    {
        public string EntityName { get; set; }

        public int SourceRecordCount { get; set; } = 0;

        public int SuccessfullyGeneratedRecordCount { get; set; } = 0;
    }
}