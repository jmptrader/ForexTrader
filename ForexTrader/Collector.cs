using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;

namespace ForEXTrader
{
    public class Collector
    {
        readonly private int _frequency;
        private  ConcurrentQueue<object> _loggerQueue;
        private LimitedQueue<HttpContent> _queue;
        private ApiRequest _collectorRequester;

        public Collector(int frequency, ConcurrentQueue<object> loggerQueue, LimitedQueue<HttpContent> queue)
        {
            _frequency = frequency;
            _loggerQueue = loggerQueue;
            _queue = queue;
        }

        public void Runner()
        {
            while (true)
            {
                var res = CollectCurrentRates();
                if (res != null)
                {
                    _queue.Enqueue(res);
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(_frequency));
            }
        }

        public HttpContent CollectCurrentRates()
        {
            var response = _collectorRequester.GetExchangeRates().Result;
            if (!response.IsSuccessStatusCode)
            {
                _loggerQueue.Enqueue(response);
                return null;
            }
            return response.Content;
        }
    }
}
