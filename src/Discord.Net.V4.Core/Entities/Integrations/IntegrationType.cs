namespace Discord;

public readonly struct IntegrationType(string name) : IEquatable<IntegrationType>
{
    public static readonly IntegrationType Twitch = new("twitch");
    public static readonly IntegrationType Youtube = new("youtube");
    public static readonly IntegrationType Discord = new("discord");
    public static readonly IntegrationType GuildSubscription = new("guild_subscription");

    public readonly string Name = name;

    public bool Equals(IntegrationType other) => Name == other.Name;

    public override bool Equals(object? obj) => obj is IntegrationType other && Equals(other);

    public override int GetHashCode() => Name.GetHashCode();

    public static bool operator ==(IntegrationType left, IntegrationType right) => left.Equals(right);

    public static bool operator !=(IntegrationType left, IntegrationType right) => !left.Equals(right);

    public static implicit operator string(IntegrationType type) => type.Name;
    public static implicit operator IntegrationType(string name) => new(name);
}
