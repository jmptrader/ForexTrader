using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Concurrent;

namespace ForEXTrader
{
    class Program
    {
        static void Main(string[] args)
        {
            // var menu = new Menu();
            // menu.StartMenu();

            //Create queues
            var loggerQueue = new ConcurrentQueue<object>();
            var queue = new LimitedQueue<HttpContent>(5);

            var logger = new Logger(loggerQueue);
            var collector = new Collector(200, loggerQueue, queue);
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
