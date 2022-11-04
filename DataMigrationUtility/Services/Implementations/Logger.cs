using System;
using System.IO;
using System.Windows.Forms;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Services.Implementations
{
    internal sealed class Logger : ILogger
    {
        private string _logsPath;
        private RichTextBox _richTxtBoxLogs;
        private readonly string _dateTiemStr = DateTime.Now.ToString("G");

        public void SetTxtLogs(RichTextBox richtxtBoxLogs)
        {
            _richTxtBoxLogs = richtxtBoxLogs;
        }

        public void SetLogsPath(string logsPath)
        {
            _logsPath = $"{logsPath}\\Log - {(DateTime.Now - DateTime.MinValue).TotalSeconds}.txt";
        }

        public void LogInfo(string text)
        {
            if (_logsPath != null)
            {
                text = " INFO: " + text;
                string logText = $"[{_dateTiemStr}]" + text;
                LogToTextBox(logText);
                LogToFile(logText);
            }
            else
            {
                throw new Exception("Log Path is Null!");
            }
        }

        public void LogError(string text)
        {
            if (_logsPath != null)
            {
                text = " ERROR: " + text;
                string logText = $"[{_dateTiemStr}]" + text;
                LogToTextBox(logText);
                LogToFile(logText);
            }
            else
            {
                throw new Exception("Log Path is Null!");
            }
        }

        private void LogToTextBox(string logText)
        {
            _richTxtBoxLogs.Text += logText;
            _richTxtBoxLogs.Text += Environment.NewLine;
        }

        private void LogToFile(string logText)
        {
            File.AppendAllText(_logsPath, logText);
            File.AppendAllText(_logsPath, Environment.NewLine);
        }
    }
}