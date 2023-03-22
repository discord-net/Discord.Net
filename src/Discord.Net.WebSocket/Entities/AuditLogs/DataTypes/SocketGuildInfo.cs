using Model = Discord.API.AuditLogs.GuildInfoAuditLogModel;

namespace Discord.WebSocket;

/// <summary>
///     Represents information for a guild.
/// </summary>
public class SocketGuildInfo
{
    internal static SocketGuildInfo Create(Model model)
    {
        return new()
        {
            Name = model.Name,
            AfkTimeout = model.AfkTimeout.GetValueOrDefault(),
            IsEmbeddable = model.IsEmbeddable,
            DefaultMessageNotifications = model.DefaultMessageNotifications,
            MfaLevel = model.MfaLevel,
            Description = model.Description,
            PreferredLocale = model.PreferredLocale,   
            IconHash = model.IconHash,
            OwnerId = model.OwnerId,
            AfkChannelId = model.AfkChannelId,
            ApplicationId = model.ApplicationId,
            BannerId = model.Banner,
            DiscoverySplashId = model.DiscoverySplash,
            EmbedChannelId = model.EmbeddedChannelId,
            ExplicitContentFilter = model.ExplicitContentFilterLevel,
            IsBoostProgressBarEnabled = model.ProgressBarEnabled,
            NsfwLevel = model.NsfwLevel,
            PublicUpdatesChannelId = model.PublicUpdatesChannelId,
            RegionId = model.RegionId,
            RulesChannelId = model.RulesChannelId,
            SplashId = model.Splash,
            SystemChannelFlags = model.SystemChannelFlags,
            SystemChannelId = model.SystemChannelId,
            VanityURLCode = model.VanityUrl,
            VerificationLevel = model.VerificationLevel
        };
    }

    /// <inheritdoc cref="IGuild.Name"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string Name { get; private set; }

    /// <inheritdoc cref="IGuild.AFKTimeout"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public int? AfkTimeout { get; private set; }

    /// <inheritdoc cref="IGuild.IsWidgetEnabled"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public bool? IsEmbeddable { get; private set; }

    /// <inheritdoc cref="IGuild.DefaultMessageNotifications"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public DefaultMessageNotifications? DefaultMessageNotifications { get; private set; }

    /// <inheritdoc cref="IGuild.MfaLevel"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public MfaLevel? MfaLevel { get; private set; }

    /// <inheritdoc cref="IGuild.VerificationLevel"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public VerificationLevel? VerificationLevel { get; private set; }

    /// <inheritdoc cref="IGuild.ExplicitContentFilter"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ExplicitContentFilterLevel? ExplicitContentFilter { get; private set; }

    /// <inheritdoc cref="IGuild.IconId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string IconHash { get; private set; }

    /// <inheritdoc cref="IGuild.SplashId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string SplashId { get; private set; }

    /// <inheritdoc cref="IGuild.DiscoverySplashId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string DiscoverySplashId { get; private set; }

    /// <inheritdoc cref="IGuild.AFKChannelId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? AfkChannelId { get; private set; }

    /// <inheritdoc cref="IGuild.WidgetChannelId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? EmbedChannelId { get; private set; }

    /// <inheritdoc cref="IGuild.SystemChannelId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? SystemChannelId { get; private set; }

    /// <inheritdoc cref="IGuild.RulesChannelId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? RulesChannelId { get; private set; }

    /// <inheritdoc cref="IGuild.PublicUpdatesChannelId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? PublicUpdatesChannelId { get; private set; }

    /// <inheritdoc cref="IGuild.OwnerId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? OwnerId { get; private set; }

    /// <inheritdoc cref="IGuild.ApplicationId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ulong? ApplicationId { get; private set; }

    /// <inheritdoc cref="IGuild.VoiceRegionId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string RegionId { get; private set; }

    /// <inheritdoc cref="IGuild.BannerId"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string BannerId { get; private set; }

    /// <inheritdoc cref="IGuild.VanityURLCode"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string VanityURLCode { get; private set; }

    /// <inheritdoc cref="IGuild.SystemChannelFlags"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public SystemChannelMessageDeny? SystemChannelFlags { get; private set; }

    /// <inheritdoc cref="IGuild.Description"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string Description { get; private set; }

    /// <inheritdoc cref="IGuild.PreferredLocale"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string PreferredLocale { get; private set; }

    /// <inheritdoc cref="IGuild.NsfwLevel"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public NsfwLevel? NsfwLevel { get; private set; }

    /// <inheritdoc cref="IGuild.IsBoostProgressBarEnabled"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public bool? IsBoostProgressBarEnabled { get; private set; }
}
