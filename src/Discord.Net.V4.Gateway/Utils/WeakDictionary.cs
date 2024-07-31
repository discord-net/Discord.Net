using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Discord.Gateway;

internal sealed partial class WeakDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
#if NET8_0_OR_GREATER
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "UnsafeGetTargetAndDependent")]
    private static extern object? UnsafeGetTargetAndDependent(ref DependentHandle handle);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe object? UnsafeGetTargetAndDependent(ref DependentHandle handle)
        => Unsafe.As<IntPtr, object>(ref *(IntPtr*)Unsafe.As<DependentHandle, IntPtr>(ref handle));
#endif

    private sealed class ValueHolder
    {
        private readonly WeakDictionary<TKey, TValue> _dictionary;
        private readonly TKey _key;
        private DependentHandle _handle;

        public bool TryGetValue([MaybeNullWhen(false)] out TValue value)
        {
            value = Unsafe.As<TValue>(UnsafeGetTargetAndDependent(ref _handle));

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

    // ReSharper disable once InconsistentlySynchronizedField
    private void UnsafeRemove(TKey key)
        => _dictionary.Remove(key);
}
