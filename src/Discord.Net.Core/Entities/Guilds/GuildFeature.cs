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
        None = 0,
        /// <summary>
        ///     The guild has access to animated banners.
        /// </summary>
        AnimatedBanner = 1 << 0,
        /// <summary>
        ///     The guild has access to set an animated guild icon.
        /// </summary>
        AnimatedIcon = 1 << 1,
        /// <summary>
        ///     The guild has access to set a guild banner image.
        /// </summary>
        Banner = 1 << 2,
        /// <summary>
        ///     The guild has access to channel banners.
        /// </summary>
        ChannelBanner = 1 << 3,
        /// <summary>
        ///     The guild has access to use commerce features (i.e. create store channels).
        /// </summary>
        Commerce = 1 << 4,
        /// <summary>
        ///     The guild can enable welcome screen, Membership Screening, stage channels and discovery, and receives community updates.
        /// </summary>
        Community = 1 << 5,
        /// <summary>
        ///     The guild is able to be discovered in the directory.
        /// </summary>
        Discoverable = 1 << 6,
        /// <summary>
        ///     The guild has discoverable disabled.
        /// </summary>
        DiscoverableDisabled = 1 << 7,
        /// <summary>
        ///     The guild has enabled discoverable before.
        /// </summary>
        EnabledDiscoverableBefore = 1 << 8,
        /// <summary>
        ///     The guild is able to be featured in the directory.
        /// </summary>
        Featureable = 1 << 9,
        /// <summary>
        ///     The guild has a force relay.
        /// </summary>
        ForceRelay = 1 << 10,
        /// <summary>
        ///     The guild has a directory entry.
        /// </summary>
        HasDirectoryEntry = 1 << 11,
        /// <summary>
        ///     The guild is a hub.
        /// </summary>
        Hub = 1 << 12,
        /// <summary>
        ///     You shouldn't be here...
        /// </summary>
        InternalEmployeeOnly = 1 << 13,
        /// <summary>
        ///     The guild has access to set an invite splash background.
        /// </summary>
        InviteSplash = 1 << 14,
        /// <summary>
        ///     The guild is linked to a hub.
        /// </summary>
        LinkedToHub = 1 << 15,
        /// <summary>
        ///     The guild has member profiles.
        /// </summary>
        MemberProfiles = 1 << 16,
        /// <summary>
        ///     The guild has enabled <seealso href="https://discord.com/developers/docs/resources/guild#membership-screening-object">Membership Screening</seealso>.
        /// </summary>
        MemberVerificationGateEnabled = 1 << 17,
        /// <summary>
        ///     The guild has enabled monetization.
        /// </summary>
        MonetizationEnabled = 1 << 18,
        /// <summary>
        ///     The guild has more emojis.
        /// </summary>
        MoreEmoji = 1 << 19,
        /// <summary>
        ///     The guild has increased custom sticker slots.
        /// </summary>
        MoreStickers = 1 << 20,
        /// <summary>
        ///     The guild has access to create news channels.
        /// </summary>
        News = 1 << 21,
        /// <summary>
        ///     The guild has new thread permissions.
        /// </summary>
        NewThreadPermissions = 1 << 22,
        /// <summary>
        ///     The guild is partnered.
        /// </summary>
        Partnered = 1 << 23,
        /// <summary>
        ///     The guild has a premium tier three override; guilds made by Discord usually have this.
        /// </summary>
        PremiumTier3Override = 1 << 24,
        /// <summary>
        ///     The guild can be previewed before joining via Membership Screening or the directory.
        /// </summary>
        PreviewEnabled = 1 << 25,
        /// <summary>
        ///     The guild has access to create private threads.
        /// </summary>
        PrivateThreads = 1 << 26,
        /// <summary>
        ///     The guild has relay enabled.
        /// </summary>
        RelayEnabled = 1 << 27,
        /// <summary>
        ///     The guild is able to set role icons.
        /// </summary>
        RoleIcons = 1 << 28,
        /// <summary>
        ///     The guild has role subscriptions available for purchase.
        /// </summary>
        RoleSubscriptionsAvailableForPurchase = 1 << 29,
        /// <summary>
        ///     The guild has role subscriptions enabled.
        /// </summary>
        RoleSubscriptionsEnabled = 1 << 30,
        /// <summary>
        ///     The guild has access to the seven day archive time for threads.
        /// </summary>
        SevenDayThreadArchive = 1 << 31,
        /// <summary>
        ///     The guild has text in voice enabled.
        /// </summary>
        TextInVoiceEnabled = 1 << 32,
        /// <summary>
        ///     The guild has threads enabled.
        /// </summary>
        ThreadsEnabled = 1 << 33,
        /// <summary>
        ///     The guild has testing threads enabled.
        /// </summary>
        ThreadsEnabledTesting = 1 << 34,
        /// <summary>
        ///     The guild has the default thread auto archive.
        /// </summary>
        ThreadsDefaultAutoArchiveDuration = 1 << 35,
        /// <summary>
        ///     The guild has access to the three day archive time for threads.
        /// </summary>
        ThreeDayThreadArchive = 1 << 36,
        /// <summary>
        ///     The guild has enabled ticketed events.
        /// </summary>
        TicketedEventsEnabled = 1 << 37,
        /// <summary>
        ///     The guild has access to set a vanity URL.
        /// </summary>
        VanityUrl = 1 << 38,
        /// <summary>
        ///     The guild is verified.
        /// </summary>
        Verified = 1 << 39,
        /// <summary>
        ///     The guild has access to set 384kbps bitrate in voice (previously VIP voice servers).
        /// </summary>
        VIPRegions = 1 << 40,
        /// <summary>
        ///     The guild has enabled the welcome screen.
        /// </summary>
        WelcomeScreenEnabled = 1 << 41,
    }
}
