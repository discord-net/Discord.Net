using Model = Discord.API.AuditLogs.GuildInfoAuditLogModel;

namespace Discord.Rest;

/// <summary>
///     Represents information for a guild.
/// </summary>
public struct GuildInfo
{
    internal GuildInfo(Model model, IUser owner)
    {
        Owner = owner;

        Name = model.Name;
        AfkTimeout = model.AfkTimeout.GetValueOrDefault();
        IsEmbeddable = model.IsEmbeddable;
        DefaultMessageNotifications = model.DefaultMessageNotifications;
        MfaLevel = model.MfaLevel;
        Description = model.Description;
        PreferredLocale = model.PreferredLocale;
        IconHash = model.IconHash;
        OwnerId = model.OwnerId;
        AfkChannelId = model.AfkChannelId;
        ApplicationId = model.ApplicationId;
        BannerId = model.Banner;
        DiscoverySplashId = model.DiscoverySplash;
        EmbedChannelId = model.EmbeddedChannelId;
        ExplicitContentFilter = model.ExplicitContentFilterLevel;
        IsBoostProgressBarEnabled = model.ProgressBarEnabled;
        NsfwLevel = model.NsfwLevel;
        PublicUpdatesChannelId = model.PublicUpdatesChannelId;
        RegionId = model.RegionId;
        RulesChannelId = model.RulesChannelId;
        SplashId = model.Splash;
        SystemChannelFlags = model.SystemChannelFlags;
        SystemChannelId = model.SystemChannelId;
        VanityURLCode = model.VanityUrl;
        VerificationLevel = model.VerificationLevel;
    }

    /// <inheritdoc cref="IGuild.DiscoverySplashId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string DiscoverySplashId { get; }

    /// <inheritdoc cref="IGuild.SplashId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string SplashId { get; }

    /// <inheritdoc cref="IGuild.RulesChannelId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? RulesChannelId { get; }

    /// <inheritdoc cref="IGuild.PublicUpdatesChannelId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? PublicUpdatesChannelId { get; }

    /// <inheritdoc cref="IGuild.OwnerId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? OwnerId { get; }

    /// <inheritdoc cref="IGuild.ApplicationId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? ApplicationId { get; }

    /// <inheritdoc cref="IGuild.BannerId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string BannerId { get; }

    /// <inheritdoc cref="IGuild.VanityURLCode"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string VanityURLCode { get; }

    /// <inheritdoc cref="IGuild.SystemChannelFlags"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public SystemChannelMessageDeny? SystemChannelFlags { get; }

    /// <inheritdoc cref="IGuild.Description"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string Description { get; }

    /// <inheritdoc cref="IGuild.PreferredLocale"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string PreferredLocale { get; }

    /// <inheritdoc cref="IGuild.NsfwLevel"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public NsfwLevel? NsfwLevel { get; }

    /// <inheritdoc cref="IGuild.IsBoostProgressBarEnabled"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public bool? IsBoostProgressBarEnabled { get; }

    /// <summary>
    ///     Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are
    ///     automatically moved to the AFK voice channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the amount of time in seconds for a user to be marked as inactive
    ///     and moved into the AFK voice channel.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public int? AfkTimeout { get; }
    /// <summary>
    ///     Gets the default message notifications for users who haven't explicitly set their notification settings.
    /// </summary>
    /// <returns>
    ///     The default message notifications setting of this guild.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public DefaultMessageNotifications? DefaultMessageNotifications { get; }
    /// <summary>
    ///     Gets the ID of the AFK voice channel for this guild.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the AFK voice channel; <see langword="null" /> if
    ///     none is set.
    /// </returns>
    public ulong? AfkChannelId { get; }
    /// <summary>
    ///     Gets the name of this guild.
    /// </summary>
    /// <returns>
    ///     A string containing the name of this guild.
    /// </returns>
    public string Name { get; }
    /// <summary>
    ///     Gets the ID of the region hosting this guild's voice channels.
    /// </summary>
    public string RegionId { get; }
    /// <summary>
    ///     Gets the ID of this guild's icon.
    /// </summary>
    /// <returns>
    ///     A string containing the identifier for the splash image; <see langword="null" /> if none is set.
    /// </returns>
    public string IconHash { get; }
    /// <summary>
    ///     Gets the level of requirements a user must fulfill before being allowed to post messages in this guild.
    /// </summary>
    /// <returns>
    ///     The level of requirements.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public VerificationLevel? VerificationLevel { get; }
    /// <summary>
    ///     Gets the owner of this guild.
    /// </summary>
    /// <returns>
    ///     A user object representing the owner of this guild.
    /// </returns>
    public IUser Owner { get; }
    /// <summary>
    ///     Gets the level of Multi-Factor Authentication requirements a user must fulfill before being allowed to
    ///     perform administrative actions in this guild.
    /// </summary>
    /// <returns>
    ///     The level of MFA requirement.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public MfaLevel? MfaLevel { get; }
    /// <summary>
    ///     Gets the level of content filtering applied to user's content in a Guild.
    /// </summary>
    /// <returns>
    ///     The level of explicit content filtering.
    /// </returns>
    public ExplicitContentFilterLevel? ExplicitContentFilter { get; }
    /// <summary>
    ///     Gets the ID of the channel where system messages are sent.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the channel where system
    ///     messages are sent; <see langword="null" /> if none is set.
    /// </returns>
    public ulong? SystemChannelId { get; }
    /// <summary>
    ///     Gets the ID of the widget embed channel of this guild.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the embedded channel found within the
    ///     widget settings of this guild; <see langword="null" /> if none is set.
    /// </returns>
    public ulong? EmbedChannelId { get; }
    /// <summary>
    ///     Gets a value that indicates whether this guild is embeddable (i.e. can use widget).
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this guild can be embedded via widgets; otherwise <see langword="false" />.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public bool? IsEmbeddable { get; }
}
