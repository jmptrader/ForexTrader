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

namespace ForexTrader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Setting up logger.");
            var loggerQueue = new ConcurrentQueue<object>();
            var logger = new Logger(loggerQueue);

            Console.WriteLine("Setting up menu.");
            var menu = new Menu(loggerQueue);
            var menuTask = Task.Factory.StartNew(() =>
            {
                lock (menu)
                {
                    menu.StartMenu();
                }
            });

            // Wait for account settings to be assigned.
            SpinWait.SpinUntil(() => menu.RetrievedAccSettings == true);

            Console.WriteLine("Setting up ApiRequest library.");
            var apiRequests = new ApiRequests(menu.ApiKey, menu.AccountId);
            Console.WriteLine("Setting up collector.");        
            var queue = new LimitedQueue<JObject>(5);
            var collector = new Collector(50, loggerQueue, apiRequests);
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
        
            Task.WaitAll(loggerTask, collectorTask);
        }
    }
}
