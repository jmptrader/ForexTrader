using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;
using ForexTrader.Logging;
using ForexTrader.DataCollector;
using ForexTrader.DataCollector.Messages;
using ForexTrader.ML;
using ForexTrader.Logging.Interfaces;
using ForexTrader.DataCollector.Interfaces;
using ForexTrader.Interfaces;

namespace ForexTrader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Setting up logger.");
            ILogger logger = new Logger();
            ICollector collector = new Collector(50, logger);
            var accountSettingsQueue = new ConcurrentQueue<AccountSettingsMessage>();
            
            Console.WriteLine("Setting up menu.");
            var menu = new Menu(logger, collector);
            var menuTask = Task.Factory.StartNew(() =>
            {
                lock (menu)
                {
                    menu.StartMenu();
                }
            });
                        
            // Wait for account settings to be assigned.
            SpinWait.SpinUntil(() => menu.RetrievedAccSettings == true);

            var mlForex = new DataQueueReceiver(collector);

            var loggerTask = Task.Factory.StartNew(() =>
            {
                lock (logger)
                {
                    logger.QueueChecker();
                }
            });

            var collectorTask = Task.Factory.StartNew(() =>
            {
                lock (collector)
                {
                    collector.Runner();
                }
            });

            var MLTask = Task.Factory.StartNew(() =>
            {
                lock (mlForex)
                {
                    mlForex.StartMLing();
                }
            });
        
            Task.WaitAll(loggerTask, collectorTask);
        }
    }
}
