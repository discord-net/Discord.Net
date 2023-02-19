using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    [Flags]
    public enum GuildFeature : long
    {
        /// <summary>
        ///     The guild has no features.
        /// </summary>
        None = 0L,
        /// <summary>
        ///     The guild has access to animated banners.
        /// </summary>
        AnimatedBanner = 1L << 0,
        /// <summary>
        ///     The guild has access to set an animated guild icon.
        /// </summary>
        AnimatedIcon = 1L << 1,
        /// <summary>
        ///     The guild has access to set a guild banner image.
        /// </summary>
        Banner = 1L << 2,
        /// <summary>
        ///     The guild has access to channel banners.
        /// </summary>
        ChannelBanner = 1L << 3,
        /// <summary>
        ///     The guild has access to use commerce features (i.e. create store channels).
        /// </summary>
        Commerce = 1L << 4,
        /// <summary>
        ///     The guild can enable welcome screen, Membership Screening, stage channels and discovery, and receives community updates. This feature is mutable.
        /// </summary>
        Community = 1L << 5,
        /// <summary>
        ///     The guild is able to be discovered in the directory. This feature is mutable.
        /// </summary>
        Discoverable = 1L << 6,
        /// <summary>
        ///     The guild has discoverable disabled.
        /// </summary>
        DiscoverableDisabled = 1L << 7,
        /// <summary>
        ///     The guild has enabled discoverable before.
        /// </summary>
        EnabledDiscoverableBefore = 1L << 8,
        /// <summary>
        ///     The guild is able to be featured in the directory.
        /// </summary>
        Featureable = 1L << 9,
        /// <summary>
        ///     The guild has a force relay.
        /// </summary>
        ForceRelay = 1L << 10,
        /// <summary>
        ///     The guild has a directory entry.
        /// </summary>
        HasDirectoryEntry = 1L << 11,
        /// <summary>
        ///     The guild is a hub.
        /// </summary>
        Hub = 1L << 12,
        /// <summary>
        ///     You shouldn't be here...
        /// </summary>
        InternalEmployeeOnly = 1L << 13,
        /// <summary>
        ///     The guild has access to set an invite splash background.
        /// </summary>
        InviteSplash = 1L << 14,
        /// <summary>
        ///     The guild is linked to a hub.
        /// </summary>
        LinkedToHub = 1L << 15,
        /// <summary>
        ///     The guild has member profiles.
        /// </summary>
        MemberProfiles = 1L << 16,
        /// <summary>
        ///     The guild has enabled <seealso href="https://discord.com/developers/docs/resources/guild#membership-screening-object">Membership Screening</seealso>.
        /// </summary>
        MemberVerificationGateEnabled = 1L << 17,
        /// <summary>
        ///     The guild has enabled monetization.
        /// </summary>
        MonetizationEnabled = 1L << 18,
        /// <summary>
        ///     The guild has more emojis.
        /// </summary>
        MoreEmoji = 1L << 19,
        /// <summary>
        ///     The guild has increased custom sticker slots.
        /// </summary>
        MoreStickers = 1L << 20,
        /// <summary>
        ///     The guild has access to create news channels.
        /// </summary>
        News = 1L << 21,
        /// <summary>
        ///     The guild has new thread permissions.
        /// </summary>
        NewThreadPermissions = 1L << 22,
        /// <summary>
        ///     The guild is partnered.
        /// </summary>
        Partnered = 1L << 23,
        /// <summary>
        ///     The guild has a premium tier three override; guilds made by Discord usually have this.
        /// </summary>
        PremiumTier3Override = 1L << 24,
        /// <summary>
        ///     The guild can be previewed before joining via Membership Screening or the directory.
        /// </summary>
        PreviewEnabled = 1L << 25,
        /// <summary>
        ///     The guild has access to create private threads.
        /// </summary>
        PrivateThreads = 1L << 26,
        /// <summary>
        ///     The guild has relay enabled.
        /// </summary>
        RelayEnabled = 1L << 27,
        /// <summary>
        ///     The guild is able to set role icons.
        /// </summary>
        RoleIcons = 1L << 28,
        /// <summary>
        ///     The guild has role subscriptions available for purchase.
        /// </summary>
        RoleSubscriptionsAvailableForPurchase = 1L << 29,
        /// <summary>
        ///     The guild has role subscriptions enabled.
        /// </summary>
        RoleSubscriptionsEnabled = 1L << 30,
        /// <summary>
        ///     The guild has access to the seven day archive time for threads.
        /// </summary>
        SevenDayThreadArchive = 1L << 31,
        /// <summary>
        ///     The guild has text in voice enabled.
        /// </summary>
        TextInVoiceEnabled = 1L << 32,
        /// <summary>
        ///     The guild has threads enabled.
        /// </summary>
        ThreadsEnabled = 1L << 33,
        /// <summary>
        ///     The guild has testing threads enabled.
        /// </summary>
        ThreadsEnabledTesting = 1L << 34,
        /// <summary>
        ///     The guild has the default thread auto archive.
        /// </summary>
        ThreadsDefaultAutoArchiveDuration = 1L << 35,
        /// <summary>
        ///     The guild has access to the three day archive time for threads.
        /// </summary>
        ThreeDayThreadArchive = 1L << 36,
        /// <summary>
        ///     The guild has enabled ticketed events.
        /// </summary>
        TicketedEventsEnabled = 1L << 37,
        /// <summary>
        ///     The guild has access to set a vanity URL.
        /// </summary>
        VanityUrl = 1L << 38,
        /// <summary>
        ///     The guild is verified.
        /// </summary>
        Verified = 1L << 39,
        /// <summary>
        ///     The guild has access to set 384kbps bitrate in voice (previously VIP voice servers).
        /// </summary>
        VIPRegions = 1L << 40,
        /// <summary>
        ///     The guild has enabled the welcome screen.
        /// </summary>
        WelcomeScreenEnabled = 1L << 41,
        /// <summary>
        ///     The guild has been set as a support server on the App Directory.
        /// </summary>
        DeveloperSupportServer = 1L << 42,
        /// <summary>
        ///     The guild has invites disabled. This feature is mutable.
        /// </summary>
        InvitesDisabled = 1L << 43,
        /// <summary>
        ///     The guild has auto moderation enabled.
        /// </summary>
        AutoModeration = 1L << 44
    }
}
