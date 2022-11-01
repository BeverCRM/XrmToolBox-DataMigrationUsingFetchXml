using System.Windows.Forms;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface ILogger
    {
        void Log(string text, string type = "info");

        void SetTxtLogs(RichTextBox txtLogs);

        void SetLogsPath(string logsPath);
    }
}
