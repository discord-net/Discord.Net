namespace Discord;

public readonly struct BucketInfo(ScopeType type, ulong value)
{
    public readonly ScopeType Type = type;
    public readonly ulong Value = value;

    public static implicit operator BucketInfo((ScopeType type, ulong value) tuple) => new(tuple.type, tuple.value);
}
