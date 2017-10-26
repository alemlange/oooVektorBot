using System;
using System.IO;
using DataModels.Configuration;

namespace Bot.Tools
{
    public class LogWriter
    {
        protected string Expath { get; set; }

        public LogWriter()
        {
            Expath = ConfigurationSettings.ExceptionPath;
        }

        public void WriteException(string message)
        {
            var file = new StreamWriter(Expath, true);
            file.WriteLine(DateTime.Now.ToString() + " - " + message);
            file.Close();
        }
    }
}