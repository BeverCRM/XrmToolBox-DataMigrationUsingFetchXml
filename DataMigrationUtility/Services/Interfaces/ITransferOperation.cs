﻿using System.Windows.Forms;
using XrmMigrationUtility.Model;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface ITransferOperation
    {
        List<ResultItem> ResultItems { get; set; }

        List<string> DisplayNames { get; set; }

        bool KeepRunning { get; set; }

        void Transfer(List<string> fetchXmls, List<int> indexesForTransfer, RichTextBox richTextBoxLogs);

        void SetConnectionDetails(ConnectionDetails connectionDetails);

        void SetLabel(Label lblInfo, Label lblTitle, Label lblError);
    }
}