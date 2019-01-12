using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Concurrent;

namespace ForEXTrader
{
    public class Logger
    {
        private StreamWriter _logWriter;
        private string _dirPath;
        private readonly string _unixLoc = @"/opt/ForexTrader/";
        private readonly string _winLoc = @"Program Files\ForexTrader\";
        private ConcurrentQueue<object> _loggerQueue;

        public Logger(ConcurrentQueue<object> loggerQueue)
        {
            _loggerQueue = loggerQueue;
            SetupLogFile();
        }

        private void SetupLogFile()
        {
            if (SystemLib.IsUnix())
            {
                _dirPath = SystemLib.ArchRootPath(_unixLoc);
            }
            else
            {
                _dirPath = SystemLib.ArchRootPath(_winLoc);
            }

            if (!File.Exists(_dirPath))
            {
                try
                {
                    Directory.CreateDirectory(_dirPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not Create log file. " + ex);
                    Console.WriteLine("Exiting Program... Try running as admin.");
                    Environment.Exit(0);
                }
            }

            _logWriter = new StreamWriter(_dirPath + "ForexTrader.log", true);
        }

        public void QueueChecker()
        {
            while (true)
            {
                if (_loggerQueue.Any())
                {
                    _loggerQueue.TryDequeue(out var item);
                    var objectType = item.GetType();
                    if (item.GetType() == typeof(HttpResponseMessage))
                    {
                        var message = (HttpResponseMessage)item;
                        LogHttpResponseMessage(message);
                    }
                    else if (item.GetType() == typeof(string))
                    {
                        var message = item.ToString();
                        LogStringMessage(message);
                    }
                }
            }
        }

        private void LogHttpResponseMessage(HttpResponseMessage responseMessage)
        {
            // format - Date Time Method Response Reason
            // example - 06 / 12 / 2018 21:30:24.6886938, GET, https://api-fxpractice.oanda.com/v3/accounts/bef06b9e0a53a0ca13149ebcecb3f9fc-606b51db4feaa79f58827dcb4d50d5e7/pricing, Bad Request

            var dateTime = DateTime.Now;
            var logMessage = string.Format("{0} {1}, {2}, {3}, {4}",
                dateTime.ToShortDateString(),
                dateTime.TimeOfDay,
                responseMessage.RequestMessage.Method.Method,
                responseMessage.RequestMessage.RequestUri.AbsoluteUri,
                responseMessage.ReasonPhrase);

            _logWriter.WriteLine(logMessage);
        }

        private void LogStringMessage(string stringMessage)
        {
            var dateTime = DateTime.Now;
            var logMessage = string.Format("{0} {1}, {2}",
                dateTime.ToShortDateString(),
                dateTime.TimeOfDay,
                stringMessage);

            _logWriter.WriteLine(logMessage);
        }
    }
}
