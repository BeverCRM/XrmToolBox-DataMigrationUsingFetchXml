﻿using System.Windows.Forms;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface ITransferOperation
    {
        List<ResultItem> ResultItems { get; set; }

        List<string> DisplayNames { get; set; }

        bool KeepRunning { get; set; }

        void Transfer(List<string> fetchXmls, List<int> indexesForTransfer, RichTextBox richTextBoxLogs);

        void SetConnectionDetails(ConnectionDetails connectionDetails);

        void SetFormData(FormData formData);
    }
}