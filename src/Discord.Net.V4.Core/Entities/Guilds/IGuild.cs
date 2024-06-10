using Discord.Models.Json;
using System.Globalization;

namespace Discord;

public interface IGuild : IPartialGuild, IDeletable, IModifiable<ModifyGuildProperties, ModifyGuildParams>
{
    /// <summary>
    ///     Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are
    ///     automatically moved to the AFK voice channel.
    /// </summary>
    /// <returns>
    ///     An <see langword="int" /> representing the amount of time in seconds for a user to be marked as inactive
    ///     and moved into the AFK voice channel.
    /// </returns>
    int AFKTimeout { get; }

    /// <summary>
    ///     Gets a value that indicates whether this guild has the widget enabled.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this guild has a widget enabled; otherwise <see langword="false" />.
    /// </returns>
    bool IsWidgetEnabled { get; }

    /// <summary>
    ///     Gets the default message notifications for users who haven't explicitly set their notification settings.
    /// </summary>
    DefaultMessageNotifications DefaultMessageNotifications { get; }

    /// <summary>
    ///     Gets the level of Multi-Factor Authentication requirements a user must fulfill before being allowed to
    ///     perform administrative actions in this guild.
    /// </summary>
    /// <returns>
    ///     The level of MFA requirement.
    /// </returns>
    MfaLevel MfaLevel { get; }

    /// <summary>
    ///     Gets the level of requirements a user must fulfill before being allowed to post messages in this guild.
    /// </summary>
    /// <returns>
    ///     The level of requirements.
    /// </returns>
    new VerificationLevel VerificationLevel { get; }

    /// <summary>
    ///     Gets the level of content filtering applied to user's content in a Guild.
    /// </summary>
    /// <returns>
    ///     The level of explicit content filtering.
    /// </returns>
    ExplicitContentFilterLevel ExplicitContentFilter { get; }

    /// <summary>
    ///     Gets the URL of this guild's icon.
    /// </summary>
    /// <returns>
    ///     A URL pointing to the guild's icon; <see langword="null" /> if none is set.
    /// </returns>
    string? IconUrl => CDN.GetGuildIconUrl(Client.Config, Id, IconId);

    /// <summary>
    ///     Gets the URL of this guild's splash image.
    /// </summary>
    /// <returns>
    ///     A URL pointing to the guild's splash image; <see langword="null" /> if none is set.
    /// </returns>
    string? SplashUrl => CDN.GetGuildSplashUrl(Client.Config, Id, SplashId);

    /// <summary>
    ///     Gets the ID of this guild's discovery splash image.
    /// </summary>
    /// <returns>
    ///     An identifier for the discovery splash image; <see langword="null" /> if none is set.
    /// </returns>
    string DiscoverySplashId { get; }

    /// <summary>
    ///     Gets the URL of this guild's discovery splash image.
    /// </summary>
    /// <returns>
    ///     A URL pointing to the guild's discovery splash image; <see langword="null" /> if none is set.
    /// </returns>
    string DiscoverySplashUrl { get; }

    ILoadableEntity<ulong, IVoiceChannel> AFKChannel { get; }
    ILoadableEntity<ulong, ITextChannel> WidgetChannel { get; }
    ILoadableEntity<ulong, ITextChannel> SafetyAlertsChannel { get; }
    ILoadableEntity<ulong, ITextChannel> SystemChannel { get; }
    ILoadableEntity<ulong, ITextChannel> RulesChannel { get; }
    ILoadableEntity<ulong, ITextChannel> PublicUpdatesChannel { get; }
    ILoadableEntity<ulong, IGuildUser> Owner { get; }

    ulong? ApplicationId { get; }

    [Obsolete("Deprecated in the Discord API")]
    string? VoiceRegion { get; }

    ILoadableEntity<ulong, IRole> EveryoneRole { get; }

    ILoadableEntityEnumerable<ulong, IGuildEmote> Emotes { get; }

    ILoadableEntityEnumerable<ulong, IGuildSticker> Stickers { get; }

    new GuildFeatures Features { get; }

    ILoadableEntityEnumerable<ulong, IRole> Roles { get; }

    /// <summary>
    ///     Gets the tier of guild boosting in this guild.
    /// </summary>
    /// <returns>
    ///     The tier of guild boosting in this guild.
    /// </returns>
    PremiumTier PremiumTier { get; }

    /// <summary>
    ///     Gets the URL of this guild's banner image.
    /// </summary>
    /// <returns>
    ///     A URL pointing to the guild's banner image; <see langword="null" /> if none is set.
    /// </returns>
    string BannerUrl { get; }

    /// <summary>
    ///     Gets the flags for the types of system channel messages that are disabled.
    /// </summary>
    /// <returns>
    ///     The flags for the types of system channel messages that are disabled.
    /// </returns>
    SystemChannelFlags SystemChannelFlags { get; }

    /// <summary>
    ///     Gets the number of premium subscribers of this guild.
    /// </summary>
    /// <remarks>
    ///     This is the number of users who have boosted this guild.
    /// </remarks>
    /// <returns>
    ///     The number of premium subscribers of this guild; <see langword="null" /> if not available.
    /// </returns>
    new int PremiumSubscriptionCount { get; }

    /// <summary>
    ///     Gets the maximum number of presences for the guild.
    /// </summary>
    /// <returns>
    ///     The maximum number of presences for the guild.
    /// </returns>
    int? MaxPresences { get; }

    /// <summary>
    ///     Gets the maximum number of members for the guild.
    /// </summary>
    /// <returns>
    ///     The maximum number of members for the guild.
    /// </returns>
    int? MaxMembers { get; }

    /// <summary>
    ///     Gets the maximum amount of users in a video channel.
    /// </summary>
    /// <returns>
    ///     The maximum amount of users in a video channel.
    /// </returns>
    int? MaxVideoChannelUsers { get; }

    /// <summary>
    ///     Gets the maximum amount of users in a stage video channel.
    /// </summary>
    /// <returns>
    ///     The maximum amount of users in a stage video channel.
    /// </returns>
    int? MaxStageVideoChannelUsers { get; }

    /// <summary>
    ///     Gets the approximate number of members in this guild.
    /// </summary>
    /// <remarks>
    ///     Only available when getting a guild via REST when `with_counts` is true.
    /// </remarks>
    /// <returns>
    ///     The approximate number of members in this guild.
    /// </returns>
    int? ApproximateMemberCount { get; }

    /// <summary>
    ///     Gets the approximate number of non-offline members in this guild.
    /// </summary>
    /// <remarks>
    ///     Only available when getting a guild via REST when `with_counts` is true.
    /// </remarks>
    /// <returns>
    ///     The approximate number of non-offline members in this guild.
    /// </returns>
    int? ApproximatePresenceCount { get; }

    /// <summary>
    ///     Gets the max bitrate for voice channels in this guild.
    /// </summary>
    /// <returns>
    ///     A <see cref="int" /> representing the maximum bitrate value allowed by Discord in this guild.
    /// </returns>
    int MaxBitrate { get; }

    /// <summary>
    ///     Gets the preferred locale of this guild in IETF BCP 47
    ///     language tag format.
    /// </summary>
    /// <returns>
    ///     The preferred locale of the guild in IETF BCP 47
    ///     language tag format.
    /// </returns>
    string PreferredLocale { get; }

    /// <summary>
    ///     Gets the NSFW level of this guild.
    /// </summary>
    /// <returns>
    ///     The NSFW level of this guild.
    /// </returns>
    new NsfwLevel NsfwLevel { get; }

    /// <summary>
    ///     Gets the preferred culture of this guild.
    /// </summary>
    /// <returns>
    ///     The preferred culture information of this guild.
    /// </returns>
    CultureInfo PreferredCulture { get; }

    /// <summary>
    ///     Gets whether the guild has the boost progress bar enabled.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the boost progress bar is enabled; otherwise <see langword="false" />.
    /// </returns>
    bool IsBoostProgressBarEnabled { get; }

    /// <summary>
    ///     Gets the upload limit in bytes for this guild. This number is dependent on the guild's boost status.
    /// </summary>
    ulong MaxUploadLimit { get; }

    VerificationLevel? IPartialGuild.VerificationLevel => VerificationLevel;
    GuildFeatures? IPartialGuild.Features => Features;
    NsfwLevel? IPartialGuild.NsfwLevel => NsfwLevel;
    int? IPartialGuild.PremiumSubscriptionCount => PremiumSubscriptionCount;
}
