using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.DataCollector.Interfaces
{
    public interface ILimitedQueue<T>
    {
        int Count();

        void Enqueue(T value);

        T Dequeue();

        void ClearQueue();
    }
}
