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

        void InitialiseFields(ConnectionDetails additionalDetailsm, System.Windows.Forms.Label lblInfo, System.Windows.Forms.Label lblTitle, List<string> displayNames);
    }
}