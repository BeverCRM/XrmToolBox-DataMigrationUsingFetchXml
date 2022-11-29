using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.Collections.ObjectModel;

namespace DataMigrationUsingFetchXml.Model
{
    internal sealed class ConnectionDetails
    {
        public IOrganizationService Service { get; set; }
        public ObservableCollection<ConnectionDetail> AdditionalConnectionDetails { get; set; }

        public ConnectionDetails(IOrganizationService service, ObservableCollection<ConnectionDetail> additionalConnectionDetails)
        {
            Service = service;
            AdditionalConnectionDetails = additionalConnectionDetails;
        }
    }
}