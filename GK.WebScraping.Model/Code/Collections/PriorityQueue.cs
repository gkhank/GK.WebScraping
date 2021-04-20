using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Model.Code.Collections
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    namespace GK.WebScraping.Model.Collection
    {
        public class PriorityQueue<T>
        {
            IComparer<T> comparer;

            public int Capacity { get; }

            T[] heap;
            public int Count { get; private set; }
            public PriorityQueue() : this(null) { }
            public PriorityQueue(int capacity) : this(capacity, null) { }
            public PriorityQueue(IComparer<T> comparer) : this(16, comparer) { }
            public PriorityQueue(int capacity, IComparer<T> comparer)
            {
                this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
                this.Capacity = capacity;
                this.heap = new T[capacity];
            }
            public void Enqueue(T v)
            {
                if (this.Count >= this.heap.Length) Array.Resize(ref this.heap, this.Count * 2);
                this.heap[this.Count] = v;
                this.SiftUp(this.Count++);
            }
            public T Dequeue()
            {
                var v = this.Peek();
                this.heap[0] = this.heap[--this.Count];
                if (this.Count > 0) this.SiftDown(0);
                return v;
            }
            public T Peek()
            {
                if (Count > 0) return this.heap[0];
                throw new InvalidOperationException("Priority queue is empty");
            }
            void SiftUp(int n)
            {
                var v = heap[n];
                for (var n2 = n / 2; n > 0 && this.comparer.Compare(v, this.heap[n2]) > 0; n = n2, n2 /= 2) this.heap[n] = this.heap[n2];
                this.heap[n] = v;
            }
            void SiftDown(int n)
            {
                var v = heap[n];
                for (var n2 = n * 2; n2 < this.Count; n = n2, n2 *= 2)
                {
                    if (n2 + 1 < this.Count && this.comparer.Compare(this.heap[n2 + 1], this.heap[n2]) > 0) n2++;
                    if (this.comparer.Compare(v, this.heap[n2]) >= 0) break;
                    this.heap[n] = this.heap[n2];
                }
                this.heap[n] = v;
            }
        }
    }
}

