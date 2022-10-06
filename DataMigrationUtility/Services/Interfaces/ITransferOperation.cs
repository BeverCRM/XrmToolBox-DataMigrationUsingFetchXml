using XrmMigrationUtility.Model;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface ITransferOperation
    {
        List<ResultItem> ResultItems { get; set; }

        void Transfer();

        void InitialiseFields(AdditionalDetails additionalDetails, List<string> entityNames, string fetchPathText);
    }
}