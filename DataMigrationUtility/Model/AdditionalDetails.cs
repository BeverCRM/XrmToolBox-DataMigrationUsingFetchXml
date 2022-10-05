using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.Collections.ObjectModel;

namespace XrmMigrationUtility.Model
{
    internal class AdditionalDetails
    {
        public IOrganizationService Service { get; set; }

        public ObservableCollection<ConnectionDetail> AdditionalConnectionDetails { get; set; }
    }
}