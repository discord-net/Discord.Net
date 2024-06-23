namespace Discord;

/// <summary>
///     Specifies the type of mentions that will be notified from the message content.
/// </summary>
public readonly struct AllowedMentionTypes(HashSet<string> values) : IEquatable<AllowedMentionTypes>
{
    public static readonly AllowedMentionTypes None = new((string[]) []);
    public static readonly AllowedMentionTypes Roles = new("roles");
    public static readonly AllowedMentionTypes Users = new("users");
    public static readonly AllowedMentionTypes Everyone = new("everyone");
    public static readonly AllowedMentionTypes All = Roles | Users | Everyone;

    public readonly HashSet<string> Values = values;

    public AllowedMentionTypes(params string[] values) : this(values.ToHashSet()) { }

    public static implicit operator AllowedMentionTypes(string value) => new(value);

    public static AllowedMentionTypes operator |(AllowedMentionTypes a, AllowedMentionTypes b) =>
        new((string[]) [..a.Values, ..b.Values]);

    public bool Equals(AllowedMentionTypes other) => Values.Equals(other.Values);

    public override bool Equals(object? obj) => obj is AllowedMentionTypes other && Equals(other);

    public override int GetHashCode() => Values.GetHashCode();

    public static bool operator ==(AllowedMentionTypes left, AllowedMentionTypes right) => left.Equals(right);

    public static bool operator !=(AllowedMentionTypes left, AllowedMentionTypes right) => !left.Equals(right);
}
