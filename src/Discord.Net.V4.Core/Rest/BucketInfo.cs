namespace Discord;

public readonly struct BucketInfo(ScopeType type, ulong value) : IEquatable<BucketInfo>
{
    public readonly ScopeType Type = type;
    public readonly ulong Value = value;

    public static implicit operator BucketInfo((ScopeType type, ulong value) tuple) => new(tuple.type, tuple.value);

    public bool Equals(BucketInfo other) => Type == other.Type && Value == other.Value;

    public override bool Equals(object? obj) => obj is BucketInfo other && Equals(other);

    public override int GetHashCode() => HashCode.Combine((int)Type, Value);

    public static bool operator ==(BucketInfo left, BucketInfo right) => left.Equals(right);

    public static bool operator !=(BucketInfo left, BucketInfo right) => !left.Equals(right);
}
