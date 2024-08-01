using Discord.Models;

namespace Discord;

/// <summary>
///     A Unicode emoji.
/// </summary>
public readonly struct Emoji :
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

    /// <inheritdoc />
    public override int GetHashCode() => Name.GetHashCode();

    public bool Equals(Emoji other) => Name == other.Name;

    public override bool Equals(object? obj) => obj is Emoji other && Equals(other);

    public static bool operator ==(Emoji left, Emoji right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Emoji left, Emoji right)
    {
        return !(left == right);
    }
}
