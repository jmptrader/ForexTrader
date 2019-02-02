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
using ForexTrader.DataCollector.Messages;

namespace ForexTrader.DataCollector
{
    public class Collector
    {
        readonly private int _frequency;
        private ConcurrentQueue<AccountSettingsMessage> _settingsQueue;
        private ConcurrentQueue<object> _loggerQueue;
        private LimitedQueue<JObject> _collectorQueue = new LimitedQueue<JObject>(5);
        private ApiRequests _apiRequests;
        
        public Collector(int frequency, ConcurrentQueue<object> loggerQueue, ConcurrentQueue<AccountSettingsMessage> settingsQueue)
        {
            _frequency = frequency;
            _loggerQueue = loggerQueue;
            _settingsQueue = settingsQueue;
        }

        public void Runner()
        {
            var legitimateSettingsReceived = true;
            var firstSettingsQueueCount = 0;
            while (true)
            {
                if (!_settingsQueue.IsEmpty)
                {
                    _settingsQueue.TryDequeue(out var newSettings);
                    firstSettingsQueueCount = _settingsQueue.Count;

                    if (newSettings == null || newSettings.ApiKey == string.Empty || newSettings.AccountId == string.Empty)
                    {
                        Console.WriteLine("Received invalid settings. Please input legitimate settings.");
                        _loggerQueue.Enqueue("Received invalid settings.");
                    }
                    else
                    {
                        legitimateSettingsReceived = true;
                        _apiRequests = new ApiRequests(newSettings.ApiKey, newSettings.AccountId);
                    }
                }

                if (legitimateSettingsReceived == true)
                {
                    var res = CollectCurrentRates();
                    if (res != null)
                    {
                        var content = JObject.Parse(res.ReadAsStringAsync().Result);
                        _collectorQueue.Enqueue(content);
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(_frequency));
                }
                else
                {
                    SpinWait.SpinUntil(() => _settingsQueue.Count > firstSettingsQueueCount);
                }
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
