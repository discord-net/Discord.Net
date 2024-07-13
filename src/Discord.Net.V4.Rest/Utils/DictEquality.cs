namespace Discord.Rest;

public sealed class DictEquality<TKey, TValue> : IEqualityComparer<IDictionary<TKey, TValue>>
    where TValue : IEquatable<TValue>
{
    private static DictEquality<TKey, TValue>? _instance;

    public static DictEquality<TKey, TValue> Instance => _instance ??= new DictEquality<TKey, TValue>();

    public bool Equals(IDictionary<TKey, TValue>? x, IDictionary<TKey, TValue>? y)
    {
        if (ReferenceEquals(x, y)) return true;

        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        if(x.Count != y.Count) return false;

        foreach(var (k,v) in x)
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if(!y.TryGetValue(k, out var v2) || !(v2?.Equals(v) ?? v is null))
                return false;

        return true;
    }

    public int GetHashCode(IDictionary<TKey, TValue> obj)
    {
        return HashCode.Combine(obj.Keys, obj.Values, obj.Count, obj.IsReadOnly);
    }
}
