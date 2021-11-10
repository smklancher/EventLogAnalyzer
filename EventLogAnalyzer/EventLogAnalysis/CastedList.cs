#nullable disable

using System.Collections;

namespace EventLogAnalysis
{
    public static class ListExtensions
    {
        public static IList<TTo> CastList<TFrom, TTo>(this IList<TFrom> list)
        {
            return new CastedList<TTo, TFrom>(list);
        }

        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }
    }

    /// <summary>
    /// https://stackoverflow.com/a/30662440/221018
    /// </summary>
    public class CastedEnumerator<TTo, TFrom> : IEnumerator<TTo>
    {
        public IEnumerator<TFrom> BaseEnumerator;

        public CastedEnumerator(IEnumerator<TFrom> baseEnumerator)
        {
            BaseEnumerator = baseEnumerator;
        }

        // IEnumerator
        object IEnumerator.Current { get { return BaseEnumerator.Current; } }

        // IEnumerator<>
        public TTo Current { get { return (TTo)(object)BaseEnumerator.Current; } }

        // IDisposable
        public void Dispose() { BaseEnumerator.Dispose(); }

        public bool MoveNext()
        {
            return BaseEnumerator.MoveNext();
        }

        public void Reset()
        {
            BaseEnumerator.Reset();
        }
    }

    public class CastedList<TTo, TFrom> : IList<TTo>
    {
        public IList<TFrom> BaseList;

        public CastedList(IList<TFrom> baseList)
        {
            BaseList = baseList;
        }

        // ICollection
        public int Count { get { return BaseList.Count; } }

        public bool IsReadOnly { get { return BaseList.IsReadOnly; } }

        // IList
        public TTo this[int index]
        {
            get { return (TTo)(object)BaseList[index]; }
            set { BaseList[index] = (TFrom)(object)value; }
        }

        public void Add(TTo item)
        {
            BaseList.Add((TFrom)(object)item);
        }

        public void Clear()
        {
            BaseList.Clear();
        }

        public bool Contains(TTo item)
        {
            return BaseList.Contains((TFrom)(object)item);
        }

        public void CopyTo(TTo[] array, int arrayIndex)
        {
            BaseList.CopyTo((TFrom[])(object)array, arrayIndex);
        }

        // IEnumerable
        IEnumerator IEnumerable.GetEnumerator() { return BaseList.GetEnumerator(); }

        // IEnumerable<>
        public IEnumerator<TTo> GetEnumerator() { return new CastedEnumerator<TTo, TFrom>(BaseList.GetEnumerator()); }

        public int IndexOf(TTo item)
        {
            return BaseList.IndexOf((TFrom)(object)item);
        }

        public void Insert(int index, TTo item)
        {
            BaseList.Insert(index, (TFrom)(object)item);
        }

        public bool Remove(TTo item)
        {
            return BaseList.Remove((TFrom)(object)item);
        }

        public void RemoveAt(int index)
        {
            BaseList.RemoveAt(index);
        }
    }
}