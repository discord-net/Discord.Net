namespace Discord;

/// <summary>
///     Represents a primary guild object.
/// </summary>
public readonly struct PrimaryGuild
{
    /// <summary>
    ///     Gets the id of the user's primary guild.
    /// </summary>
    public ulong? GuildId { get; }

    /// <summary>
    ///     Gets whether the user is displaying the primary guild's server tag.
    /// </summary>
    /// <remarks>
    ///     This property will be <see langword="null"/> if the system clears the identity, e.g. because the server no longer supports tags.
    /// </remarks>
    public bool? IdentityEnabled { get; }

    /// <summary>
    ///     Gets the text of the user's server tag.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    ///     Gets the hash of the guild tag badge.
    /// </summary>
    public string BadgeHash { get; }

    internal PrimaryGuild(ulong? guildId, bool? identityEnabled, string tag, string badgeHash)
    {
        GuildId = guildId;
        IdentityEnabled = identityEnabled;
        Tag = tag;
        BadgeHash = badgeHash;
    }

    /// <summary>
    ///     Gets the url for the tag badge.
    /// </summary>
    public string GetBadgeUrl()
        => (GuildId is null || BadgeHash is null)
            ? null
            : CDN.GetGuildTagBadgeUrl(GuildId.Value, BadgeHash);
}
