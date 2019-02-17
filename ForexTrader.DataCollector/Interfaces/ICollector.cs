using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.DataCollector.Interfaces
{
    public interface ICollector
    {
        void AddToCollectorQueue(JObject item);

        JObject TakeLatestFromCollectorQueue();

        void AddToMessageQueue(IMessage messsage);

        int CollectorQueueCount();

        void Runner();
    }
}
