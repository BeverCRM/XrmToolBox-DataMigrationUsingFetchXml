using System.Windows.Forms;

namespace XrmMigrationUtility.Services.Interfaces
{
    internal interface ILogger
    {
        void Log(string text);

        void SetTxtLogs(TextBox txtLogs);

        void SetLogsPath(string logsPath);
    }
}
