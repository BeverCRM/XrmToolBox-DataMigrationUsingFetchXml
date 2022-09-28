using Unity;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface ITransferOperation
    {
        List<IResultItem> ResultItems { get; set; }

        void Transfer();

        void InitialiseFields(IUnityContainer unityContainer, List<string> entityNames, string fetchPathText);
    }
}