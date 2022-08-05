using System;
using System.IO;
using System.Windows.Forms;

namespace NewXrmToolBoxTool1
{
    public class Logger
    {
        readonly TextBox _txtLogs;
        readonly string _logsPath;

        public Logger(TextBox txtLogs, string logsPath)
        {
            logsPath += "\\Logs.txt";
            if (File.Exists(logsPath))
            {
                File.Delete(logsPath);
            }
            _txtLogs = txtLogs;
            _logsPath = logsPath;
        }

        public void Log(string text)
        {
            string dateTiemStr = DateTime.Now.ToString("G");
            string logText = $"[{dateTiemStr}] " + text;

            _txtLogs.Text += text;
            _txtLogs.Text += Environment.NewLine;
            File.AppendAllText(_logsPath, logText);//D:\\XrmToolbox\\Logs.txt Environment.NewLine
            File.AppendAllText(_logsPath, Environment.NewLine);
        }
    }
}
