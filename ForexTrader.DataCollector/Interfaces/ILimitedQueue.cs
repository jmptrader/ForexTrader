using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.DataCollector.Interfaces
{
    public interface ILimitedQueue<T>
    {
        void Enqueue(T value);

        T Dequeue();

        void ClearQueue();
    }
}
