using Discord.API;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents information for a guild.
    /// </summary>
    public struct GuildInfo
    {
        internal GuildInfo(AuditLogChange[] changes, bool useOldData)
        {
            var nameT = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            Name = (useOldData ? nameT?.OldValue : nameT?.NewValue)?.ToObject<string>();

            var afkTimeoutT = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            AfkTimeout = (useOldData ? afkTimeoutT?.OldValue : afkTimeoutT?.NewValue)?.ToObject<int?>();

            var isEmbeddableT = changes.FirstOrDefault(x => x.ChangedProperty == "widget_enabled");
            IsEmbeddable = (useOldData ? isEmbeddableT?.OldValue : isEmbeddableT?.NewValue)?.ToObject<bool?>();

            var defaultNotificationsT = changes.FirstOrDefault(x => x.ChangedProperty == "default_message_notifications");
            DefaultMessageNotifications = (useOldData ? defaultNotificationsT?.OldValue : defaultNotificationsT?.NewValue)?.ToObject<DefaultMessageNotifications?>();

            var mfaLevelT = changes.FirstOrDefault(x => x.ChangedProperty == "mfa_level");
            MfaLevel = (useOldData ? mfaLevelT?.OldValue : mfaLevelT?.NewValue)?.ToObject<MfaLevel?>();

            var verificationLevelT = changes.FirstOrDefault(x => x.ChangedProperty == "verification_level");
            VerificationLevel = (useOldData ? verificationLevelT?.OldValue : verificationLevelT?.NewValue)?.ToObject<VerificationLevel?>();

            var expContFilterT = changes.FirstOrDefault(x => x.ChangedProperty == "explicit_content_filter");
            ExplicitContentFilter = (useOldData ? expContFilterT?.OldValue : expContFilterT?.NewValue)?.ToObject<ExplicitContentFilterLevel?>();

            var iconHashT = changes.FirstOrDefault(x => x.ChangedProperty == "icon_hash");
            IconHash = (useOldData ? iconHashT?.OldValue : iconHashT?.NewValue)?.ToObject<string>();

            var discoverySplashT = changes.FirstOrDefault(x => x.ChangedProperty == "discovery_splash");
            DiscoverySplashId = (useOldData ? discoverySplashT?.OldValue : discoverySplashT?.NewValue)?.ToObject<string>(); //TODO: test

            var splashT = changes.FirstOrDefault(x => x.ChangedProperty == "splash");
            SplashId = (useOldData ? splashT?.OldValue : splashT?.NewValue)?.ToObject<string>(); //TODO: test

            var afkChannelT = changes.FirstOrDefault(x => x.ChangedProperty == "afk_channel_id");
            AfkChannelId = (useOldData ? afkChannelT?.OldValue : afkChannelT?.NewValue)?.ToObject<ulong?>();

            var embedChannelT = changes.FirstOrDefault(x => x.ChangedProperty == "widget_channel_id");
            EmbedChannelId = (useOldData ? embedChannelT?.OldValue : embedChannelT?.NewValue)?.ToObject<ulong?>();

            var systemChannelT = changes.FirstOrDefault(x => x.ChangedProperty == "system_channel_id");
            SystemChannelId = (useOldData ? systemChannelT?.OldValue : systemChannelT?.NewValue)?.ToObject<ulong?>();

            var rulesChannelT = changes.FirstOrDefault(x => x.ChangedProperty == "rules_channel_id");
            RulesChannelId = (useOldData ? rulesChannelT?.OldValue : rulesChannelT?.NewValue)?.ToObject<ulong?>();

            var publicUpdatesChannelT = changes.FirstOrDefault(x => x.ChangedProperty == "public_updates_channel_id");
            PublicUpdatesChannelId = (useOldData ? publicUpdatesChannelT?.OldValue : publicUpdatesChannelT?.NewValue)?.ToObject<ulong?>();

            var ownerIdT = changes.FirstOrDefault(x => x.ChangedProperty == "owner_id");
            OwnerId = (useOldData ? ownerIdT?.OldValue : ownerIdT?.NewValue)?.ToObject<ulong?>();

            var applicationIdT = changes.FirstOrDefault(x => x.ChangedProperty == "application_id");
            ApplicationId = (useOldData ? applicationIdT?.OldValue : applicationIdT?.NewValue)?.ToObject<ulong?>();

            var regionIdT = changes.FirstOrDefault(x => x.ChangedProperty == "region");
            RegionId = (useOldData ? regionIdT?.OldValue : regionIdT?.NewValue)?.ToObject<string>();

            var bannerIdT = changes.FirstOrDefault(x => x.ChangedProperty == "banner");
            BannerId = (useOldData ? bannerIdT?.OldValue : bannerIdT?.NewValue)?.ToObject<string>(); //TODO: test

            var vanityT = changes.FirstOrDefault(x => x.ChangedProperty == "vanity_url_code");
            VanityURLCode = (useOldData ? vanityT?.OldValue : vanityT?.NewValue)?.ToObject<string>(); //TODO: test

            var systemChannelFlagsT = changes.FirstOrDefault(x => x.ChangedProperty == "system_channel_flags");
            SystemChannelFlags = (useOldData ? systemChannelFlagsT?.OldValue : systemChannelFlagsT?.NewValue)?.ToObject<SystemChannelMessageDeny?>();

            var descriptionT = changes.FirstOrDefault(x => x.ChangedProperty == "description");
            Description = (useOldData ? descriptionT?.OldValue : descriptionT?.NewValue)?.ToObject<string>();

            var localeT = changes.FirstOrDefault(x => x.ChangedProperty == "preferred_locale");
            PreferredLocale = (useOldData ? localeT?.OldValue : localeT?.NewValue)?.ToObject<string>();

            var nsfwLevelT = changes.FirstOrDefault(x => x.ChangedProperty == "nsfw_level");
            NsfwLevel = (useOldData ? nsfwLevelT?.OldValue : nsfwLevelT?.NewValue)?.ToObject<NsfwLevel?>();

            var boostBarT = changes.FirstOrDefault(x => x.ChangedProperty == "premium_progress_bar_enabled");
            IsBoostProgressBarEnabled = (useOldData ? boostBarT?.OldValue : boostBarT?.NewValue)?.ToObject<bool?>();

        }

        /// <inheritdoc cref="IGuild.Name"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string Name { get; }
        /// <inheritdoc cref="IGuild.AFKTimeout"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        int? AfkTimeout { get; }
        /// <inheritdoc cref="IGuild.IsWidgetEnabled"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        bool? IsEmbeddable { get; }
        /// <inheritdoc cref="IGuild.DefaultMessageNotifications"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        DefaultMessageNotifications? DefaultMessageNotifications { get; }
        /// <inheritdoc cref="IGuild.MfaLevel"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        MfaLevel? MfaLevel { get; }
        /// <inheritdoc cref="IGuild.VerificationLevel"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        VerificationLevel? VerificationLevel { get; }
        /// <inheritdoc cref="IGuild.ExplicitContentFilter"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        ExplicitContentFilterLevel? ExplicitContentFilter { get; }
        /// <inheritdoc cref="IGuild.IconId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string IconHash { get; }
        /// <inheritdoc cref="IGuild.SplashId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string SplashId { get; }
        /// <inheritdoc cref="IGuild.DiscoverySplashId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string DiscoverySplashId { get; }
        /// <inheritdoc cref="IGuild.AFKChannelId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        ulong? AfkChannelId { get; }
        /// <inheritdoc cref="IGuild.WidgetChannelId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        ulong? EmbedChannelId { get; }
        /// <inheritdoc cref="IGuild.SystemChannelId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        ulong? SystemChannelId { get; }
        /// <inheritdoc cref="IGuild.RulesChannelId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        ulong? RulesChannelId { get; }
        /// <inheritdoc cref="IGuild.PublicUpdatesChannelId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        ulong? PublicUpdatesChannelId { get; }
        /// <inheritdoc cref="IGuild.OwnerId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        ulong? OwnerId { get; }
        /// <inheritdoc cref="IGuild.ApplicationId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        ulong? ApplicationId { get; }
        /// <inheritdoc cref="IGuild.VoiceRegionId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string RegionId { get; }
        /// <inheritdoc cref="IGuild.BannerId"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string BannerId { get; }
        /// <inheritdoc cref="IGuild.VanityURLCode"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string VanityURLCode { get; }
        /// <inheritdoc cref="IGuild.SystemChannelFlags"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        SystemChannelMessageDeny? SystemChannelFlags { get; }
        /// <inheritdoc cref="IGuild.Description"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string Description { get; }
        /// <inheritdoc cref="IGuild.PreferredLocale"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        string PreferredLocale { get; }
        /// <inheritdoc cref="IGuild.NsfwLevel"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        NsfwLevel? NsfwLevel { get; }
        /// <inheritdoc cref="IGuild.IsBoostProgressBarEnabled"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        bool? IsBoostProgressBarEnabled { get; }
    }
}
