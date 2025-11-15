using System;

namespace Utils.Collections
{
    // source: https://gist.github.com/adammyhre/51aed0c1168ed7fcedf3d6ca6f5c036b (git-amend)
    // A fixed-size ring buffer that overwrites the oldest entries when full.
    public class CircularBuffer<T>
    {
        readonly T[] buffer;
        int head, tail;
        public int Count { get; private set; }
        public int Capacity => buffer.Length;

        public CircularBuffer(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            buffer = new T[size];
        }

        public void Enqueue(T item)
        {
            buffer[head] = item;
            head = (head + 1) % Capacity;
            if (Count == Capacity) tail = (tail + 1) % Capacity;
            else Count++;
        }

        public T Dequeue()
        {
            if (Count == 0) throw new InvalidOperationException("Buffer is empty");
            var item = buffer[tail];
            tail = (tail + 1) % Capacity;
            Count--;
            return item;
        }
    
        // Access elements by logical index (0 = oldest, Count-1 = newest)
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException(nameof(index));
                if (Capacity == 0 || buffer == null) throw new InvalidOperationException();
                return buffer[(tail + index) % Capacity];
            }
        }
    }
}