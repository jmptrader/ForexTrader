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
            _apiRequests = new ApiRequests();

            while (true)
            {
                if (!_settingsQueue.IsEmpty)
                {
                    while (true)
                    {
                        _settingsQueue.TryDequeue(out var newSettings);

                        // check if valid
                        if (AssertValidSettings(newSettings))
                        {
                            _apiRequests = new ApiRequests(newSettings.ApiKey, newSettings.AccountId);
                            break;
                        }
                        else
                        {
                            // check if old settings are still valid
                            _loggerQueue.Enqueue($"Could not apply new settings. Trying to revert back to old settings.");

                            if (!_apiRequests.HasValidSettings())
                            {
                                _loggerQueue.Enqueue($"Could not use old settings. Waiting for new settings to arrive.");
                                SpinWait.SpinUntil(() => !_settingsQueue.IsEmpty);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
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
            
        }

        private bool AssertValidSettings(AccountSettingsMessage newSettings)
        {
            if (newSettings == null || newSettings.ApiKey == string.Empty || newSettings.AccountId == string.Empty)
            {
                Console.WriteLine("\nReceived invalid settings. Please input legitimate settings.");
                _loggerQueue.Enqueue("Received invalid settings.");
                return false;
            }

            return true;
        }

        private HttpContent CollectCurrentRates()
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
