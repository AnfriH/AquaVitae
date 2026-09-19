using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AquaVitae.Common;

/// <summary>
/// Struct wrapper around a nullable <see cref="List{T}"/>.
/// </summary>
public struct OptionalList<T> : IList<T>
{
    public List<T>? BackingList { get; set; }
    
    public IReadOnlyList<T> AsReadOnly() => BackingList ?? (IReadOnlyList<T>)[];
    public IList<T> AsList() => BackingList ?? (IList<T>)Array.Empty<T>();
    
    /// <summary>
    /// Explicitly initializes <see cref="BackingList"/> with a new <see cref="List{T}"/>.
    /// </summary>
    [MemberNotNull(nameof(BackingList))]
    public void Initialize() => BackingList ??= [];
    
    public IEnumerator<T> GetEnumerator() => AsList().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item)
    {
        Initialize();
        BackingList.Add(item);
    }

    public void AddRange(IEnumerable<T> items)
    {
        Initialize();
        BackingList.AddRange(items);
    }

    public void EnsureCapacity(int capacity)
    {
        if (capacity == 0) return;
        Initialize();
        BackingList.EnsureCapacity(capacity);
    }

    public void Clear() => BackingList?.Clear();
    public bool Contains(T item) => BackingList != null && BackingList.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => BackingList?.CopyTo(array, arrayIndex);

    public bool Remove(T item) => BackingList != null && BackingList.Remove(item);

    public int Count => BackingList?.Count ?? 0;

    public bool IsReadOnly => false;
    public int IndexOf(T item) => BackingList?.IndexOf(item) ?? -1;

    public void Insert(int index, T item)
    {
        if (index == 0) Initialize();
        AsList().Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        AsList().RemoveAt(index);
    }

    public T this[int index]
    {
        get => AsList()[index];
        set => AsList()[index] = value;
    }
}