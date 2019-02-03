using ForexTrader.DataCollector;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace ForexTrader.ML
{
    public class DataQueueReceiver
    {
        private LimitedQueue<JObject> _dataQueue;

        public DataQueueReceiver(LimitedQueue<JObject> dataQueue)
        {
            _dataQueue = dataQueue;
        }

        public void StartMLing()
        {
            while (true)
            {
                SpinWait.SpinUntil(() => _dataQueue.Count > 0);
                var data = _dataQueue.Dequeue();
            }
        }
    }
}
