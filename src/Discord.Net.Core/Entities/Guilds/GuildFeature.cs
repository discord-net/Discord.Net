using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    [Flags]
    public enum GuildFeature
    {
        /// <summary>
        ///     The guild has no features.
        /// </summary>
        None = 0,
        /// <summary>
        ///     The guild has access to set an animated guild icon.
        /// </summary>
        AnimatedIcon = 1 << 0,
        /// <summary>
        ///     The guild has access to set a guild banner image.
        /// </summary>
        Banner = 1 << 1,
        /// <summary>
        ///     The guild has access to use commerce features (i.e. create store channels).
        /// </summary>
        Commerce = 1 << 2,
        /// <summary>
        ///     The guild can enable welcome screen, Membership Screening, stage channels and discovery, and receives community updates.
        /// </summary>
        Community = 1 << 3,
        /// <summary>
        ///     The guild is able to be discovered in the directory.
        /// </summary>
        Discoverable = 1 << 4,
        /// <summary>
        ///     The guild is able to be featured in the directory.
        /// </summary>
        Featureable = 1 << 5,
        /// <summary>
        ///     The guild has access to set an invite splash background.
        /// </summary>
        InviteSplash = 1 << 6,
        /// <summary>
        ///     The guild has enabled <seealso href="https://discord.com/developers/docs/resources/guild#membership-screening-object">Membership Screening</seealso>.
        /// </summary>
        MemberVerificationGateEnabled = 1 << 7,
        /// <summary>
        ///     The guild has enabled monetization.
        /// </summary>
        MonetizationEnabled = 1 << 8,
        /// <summary>
        ///     The guild has increased custom sticker slots.
        /// </summary>
        MoreStickers = 1 << 9,
        /// <summary>
        ///     The guild has access to create news channels.
        /// </summary>
        News = 1 << 10,
        /// <summary>
        ///     The guild is partnered.
        /// </summary>
        Partnered = 1 << 11,
        /// <summary>
        ///     The guild can be previewed before joining via Membership Screening or the directory.
        /// </summary>
        PreviewEnabled = 1 << 12,
        /// <summary>
        ///     The guild has access to create private threads.
        /// </summary>
        PrivateThreads = 1 << 13,
        /// <summary>
        ///     The guild is able to set role icons.
        /// </summary>
        RoleIcons = 1 << 14,
        /// <summary>
        ///     The guild has access to the seven day archive time for threads.
        /// </summary>
        SevenDayThreadArchive = 1 << 15,
        /// <summary>
        ///     The guild has access to the three day archive time for threads.
        /// </summary>
        ThreeDayThreadArchive = 1 << 16,
        /// <summary>
        ///     The guild has enabled ticketed events.
        /// </summary>
        TicketedEventsEnabled = 1 << 17,
        /// <summary>
        ///     The guild has access to set a vanity URL.
        /// </summary>
        VanityUrl = 1 << 18,
        /// <summary>
        ///     The guild is verified.
        /// </summary>
        Verified = 1 << 19,
        /// <summary>
        ///     The guild has access to set 384kbps bitrate in voice (previously VIP voice servers).
        /// </summary>
        VIPRegions = 1 << 20,
        /// <summary>
        ///     The guild has enabled the welcome screen.
        /// </summary>
        WelcomeScreenEnabled = 1 << 21,
    }
}
