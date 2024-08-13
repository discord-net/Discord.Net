using Discord.Models;

namespace Discord;

/// <summary>
///     A Unicode emoji.
/// </summary>
public sealed class Emoji :
    IEmote,
    IEquatable<Emoji>,
    IIdentifiable<string>,
    IConstructable<Emoji, IEmojiModel>
{
    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    ///     Initializes a new <see cref="Emoji" /> class with the provided Unicode.
    /// </summary>
    /// <param name="unicode">The pure UTF-8 encoding of an emoji.</param>
    public Emoji(string unicode)
    {
        Name = unicode;
    }

    public static Emoji Construct(IDiscordClient client, IEmojiModel model)
        => new(model.Name!);

    public IEmoteModel ToApiModel(IEmoteModel? existing = default) =>
        existing ?? new Models.Json.Emoji {Name = Name};

    string IIdentifiable<string>.Id => Name;
    IdentityDetail IIdentifiable<string>.Detail => IdentityDetail.Entity;

    /// <summary>
    ///     Gets the Unicode representation of this emoji.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="Emoji.Name" />.
    /// </returns>
    public override string ToString() => Name;

    public bool Equals(Emoji? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Emoji other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public static bool operator ==(Emoji? left, Emoji? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Emoji? left, Emoji? right)
    {
        return !Equals(left, right);
    }
}
