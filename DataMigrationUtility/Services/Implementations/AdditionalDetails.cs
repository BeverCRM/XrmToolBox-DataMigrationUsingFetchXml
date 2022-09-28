using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.Collections.ObjectModel;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Services.Implementations
{
    internal class AdditionalDetails : IAdditionalDetails
    {
        public IOrganizationService Service { get; set; }

        public ObservableCollection<ConnectionDetail> AdditionalConnectionDetails { get; set; }
    }
}