using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.DataCollector
{
    public class LimitedQueue<T> : LinkedList<T>
    {
        private readonly int _capacity;
        private LinkedList<T> _queue = new LinkedList<T>();

        public LimitedQueue(int capacity)
        {
            _capacity = capacity;
        }

        public void Enqueue(T value)
        {
            if (_queue.Count == _capacity)
            {
                RemoveLastItem();
            }
            _queue. AddFirst(value);
        }

        public LinkedListNode<T> Dequeue()
        {
            var item = _queue.Last;
            _queue.RemoveLast();
            return item;
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }

        private void RemoveLastItem()
        {
            _queue.RemoveLast();
        }
    }
}
