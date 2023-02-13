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
            if (_logsPath == null)
            {
                throw new Exception("Log Path is Null!");
            }

            text = " INFO: " + text;
            string logText = $"[{DateTime.Now:G}]" + text + "\n";
            LogToTextBox(logText);
            LogToFile(logText);
        }

        public void LogError(string text)
        {
            if (_logsPath == null)
            {
                throw new Exception("Log Path is Null!");
            }

            string word = " ERROR: ";
            ColorTextBoxLogs(text, word, Color.Red);
            string logText = $"[{DateTime.Now:G}]" + word + text + "\n";
            LogToFile(logText);
        }

        public void LogWarning(string text)
        {
            if (_logsPath == null)
            {
                throw new Exception("Log Path is Null!");
            }

            string word = " WARNING: ";
            ColorTextBoxLogs(text, word, Color.Gold);
            string logText = $"[{DateTime.Now:G}]" + word + text + "\n";
            LogToFile(logText);
        }

        private void ColorTextBoxLogs(string text, string colorWord, Color color)
        {
            _richTxtBoxLogs.Invoke(new MethodInvoker(delegate
            {
                _richTxtBoxLogs.AppendText($"[{DateTime.Now:G}]");
                _richTxtBoxLogs.SelectionStart = _richTxtBoxLogs.Text.Length;
                _richTxtBoxLogs.SelectionLength = 0;
                _richTxtBoxLogs.SelectionColor = color;
                _richTxtBoxLogs.AppendText($"{colorWord}");
                _richTxtBoxLogs.SelectionColor = _richTxtBoxLogs.ForeColor;
                _richTxtBoxLogs.AppendText(text + "\n");
            }));
        }

        private void LogToTextBox(string logText)
        {
            _richTxtBoxLogs.Invoke((MethodInvoker)(() => _richTxtBoxLogs.AppendText(logText)));
        }

        private void LogToFile(string logText)
        {
            _richTxtBoxLogs.Invoke(new MethodInvoker(delegate
            {
                File.AppendAllText(_logsPath, logText);
            }));
        }
    }
}