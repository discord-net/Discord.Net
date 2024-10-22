using System.Collections;

namespace Discord.Net.Hanz.Utils.Bakery;

public sealed class ImmutableEquatableArray<T> : 
    IEquatable<ImmutableEquatableArray<T>>, 
    IReadOnlyList<T>
    where T : IEquatable<T>
{
    public static ImmutableEquatableArray<T> Empty { get; } = new ImmutableEquatableArray<T>(Array.Empty<T>());

    private readonly T[] _values;
    public T this[int index] => _values[index];
    public int Count => _values.Length;

    public ImmutableEquatableArray(IEnumerable<T> values)
        => _values = values.ToArray();

    public ImmutableEquatableArray<T> Add(T value)
    {
        if (Count == 0)
            return new ImmutableEquatableArray<T>([value]);

        return new ImmutableEquatableArray<T>(
            [.._values, value]
        );
    }

    public ImmutableEquatableArray<T> AddRange(IEnumerable<T> values)
    {
        if (Count == 0)
            return new ImmutableEquatableArray<T>(values);

        return new ImmutableEquatableArray<T>(
            [.._values, ..values]
        );
    }

    public ImmutableEquatableArray<T> AddRange(params T[] values)
        => AddRange((IEnumerable<T>) values);

    public bool Equals(ImmutableEquatableArray<T>? other)
        => other != null && ((ReadOnlySpan<T>) _values).SequenceEqual(other._values);

    public override bool Equals(object? obj)
        => obj is ImmutableEquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        int hash = 0;
        foreach (T value in _values)
        {
            hash = System.HashCode.Combine(hash, value is null ? 0 : value.GetHashCode());
        }

        return hash;
    }

    public Enumerator GetEnumerator() => new Enumerator(_values);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>) _values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

    public struct Enumerator
    {
        private readonly T[] _values;
        private int _index;

        internal Enumerator(T[] values)
        {
            _values = values;
            _index = -1;
        }

        public bool MoveNext()
        {
            int newIndex = _index + 1;

            if ((uint) newIndex < (uint) _values.Length)
            {
                _index = newIndex;
                return true;
            }

            return false;
        }

        public readonly T Current => _values[_index];
    }
}

internal static class ImmutableEquatableArray
{
    public static ImmutableEquatableArray<T> ToImmutableEquatableArray<T>(this IEnumerable<T> values)
        where T : IEquatable<T>
        => new(values);
}