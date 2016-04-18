using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Player.AI.Neat.Trainer
{
    internal sealed class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _buffer;

        private int _nextIndex;

        public CircularBuffer(int size)
        {
            _buffer = new T[size];
            _nextIndex = 0;
        }

        public void Add(T item)
        {
            lock (this)
            {
                _buffer[_nextIndex++] = item;

                _nextIndex %= _buffer.Length;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _buffer.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}