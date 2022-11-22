using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using DataMigrationUsingFetchXml.Services.Interfaces;

namespace DataMigrationUsingFetchXml.Services.Implementations
{
    internal sealed class Logger : ILogger
    {
        private string _logsPath;
        private RichTextBox _richTxtBoxLogs;

        public void SetTxtLogs(RichTextBox richtxtBoxLogs)
        {
            _richTxtBoxLogs = richtxtBoxLogs;
        }

        public void SetLogsPath(string logsPath)
        {
            string logName = $"Log_{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.txt";
            _logsPath = $"{logsPath}\\{logName}";
        }

        public void LogInfo(string text)
        {
            if (_logsPath != null)
            {
                text = " INFO: " + text;
                string logText = $"[{DateTime.Now:G}]" + text + "\n";
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
                LogErrorToTextBox(text);
                string logText = $"[{DateTime.Now:G}]" + " ERROR: " + text + "\n";
                LogToFile(logText);
            }
            else
            {
                throw new Exception("Log Path is Null!");
            }
        }

        private void LogErrorToTextBox(string text)
        {
            _richTxtBoxLogs.AppendText($"[{DateTime.Now:G}]");
            _richTxtBoxLogs.SelectionStart = _richTxtBoxLogs.Text.Length;
            _richTxtBoxLogs.SelectionLength = 0;
            _richTxtBoxLogs.SelectionColor = Color.Red;
            _richTxtBoxLogs.AppendText(" ERROR: ");
            _richTxtBoxLogs.SelectionColor = _richTxtBoxLogs.ForeColor;
            _richTxtBoxLogs.AppendText(text + "\n");
        }

        private void LogToTextBox(string logText)
        {
            _richTxtBoxLogs.AppendText(logText);
        }

        private void LogToFile(string logText)
        {
            File.AppendAllText(_logsPath, logText);
        }
    }
}