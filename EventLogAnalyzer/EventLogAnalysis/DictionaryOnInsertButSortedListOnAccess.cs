#nullable disable

using System.Collections;
using System.Collections.Concurrent;

namespace EventLogAnalysis;

public class DictionaryOnInsertButSortedListOnAccess<T> : IList<T>
{
    // IList(Of T) Interface - Represents a collection of objects that can be individually accessed by index.
    // IDictionary(Of TKey, TValue) Interface - Represents a generic collection of key/value pairs.

    /// <summary>
    /// For speed of insertion, the underlying collection is a dictionary
    /// </summary>
    /// <remarks></remarks>
    private ConcurrentDictionary<string, T> mDictionary = new ConcurrentDictionary<string, T>();

    // Private mDictionary As New Dictionary(Of String, T)

    /// <summary>
    /// Whether it is locked, after which the dictionary is cleared and no further updates are allowed.
    /// </summary>
    /// <remarks></remarks>
    private bool mIsLocked = false;

    /// <summary>
    /// When the items are accessed they will be sorted into a list
    /// </summary>
    /// <remarks></remarks>
    private List<T> mList = new();

    /// <summary>
    /// When items are inserted into the dictionary this marks the list as dirty which means it will need to be recreated
    /// </summary>
    /// <remarks></remarks>
    private bool mListIsDirty = true;

    //private bool mNeedsSort = true;

    private object sortLock = new object();

    public int Count
    {
        get
        {
            if (IsLocked)
            {
                return mSortedList.Count;
            }
            else
            {
                return mDictionary.Count;
            }
        }
    }

    public bool IsLocked
    {
        get
        {
            return mIsLocked;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// Return items in this itemCollection.  Sorting performed if needed.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    private List<T> mSortedList
    {
        get
        {
            if (mListIsDirty)
            {
                lock (sortLock)
                {
                    if (mListIsDirty)
                    {
                        mList = mDictionary.Values.ToList();
                        mList.Sort();
                        mListIsDirty = false;

                        // If it has been locked then clear the dictionary
                        if (IsLocked & mDictionary != null)
                        {
                            mDictionary.Clear();
                            mDictionary = null;
                        }
                    }
                }
            }

            // If mNeedsSort Then
            // mList.Sort()
            // mNeedsSort = False
            // End If
            return mList;
        }
    }

    public T this[int index]
    {
        get
        {
            return mSortedList[index];
        }
        set
        {
            mSortedList[index] = value;
        }
    }

    /// <summary>
    /// Adds a item (using ToString as the key, which may not be desired)
    /// </summary>
    /// <param name="item"></param>
    /// <remarks></remarks>
    //[Obsolete("Not recommended.  Use Add(item, key) or override this to call Add(item, key) instead.")]
    public virtual void Add(T item)
    {
        Add(item, item.ToString());
    }

    /// <summary>
    /// Adds a item (dupicate IDs ignored)
    /// </summary>
    /// <param name="item"></param>
    /// <remarks></remarks>
    public void Add(T item, string key)
    {
        if (!IsLocked)
        {
            // this will do nothing if the item already exists
            if (!mDictionary.ContainsKey(key))
            {
                // 'The list would need to be updated from the dictionary now that the dictionary has changed
                mListIsDirty = true;
                // mDictionary.Add(key, item)
                mDictionary.TryAdd(key, item);
            }
        }
        else
        {
            throw new InvalidOperationException("Cannot add after list is locked.");
        }
    }

    public void Clear()
    {
        mList.Clear();
        mDictionary.Clear();
    }

    public bool Contains(T item)
    {
        return mSortedList.Contains(item);
    }

    public bool ContainsKey(string Key)
    {
        return mDictionary.ContainsKey(Key);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        mList.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return mSortedList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        // is this right after conversion to c#?
        return ((IEnumerable)mList).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return mSortedList.IndexOf(item);
    }

    [Obsolete("Not suported")]
    public void Insert(int index, T item)
    {
        throw new NotSupportedException();
    }

    public void Lock()
    {
        mIsLocked = true;
    }

    [Obsolete("Not suported")]
    public bool Remove(T item)
    {
        throw new NotSupportedException();
    }

    [Obsolete("Not suported")]
    public void RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    public bool RemoveKey(string Key)
    {
        if (!IsLocked)
        {
            mListIsDirty = true;
            // Return mDictionary.Remove(Key)
            T RemovedValue;
            return mDictionary.TryRemove(Key, out RemovedValue);
        }
        else
        {
            throw new InvalidOperationException("Cannot remove after list is locked.");
        }
    }
}