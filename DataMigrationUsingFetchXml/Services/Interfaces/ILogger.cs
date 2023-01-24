using System.Windows.Forms;

namespace DataMigrationUsingFetchXml.Services.Interfaces
{
    internal interface ILogger
    {
        void LogInfo(string text);
        void LogError(string text);
        void LogWarning(string text);
        void SetTxtLogs(RichTextBox txtLogs);
        void SetLogsPath(string logsPath);
    }
}
