using ForexTrader.DataCollector;
using ForexTrader.DataCollector.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace ForexTrader.ML
{
    public class DataQueueReceiver
    {
        private ICollector _collector;

        public DataQueueReceiver(ICollector collector)
        {
            _collector = collector;
        }

        public void StartMLing()
        {
            while (true)
            {
                SpinWait.SpinUntil(() => _collector.CollectorQueueCount() > 0);
                var data = _collector.TakeLatestFromCollectorQueue();
            }
        }
    }
}
