using System.Diagnostics;

namespace Discord;

/// <summary> A footer field for an <see cref="Embed" />. </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct EmbedFooter : IEntityProperties<Models.Json.EmbedFooter>, IConstructable<EmbedFooter, Models.Json.EmbedFooter>
{
    /// <summary>
    ///     The text of the footer field.
    /// </summary>
    /// <returns>
    ///     A string containing the text of the footer field.
    /// </returns>
    public readonly string Text;

    /// <summary>
    ///     The URL of the footer icon.
    /// </summary>
    /// <returns>
    ///     A string containing the URL of the footer icon.
    /// </returns>
    public readonly string? IconUrl;

    /// <summary>
    ///     The proxied URL of the footer icon link.
    /// </summary>
    /// <returns>
    ///     A string containing the proxied URL of the footer icon.
    /// </returns>
    public readonly string? ProxyIconUrl;

    internal EmbedFooter(string text, string? iconUrl, string? proxyIconUrl)
    {
        Text = text;
        IconUrl = iconUrl;
        ProxyIconUrl = proxyIconUrl;
    }

    private string DebuggerDisplay => $"{Text} ({IconUrl})";

    /// <summary>
    ///     Gets the text of the footer field.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="Text" />.
    /// </returns>
    public override string ToString() => Text;

    public static bool operator ==(EmbedFooter? left, EmbedFooter? right)
        => left is null
            ? right is null
            : left.Equals(right);

    public static bool operator !=(EmbedFooter? left, EmbedFooter? right)
        => !(left == right);

    public Models.Json.EmbedFooter ToApiModel(Models.Json.EmbedFooter? existing = default)
    {
        existing ??= new() {Text = Text};

        existing.IconUrl = Optional.FromNullable(IconUrl);
        existing.ProxyIconUrl = Optional.FromNullable(ProxyIconUrl);

        return existing;
    }

    public static EmbedFooter Construct(IDiscordClient client, Models.Json.EmbedFooter model) => throw new NotImplementedException();

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="EmbedFooter" />.
    /// </summary>
    /// <remarks>
    ///     If the object passes is an <see cref="EmbedFooter" />, <see cref="Equals(EmbedFooter?)" /> will be called to
    ///     compare the 2 instances
    /// </remarks>
    /// <param name="obj">The object to compare with the current <see cref="EmbedFooter" /></param>
    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EmbedFooter embedFooter && Equals(embedFooter);

    /// <summary>
    ///     Determines whether the specified <see cref="EmbedFooter" /> is equal to the current <see cref="EmbedFooter" />
    /// </summary>
    /// <param name="embedFooter">The <see cref="EmbedFooter" /> to compare with the current <see cref="EmbedFooter" /></param>
    /// <inheritdoc cref="Object.Equals(object?)" />
    public bool Equals(EmbedFooter? embedFooter)
        => GetHashCode() == embedFooter?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Text, IconUrl, ProxyUrl: ProxyIconUrl).GetHashCode();
}
