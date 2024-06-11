namespace Discord;

/// <summary>
///     Specifies the type of embed.
/// </summary>
public readonly struct EmbedType(string? value) : IEquatable<EmbedType>
{
    public static readonly EmbedType Rich = new("rich");
    public static readonly EmbedType Image = new("image");
    public static readonly EmbedType Video = new("video");
    public static readonly EmbedType Gifv = new("gifv");
    public static readonly EmbedType Article = new("article");
    public static readonly EmbedType Link = new("link");
    public static readonly EmbedType Unspecified = new(null);

    public readonly string? Value = value;

    public bool Equals(EmbedType other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is EmbedType other && Equals(other);

    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

    public static bool operator ==(EmbedType left, EmbedType right) => left.Equals(right);

    public static bool operator !=(EmbedType left, EmbedType right) => !left.Equals(right);

    public static implicit operator string?(EmbedType type) => type.Value;
    public static implicit operator EmbedType(string? value) => new(value);
}
