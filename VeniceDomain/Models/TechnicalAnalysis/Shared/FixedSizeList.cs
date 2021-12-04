using System.Collections;
using System.Collections.Generic;

namespace VeniceDomain.Models.TechnicalAnalysis.Shared
{
    /// <summary>
    /// A list of fixed size
    /// When adding a new element, if the capacity is reached, removes the first element from the start
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class FixedSizedList<T> : IEnumerable<T>
    {
        internal FixedSizedList(int capacity) => Capacity = capacity;

        internal int Capacity { get; }

        internal int Count => List.Count;

        private List<T> List { get; } = new List<T>();

        public void Add(T item)
        {
            if (Count == Capacity)
                List.RemoveAt(0);
            List.Add(item);
        }

        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
