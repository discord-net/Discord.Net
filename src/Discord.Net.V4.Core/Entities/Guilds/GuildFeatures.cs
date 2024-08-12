using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     Represents a collection of features found within a guild.
/// </summary>
public readonly struct GuildFeatures : IReadOnlySet<string>, IEquatable<GuildFeatures>
{
    #region Features

    public const string AnimatedBanner = "ANIMATED_BANNER";
    public const string AnimatedIcon = "ANIMATED_ICON";
    public const string ApplicationCommandsPermissionsV2 = "APPLICATION_COMMAND_PERMISSIONS_V2";
    public const string AutoModeration = "AUTO_MODERATION";
    public const string Banner = "BANNER";
    public const string ChannelBanner = "CHANNEL_BANNER";
    public const string ClydeEnabled = "CLYDE_ENABLED";
    public const string Commerce = "COMMERCE";
    public const string Community = "COMMUNITY";
    public const string CreatorMonetizableProvisional = "CREATOR_MONETIZABLE_PROVISIONAL";
    public const string CreatorStorePage = "CREATOR_STORE_PAGE";
    public const string DeveloperSupportServer = "DEVELOPER_SUPPORT_SERVER";
    public const string Discoverable = "DISCOVERABLE";
    public const string DiscoverableDisabled = "DISCOVERABLE_DISABLED";
    public const string EnabledDiscoverableBefore = "ENABLED_DISCOVERABLE_BEFORE";
    public const string Featurable = "FEATURABLE";
    public const string ForceRelay = "FORCE_RELAY";
    public const string HasDirectoryEntry = "HAS_DIRECTORY_ENTRY";
    public const string Hub = "HUB";

    public const string InternalEmployeeOnly = "INTERNAL_EMPLOYEE_ONLY"; // sus :O (pls help i've not been fed in 23 days and quahu is keeping me in his basement)

    public const string InviteSplash = "INVITE_SPLASH";
    public const string InvitesDisabled = "INVITES_DISABLED";
    public const string LinkedToHub = "LINKED_TO_HUB";
    public const string MemberProfiles = "MEMBER_PROFILES";
    public const string MemberVerificationGateEnabled = "MEMBER_VERIFICATION_GATE_ENABLED";
    public const string MonetizationEnabled = "MONETIZATION_ENABLED";
    public const string MoreEmoji = "MORE_EMOJI";
    public const string MoreStickers = "MORE_STICKERS";
    public const string News = "NEWS";
    public const string NewThreadPermissions = "NEW_THREAD_PERMISSIONS";
    public const string Partnered = "PARTNERED";
    public const string PremiumTier3Override = "PREMIUM_TIER_3_OVERRIDE";
    public const string PreviewEnabled = "PREVIEW_ENABLED";
    public const string PrivateThreads = "PRIVATE_THREADS";
    public const string RaidAlertsDisabled = "RAID_ALERTS_DISABLED";
    public const string RelayEnabled = "RELAY_ENABLED";
    public const string RoleIcons = "ROLE_ICONS";
    public const string RoleSubscriptionsAvailableForPurchase = "ROLE_SUBSCRIPTIONS_AVAILABLE_FOR_PURCHASE";
    public const string RoleSubscriptionsEnabled = "ROLE_SUBSCRIPTIONS_ENABLED";
    public const string SevenDayThreadArchive = "SEVEN_DAY_THREAD_ARCHIVE";
    public const string TextInVoiceEnabled = "TEXT_IN_VOICE_ENABLED";
    public const string ThreadsEnabled = "THREADS_ENABLED";
    public const string ThreadsEnabledTesting = "THREADS_ENABLED_TESTING";
    public const string ThreadsDefaultAutoArchiveDuration = "THREADS_DEFAULT_AUTO_ARCHIVE_DURATION";
    public const string ThreeDayThreadArchive = "THREE_DAY_THREAD_ARCHIVE";
    public const string TicketedEventsEnabled = "TICKETED_EVENTS_ENABLED";
    public const string VanityUrl = "VANITY_URL";
    public const string Verified = "VERIFIED";
    public const string VIPRegions = "VIP_REGIONS";
    public const string WelcomeScreenEnabled = "WELCOME_SCREEN_ENABLED";
    public const string GuildWebPageVanityUrl = "";

    #endregion

    /// <inheritdoc />
    public int Count => _features.Count;

    private readonly IReadOnlySet<string> _features;

    internal GuildFeatures(string[]? features)
    {
        _features = features?.ToFrozenSet() ?? (IReadOnlySet<string>)ImmutableHashSet<string>.Empty;
    }

    public IEnumerator<string> GetEnumerator() => _features.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_features).GetEnumerator();

    public bool Contains(string item) => _features.Contains(item);

    public bool IsProperSubsetOf(IEnumerable<string> other) => _features.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<string> other) => _features.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<string> other) => _features.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<string> other) => _features.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<string> other) => _features.Overlaps(other);

    public bool SetEquals(IEnumerable<string> other) => _features.SetEquals(other);

    public bool Equals(GuildFeatures other) => _features.Equals(other._features);
    public bool Equals(IEnumerable<string>? other) => other is not null && _features.SequenceEqual(other);

    public override bool Equals(object? obj) => obj switch
    {
        GuildFeatures other => Equals(other),
        IEnumerable<string> other => Equals(other),
        _ => false
    };

    public override int GetHashCode() => _features.GetHashCode();

    public static bool operator ==(GuildFeatures left, GuildFeatures right) => left.Equals(right);

    public static bool operator !=(GuildFeatures left, GuildFeatures right) => !left.Equals(right);

    public static bool operator ==(GuildFeatures left, IEnumerable<string> right) => left.Equals(right);

    public static bool operator !=(GuildFeatures left, IEnumerable<string> right) => !left.Equals(right);
}
