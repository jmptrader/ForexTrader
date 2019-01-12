using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ForexTrader.DataCollector
{
    public class Collector
    {
        readonly private int _frequency;
        private static ConcurrentQueue<object> _loggerQueue;
        private static LimitedQueue<JObject> _collectorQueue;
        private ApiRequests _apiRequests;

        public Collector(int frequency, ConcurrentQueue<object> loggerQueue, ApiRequests apiRequest)
        {
            _frequency = frequency;
            _loggerQueue = loggerQueue;
            _apiRequests = apiRequest;
            _collectorQueue = new LimitedQueue<JObject>(5);
        }

        public void Runner()
        {
            while (true)
            {
                var res = CollectCurrentRates();
                if (res != null)
                {
                    var content = JObject.Parse(res.ReadAsStringAsync().Result);
                    _collectorQueue.Enqueue(content);
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(_frequency));
            }
        }

        public HttpContent CollectCurrentRates()
        {
            var response = _apiRequests.GetExchangeRates().Result;
            if (!response.IsSuccessStatusCode)
            {
                _loggerQueue.Enqueue(response);
                return null;
            }
            return response.Content;
        }
    }
}
