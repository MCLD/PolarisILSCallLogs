using PolarisCallLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseLogs
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = NLog.LogManager.GetLogger(AppDomain.CurrentDomain.FriendlyName);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                logger.Fatal($"An uncaught exception occurred: {e.ExceptionObject.ToString()}", e);
            };

            string filePath = @"c:\PHONE NOTIFICATION";

            var parser = new Parser();
            IEnumerable<string> fileList = System.IO.Directory.GetFiles(filePath, parser.FileBlob);
            foreach (string file in fileList)
            {
                var summary = parser.ReadLog($"{file}");
            }
        }
    }
}
