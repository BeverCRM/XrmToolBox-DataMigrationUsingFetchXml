using System.Windows.Forms;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface ILogger
    {
        int LastIndex { get; set; }

        void Log(string text, bool isError = false);

        void SetTxtLogs(RichTextBox txtLogs);

        void SetLogsPath(string logsPath);
    }
}
