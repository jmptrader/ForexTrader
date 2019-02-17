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
using ForexTrader.DataCollector.Interfaces;
using ForexTrader.Logging.Interfaces;

namespace ForexTrader.DataCollector
{
    public class Collector : ICollector
    {
        readonly private int _frequency;
        private ILogger _logger;
        private ILimitedQueue<JObject> _collectorQueue = new LimitedQueue<JObject>(5);
        private ConcurrentQueue<IMessage> _settingsQueue = new ConcurrentQueue<IMessage>();
        private ApiRequests _apiRequests;
        
        public Collector(int frequency, ILogger logger)
        {
            _frequency = frequency;
            _logger = logger;
        }

        public void AddToCollectorQueue(JObject item)
        {
            _collectorQueue.Enqueue(item);
        }

        public JObject TakeLatestFromCollectorQueue()
        {
            return _collectorQueue.Dequeue();
        }

        public int CollectorQueueCount()
        {
            return _collectorQueue.Count();
        }

        public void AddToMessageQueue(IMessage message)
        {
            _settingsQueue.Enqueue(message);
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
                            _logger.AddLogEntry($"Could not apply new settings. Trying to revert back to old settings.");

                            if (!_apiRequests.HasValidSettings())
                            {
                                _logger.AddLogEntry($"Could not use old settings. Waiting for new settings to arrive.");
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
                _logger.AddLogEntry("Received invalid settings.");
                return false;
            }

            return true;
        }

        private HttpContent CollectCurrentRates()
        {
            var response = _apiRequests.GetExchangeRates().Result;
            if (!response.IsSuccessStatusCode)
            {
                _logger.AddLogEntry(response);
                return null;
            }

            return response.Content;
        }
    }
}
