using System;
using System.IO;
using System.Windows.Forms;
using XrmMigrationUtility.Services.Interfaces;

namespace XrmMigrationUtility.Services.Implementations
{
    internal sealed class Logger : ILogger
    {
        private RichTextBox _richtxtBoxLogs;
        private string _logsPath;

        public void SetTxtLogs(RichTextBox richtxtBoxLogs)
        {
            _richtxtBoxLogs = richtxtBoxLogs;
        }

        public void SetLogsPath(string logsPath)
        {
            _logsPath = $"{logsPath}\\Log - {(DateTime.Now - DateTime.MinValue).TotalSeconds}.txt";
        }

        public void Log(string text, bool isError = false)
        {
            if (_logsPath != null)
            {
                string dateTiemStr = DateTime.Now.ToString("G");
                if (!isError)
                {
                    text = " INFO: " + text;
                }
                else
                {
                    text = " ERROR: " + text;
                }
                string logText = $"[{dateTiemStr}]" + text;

                LogToTextBox(logText);
                LogToFile(logText);
            }
            else
            {
                throw new Exception("Log Path is Null!");
            }
        }

        private void LogToTextBox(string logText, bool isError = false)
        {
            _richtxtBoxLogs.Text += logText;
            _richtxtBoxLogs.Text += Environment.NewLine;
        }

        private void LogToFile(string logText)
        {
            File.AppendAllText(_logsPath, logText);
            File.AppendAllText(_logsPath, Environment.NewLine);
        }
    }
}