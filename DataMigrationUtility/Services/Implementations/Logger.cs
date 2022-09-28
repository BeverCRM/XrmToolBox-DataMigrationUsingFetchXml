using System;
using System.IO;
using System.Windows.Forms;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Services.Implementations
{
    internal sealed class Logger : ILogger
    {
        private TextBox _txtLogs;

        private string _logsPath;

        public void SetTxtLogs(TextBox txtLogs)
        {
            _txtLogs = txtLogs;
        }

        public void SetLogsPath(string logsPath)
        {
            _logsPath = $"{logsPath}\\Logs[{DateTime.Now.Millisecond}].txt";
        }

        public void Log(string text)
        {
            string dateTiemStr = DateTime.Now.ToString("G");
            string logText = $"[{dateTiemStr}] " + text;

            LogToTextBox(logText);
            LogToFile(logText);
        }

        private void LogToTextBox(string logText)
        {
            _txtLogs.Text += logText;
            _txtLogs.Text += Environment.NewLine;
        }

        private void LogToFile(string logText)
        {
            File.AppendAllText(_logsPath, logText);
            File.AppendAllText(_logsPath, Environment.NewLine);
        }

    }
}