using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.Collections.ObjectModel;

namespace XrmMigrationUtility.Model
{
    internal sealed class ConnectionDetails
    {
        public IOrganizationService Service { get; set; }

        public ObservableCollection<ConnectionDetail> AdditionalConnectionDetails { get; set; }
    }
}