using System.Windows.Forms;
using DataMigrationUsingFetchXml.Model;
using System.Collections.Generic;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface ITransferOperation
    {
        List<ResultItem> ResultItems { get; set; }

        List<string> DisplayNames { get; set; }

        bool KeepRunning { get; set; }

        void Transfer(List<string> fetchXmls, List<int> indexesForTransfer, RichTextBox richTextBoxLogs, Button btnTransferData);

        void SetConnectionDetails(ConnectionDetails connectionDetails);

        void SetLabel(Label lblInfo, Label lblTitle, Label lblError);
    }
}