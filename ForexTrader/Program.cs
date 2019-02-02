﻿using System;
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

namespace ForexTrader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Setting up logger.");
            var loggerQueue = new ConcurrentQueue<object>();
            var accountSettingsQueue = new ConcurrentQueue<AccountSettingsMessage>();
            var logger = new Logger(loggerQueue);
            var settingsQueue = new ConcurrentQueue<AccountSettingsMessage>();


            Console.WriteLine("Setting up menu.");
            var menu = new Menu(loggerQueue, settingsQueue);
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
            Console.WriteLine("Setting up collector.");        

            var dataResultQueue = new LimitedQueue<JObject>(5);
            var collector = new Collector(50, loggerQueue, settingsQueue);

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
