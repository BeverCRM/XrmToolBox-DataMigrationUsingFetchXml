using System.Windows.Forms;
using XrmMigrationUtility.Model;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface ITransferOperation
    {
        List<ResultItem> ResultItems { get; set; }

        bool KeepRunning { get; set; }

        void Transfer(List<string> fetchXmls, List<int> indexesForTransfer, RichTextBox richTextBoxLogs);

        void InitialiseFields(ConnectionDetails additionalDetailsm, Label lblInfo, Label lblTitle, Label lblError, List<string> displayNames);
    }
}