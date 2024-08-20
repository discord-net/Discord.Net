using Discord.Models;
using System.Diagnostics;

namespace Discord;

/// <summary>
///     A author field of an <see cref="Embed" />.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct EmbedAuthor : IEntityProperties<Models.Json.EmbedAuthor>,
    IModelConstructable<EmbedAuthor, IEmbedAuthorModel>
{
    /// <summary>
    ///     The name of the author field.
    /// </summary>
    public readonly string Name;

    /// <summary>
    ///     The URL of the author field.
    /// </summary>
    public readonly string? Url;

    /// <summary>
    ///     The icon URL of the author field.
    /// </summary>
    public readonly string? IconUrl;

    /// <summary>
    ///     The proxified icon URL of the author field.
    /// </summary>
    public readonly string? ProxyIconUrl;

    internal EmbedAuthor(string name, string? url, string? iconUrl, string? proxyIconUrl)
    {
        Name = name;
        Url = url;
        IconUrl = iconUrl;
        ProxyIconUrl = proxyIconUrl;
    }

    private string DebuggerDisplay => $"{Name} ({Url})";

    /// <summary>
    ///     Gets the name of the author field.
    /// </summary>
    /// <returns>
    /// </returns>
    public override string ToString() => Name;

    public static bool operator ==(EmbedAuthor? left, EmbedAuthor? right)
        => left is null
            ? right is null
            : left.Equals(right);

    public static bool operator !=(EmbedAuthor? left, EmbedAuthor? right)
        => !(left == right);

    public Models.Json.EmbedAuthor ToApiModel(Models.Json.EmbedAuthor? existing = default)
    {
        existing ??= new Models.Json.EmbedAuthor {Name = Name};

        existing.Url = Optional.FromNullable(Url);
        existing.IconUrl = Optional.FromNullable(IconUrl);
        existing.ProxyIconUrl = Optional.FromNullable(ProxyIconUrl);

        return existing;
    }

    public static EmbedAuthor Construct(IDiscordClient client, IEmbedAuthorModel model) =>
        new(model.Name, model.Url, model.IconUrl, model.ProxyIconUrl);

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="EmbedAuthor" />.
    /// </summary>
    /// <remarks>
    ///     If the object passes is an <see cref="EmbedAuthor" />, <see cref="Equals(EmbedAuthor?)" /> will be called to
    ///     compare the 2 instances
    /// </remarks>
    /// <param name="obj">The object to compare with the current <see cref="EmbedAuthor" /></param>
    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EmbedAuthor embedAuthor && Equals(embedAuthor);

    /// <summary>
    ///     Determines whether the specified <see cref="EmbedAuthor" /> is equal to the current <see cref="EmbedAuthor" />
    /// </summary>
    /// <param name="embedAuthor">The <see cref="EmbedAuthor" /> to compare with the current <see cref="EmbedAuthor" /></param>
    /// <inheritdoc cref="Object.Equals(object?)" />
    public bool Equals(EmbedAuthor? embedAuthor)
        => GetHashCode() == embedAuthor?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Name, Url, IconUrl).GetHashCode();
}
