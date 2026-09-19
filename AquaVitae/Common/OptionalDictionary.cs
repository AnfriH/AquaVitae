using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace AquaVitae.Common;

/// <summary>
/// Struct wrapper around a nullable <see cref="Dictionary{TKey,TValue}"/>.
/// </summary>
public struct OptionalDictionary<TKey, TValue> : 
    IDictionary<TKey, TValue>
    where TKey : notnull
{
    public Dictionary<TKey, TValue>? BackingDictionary { get; set; }

    public IReadOnlyDictionary<TKey, TValue> AsReadOnly()
    {
        return BackingDictionary ?? (IReadOnlyDictionary<TKey, TValue>)ReadOnlyDictionary<TKey, TValue>.Empty;
    }

    public IDictionary<TKey, TValue> AsDictionary()
    {
        return BackingDictionary ?? (IDictionary<TKey, TValue>)ReadOnlyDictionary<TKey, TValue>.Empty;
    }

    /// <summary>
    /// Explicitly initializes <see cref="BackingDictionary"/> with a new <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    [MemberNotNull(nameof(BackingDictionary))]
    public void Initialize()
    {
        BackingDictionary ??= new Dictionary<TKey, TValue>();
    }
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => AsDictionary().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Initialize();
        ((ICollection<KeyValuePair<TKey, TValue>>)BackingDictionary).Add(item);
    }

    public void Clear()
    {
        BackingDictionary?.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return AsDictionary().Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>?)BackingDictionary)?.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return BackingDictionary != null && ((ICollection<KeyValuePair<TKey, TValue>>)BackingDictionary).Remove(item);
    }

    public int Count => BackingDictionary?.Count ?? 0;
    int ICollection<KeyValuePair<TKey, TValue>>.Count => BackingDictionary?.Count ?? 0;

    public bool IsReadOnly => false;
    
    public void Add(TKey key, TValue value)
    {
        Initialize();
        BackingDictionary.Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
        return BackingDictionary != null && BackingDictionary.ContainsKey(key);
    }
    
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (BackingDictionary != null) return BackingDictionary.TryGetValue(key, out value);
        
        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        return AsDictionary().Remove(key);
    }

    public TValue this[TKey key]
    {
        get => AsDictionary()[key];
        set => AsDictionary()[key] = value;
    }
    
    /// <inheritdoc/>
    /// <remarks>
    /// This collection does not update if <see cref="BackingDictionary"/> is null.
    /// </remarks>
    public ICollection<TKey> Keys => AsDictionary().Keys;
    
    /// <inheritdoc/>
    /// <remarks>
    /// This collection does not update if <see cref="BackingDictionary"/> is null.
    /// </remarks>
    public ICollection<TValue> Values => AsDictionary().Values;
}