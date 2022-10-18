using XrmMigrationUtility.Model;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface ITransferOperation
    {
        List<ResultItem> ResultItems { get; set; }

        void Transfer();

        void InitialiseFields(ConnectionDetails additionalDetails, List<string> entityNames, string fetchPathText);
    }
}