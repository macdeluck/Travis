using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Travis.Logic.Algorithm
{
    /// <summary>
    /// Simple priority queue collection.
    /// </summary>
    /// <typeparam name="T">Type of collection element.</typeparam>
    public abstract class Heap<T> : ICollection<T>
    {
        private const int InitialCapacity = 0;
        private const int GrowFactor = 2;
        private const int MinGrow = 1;

        private int _capacity;
        private T[] _heap;
        private int _tail;

        /// <summary>
        /// Returns number of elements.
        /// </summary>
        public int Count { get { return _tail; } }

        /// <summary>
        /// Returns collection capacity.
        /// </summary>
        public int Capacity { get { return _capacity; } }

        /// <summary>
        /// Object used to compare elements.
        /// </summary>
        protected Comparer<T> Comparer { get; private set; }

        /// <summary>
        /// Checks if collection is read only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Compares priority of two elements.
        /// </summary>
        /// <param name="x">First element to compare.</param>
        /// <param name="y">Second element to compare.</param>
        protected abstract bool Dominates(T x, T y);

        /// <summary>
        /// Creates heap with default comparer for element type.
        /// </summary>
        protected Heap() : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Creates heap with specified comparer.
        /// </summary>
        /// <param name="comparer">Object used to compare elements.</param>
        protected Heap(Comparer<T> comparer) : this(Enumerable.Empty<T>(), comparer)
        {
        }

        /// <summary>
        /// Creates heap initialised with collection.
        /// </summary>
        /// <param name="collection">Collection used to initially fill priority queue.</param>
        protected Heap(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Creates heap initialised with collection using specified comparer.
        /// </summary>
        /// <param name="collection">Collection used to initially fill priority queue.</param>
        /// <param name="comparer">Object used to compare elements.</param>
        protected Heap(IEnumerable<T> collection, Comparer<T> comparer)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            Comparer = comparer;

            Clear();
            foreach (var item in collection)
            {
                if (Count == Capacity)
                    Grow();

                _heap[_tail++] = item;
            }

            for (int i = Parent(_tail - 1); i >= 0; i--)
                BubbleDown(i);
        }

        /// <summary>
        /// Adds element to collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (Count == Capacity)
                Grow();

            _heap[_tail++] = item;
            BubbleUp(_tail - 1);
        }

        private void BubbleUp(int i)
        {
            if (i == 0 || Dominates(_heap[Parent(i)], _heap[i]))
                return; //correct domination (or root)

            Swap(i, Parent(i));
            BubbleUp(Parent(i));
        }

        /// <summary>
        /// Returns element at top of priority queue.
        /// </summary>
        public T Top()
        {
            if (Count == 0) throw new InvalidOperationException("Heap is empty");
            return _heap[0];
        }

        /// <summary>
        /// Removes top element from priority queue.
        /// </summary>
        public T Dequeue()
        {
            if (Count == 0) throw new InvalidOperationException("Heap is empty");
            T ret = _heap[0];
            _tail--;
            Swap(_tail, 0);
            BubbleDown(0);
            return ret;
        }

        private void BubbleDown(int i)
        {
            int dominatingNode = Dominating(i);
            if (dominatingNode == i) return;
            Swap(i, dominatingNode);
            BubbleDown(dominatingNode);
        }

        private int Dominating(int i)
        {
            int dominatingNode = i;
            dominatingNode = GetDominating(YoungChild(i), dominatingNode);
            dominatingNode = GetDominating(OldChild(i), dominatingNode);

            return dominatingNode;
        }

        private int GetDominating(int newNode, int dominatingNode)
        {
            if (newNode < _tail && !Dominates(_heap[dominatingNode], _heap[newNode]))
                return newNode;
            else
                return dominatingNode;
        }

        private void Swap(int i, int j)
        {
            T tmp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = tmp;
        }

        private static int Parent(int i)
        {
            return (i + 1) / 2 - 1;
        }

        private static int YoungChild(int i)
        {
            return (i + 1) * 2 - 1;
        }

        private static int OldChild(int i)
        {
            return YoungChild(i) + 1;
        }

        private void Grow()
        {
            int newCapacity = _capacity * GrowFactor + MinGrow;
            var newHeap = new T[newCapacity];
            Array.Copy(_heap, newHeap, _capacity);
            _heap = newHeap;
            _capacity = newCapacity;
        }

        /// <summary>
        /// Returns collection enumerator.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _heap.Take(Count).GetEnumerator();
        }

        /// <summary>
        /// Returns collection enumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clears collection.
        /// </summary>
        public void Clear()
        {
            _tail = 0;
            _capacity = InitialCapacity;
            _heap = new T[_capacity];
        }

        /// <summary>
        /// Checks if collection contains element.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        public bool Contains(T item)
        {
            return ((IEnumerable<T>)this).Contains(item);
        }

        /// <summary>
        /// Copies collection elements to array.
        /// </summary>
        /// <param name="array">Array to copy elements to.</param>
        /// <param name="arrayIndex">Start index of array where elements should be placed.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < arrayIndex + Count; i++)
                array[i] = _heap[i];
        }

        /// <summary>
        /// Not supported for heap.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Heap in which elements priorities are ascending.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public class MaxHeap<T> : Heap<T>
    {
        /// <summary>
        /// Creates new instance of class with default comparer.
        /// </summary>
        public MaxHeap()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Creates heap with specified comparer.
        /// </summary>
        /// <param name="comparer">Object used to compare elements.</param>
        public MaxHeap(Comparer<T> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Creates heap initialised with collection using specified comparer.
        /// </summary>
        /// <param name="collection">Collection used to initially fill priority queue.</param>
        /// <param name="comparer">Object used to compare elements.</param>
        public MaxHeap(IEnumerable<T> collection, Comparer<T> comparer)
            : base(collection, comparer)
        {
        }

        /// <summary>
        /// Creates heap initialised with collection.
        /// </summary>
        /// <param name="collection">Collection used to initially fill priority queue.</param>
        public MaxHeap(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// Compares priority of two elements.
        /// </summary>
        /// <param name="x">First element to compare.</param>
        /// <param name="y">Second element to compare.</param>
        protected override bool Dominates(T x, T y)
        {
            return Comparer.Compare(x, y) >= 0;
        }
    }

    /// <summary>
    /// Heap in which elements priorities are descending.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public class MinHeap<T> : Heap<T>
    {
        /// <summary>
        /// Creates new instance of class with default comparer.
        /// </summary>
        public MinHeap()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Creates heap with specified comparer.
        /// </summary>
        /// <param name="comparer">Object used to compare elements.</param>
        public MinHeap(Comparer<T> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Creates heap initialised with collection.
        /// </summary>
        /// <param name="collection">Collection used to initially fill priority queue.</param>
        public MinHeap(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// Creates heap initialised with collection using specified comparer.
        /// </summary>
        /// <param name="collection">Collection used to initially fill priority queue.</param>
        /// <param name="comparer">Object used to compare elements.</param>
        public MinHeap(IEnumerable<T> collection, Comparer<T> comparer)
            : base(collection, comparer)
        {
        }

        /// <summary>
        /// Compares priority of two elements.
        /// </summary>
        /// <param name="x">First element to compare.</param>
        /// <param name="y">Second element to compare.</param>
        protected override bool Dominates(T x, T y)
        {
            return Comparer.Compare(x, y) <= 0;
        }
    }
}
