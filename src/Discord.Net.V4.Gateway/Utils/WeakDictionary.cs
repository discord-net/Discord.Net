using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Discord.Gateway;

internal sealed partial class WeakDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    private sealed class ValueHolder
    {
        private readonly WeakDictionary<TKey, TValue> _dictionary;
        private readonly TKey _key;
        private DependentHandle _handle;

        public bool TryGetValue([MaybeNullWhen(false)] out TValue value)
        {
            value = Unsafe.As<TValue>(DependantHandleUtils.UnsafeGetTargetAndDependent(ref _handle));

            if (value is not null) return true;

            _dictionary.UnsafeRemove(_key);
            return false;
        }

        public ValueHolder(WeakDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            _dictionary = dictionary;
            _key = key;
            _handle = new(value, this);
        }

        ~ValueHolder()
        {
            _dictionary.Remove(_key);
        }
    }

    // ReSharper disable once InconsistentlySynchronizedField
    public int Count => _dictionary.Count;


    private readonly Dictionary<TKey, WeakReference<ValueHolder>> _dictionary = new();
    private readonly object _syncRoot = new();

    ~WeakDictionary()
    {
        lock (_syncRoot)
        {
            _dictionary.Clear();
        }
    }

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
    {
        lock (_syncRoot)
        {
            if (
                _dictionary.TryGetValue(key, out var weakRef) &&
                weakRef.TryGetTarget(out var holder) &&
                holder.TryGetValue(out var value)
            ) return value;

            value = factory(key);

            _dictionary[key] = new(
                new(this, key, value)
            );

            return value;
        }
    }

    public TValue GetOrAdd<T>(TKey key, Func<T, TKey, TValue> factory, T state)
    {
        lock (_syncRoot)
        {
            if (
                _dictionary.TryGetValue(key, out var weakRef) &&
                weakRef.TryGetTarget(out var holder) &&
                holder.TryGetValue(out var value)
            ) return value;

            value = factory(state, key);

            _dictionary[key] = new(
                new(this, key, value)
            );

            return value;
        }
    }

    public void Add(TKey key, TValue value)
    {
        lock (_syncRoot)
        {
            var holder = new ValueHolder(this, key, value);
            _dictionary.Add(key, new(holder));
        }
    }

    public bool Remove(TKey key)
    {
        lock (_syncRoot)
        {
            return _dictionary.Remove(key);
        }
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TValue? FirstOrDefault(TValue? defaultValue = null)
    {
        if (Count == 0)
            return defaultValue;

        lock (_syncRoot)
        {
            if (Count == 0)
                return defaultValue;

            // TODO: FirstOrDefault compared to copy out
            var valueHolder = new WeakReference<ValueHolder>[1];
            _dictionary.Values.CopyTo(valueHolder, 0);

            if (valueHolder[0].TryGetTarget(out var holder) && holder.TryGetValue(out var value))
                return value;

            GC.KeepAlive(this);

            return defaultValue;
        }
    }

    // ReSharper disable once InconsistentlySynchronizedField
    private void UnsafeRemove(TKey key)
        => _dictionary.Remove(key);
}
