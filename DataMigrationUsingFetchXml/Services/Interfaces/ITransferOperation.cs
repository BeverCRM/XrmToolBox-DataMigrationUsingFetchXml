using System.Windows.Forms;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;
using System.ComponentModel;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface ITransferOperation
    {
        List<ResultItem> ResultItems { get; set; }
        List<string> DisplayNames { get; set; }
        void Transfer(List<string> fetchXmls, List<int> indexesForTransfer, BackgroundWorker worker, DoWorkEventArgs args);
        void SetConnectionDetails(ConnectionDetails connectionDetails);
    }
}