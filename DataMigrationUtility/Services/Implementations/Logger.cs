using System;
using System.IO;
using System.Windows.Forms;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Services.Implementations
{
    internal sealed class Logger : ILogger
    {
        private readonly TextBox _txtLogs;
        private readonly string _logsPath;

        public Logger(TextBox txtLogs, string logsPath)
        {
            _logsPath = $"{logsPath}\\Logs[{DateTime.Now.Millisecond}].txt";
            _txtLogs = txtLogs;
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