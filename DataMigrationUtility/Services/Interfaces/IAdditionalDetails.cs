using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.Collections.ObjectModel;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface IAdditionalDetails
    {
        IOrganizationService Service { get; set; }

        ObservableCollection<ConnectionDetail> AdditionalConnectionDetails { get; set; }
    }
}