using System.Collections;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     Represents a collection of features found within a guild.
/// </summary>
public readonly struct GuildFeatures : IReadOnlySet<string>, IImmutableSet<string>
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
    public const string Featureable = "FEATURABLE";
    public const string ForceRelay = "FORCE_RELAY";
    public const string HasDirectoryEntry = "HAS_DIRECTORY_ENTRY";
    public const string Hub = "HUB";

    public const string
        InternalEmployeeOnly =
            "INTERNAL_EMPLOYEE_ONLY"; // sus :O (pls help i've not been fed in 23 days and quahu is keeping me in his basement)

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

    internal string[] RawValue { get; }

    private readonly ImmutableHashSet<string> _features;

    internal GuildFeatures(string[] features)
    {
        RawValue = features;
        _features = features.ToImmutableHashSet();
    }

    /// <inheritdoc />
    public bool Contains(string item) => _features.Contains(item);

    /// <inheritdoc />
    public bool IsProperSubsetOf(IEnumerable<string> other) => _features.IsProperSubsetOf(other);

    /// <inheritdoc />
    public bool IsProperSupersetOf(IEnumerable<string> other) => _features.IsProperSupersetOf(other);

    /// <inheritdoc />
    public bool IsSubsetOf(IEnumerable<string> other) => _features.IsSubsetOf(other);

    /// <inheritdoc />
    public bool IsSupersetOf(IEnumerable<string> other) => _features.IsSupersetOf(other);

    /// <inheritdoc />
    public bool Overlaps(IEnumerable<string> other) => _features.Overlaps(other);

    /// <inheritdoc />
    public bool SetEquals(IEnumerable<string> other) => _features.SetEquals(other);

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_features).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_features).GetEnumerator();

    /// <inheritdoc />
    public IImmutableSet<string> Add(string value) => ((IImmutableSet<string>)_features).Add(value);

    /// <inheritdoc />
    public IImmutableSet<string> Clear() => ((IImmutableSet<string>)_features).Clear();

    /// <inheritdoc />
    public IImmutableSet<string> Except(IEnumerable<string> other) => ((IImmutableSet<string>)_features).Except(other);

    /// <inheritdoc />
    public IImmutableSet<string> Intersect(IEnumerable<string> other) =>
        ((IImmutableSet<string>)_features).Intersect(other);

    /// <inheritdoc />
    public IImmutableSet<string> Remove(string value) => ((IImmutableSet<string>)_features).Remove(value);

    /// <inheritdoc />
    public IImmutableSet<string> SymmetricExcept(IEnumerable<string> other) =>
        ((IImmutableSet<string>)_features).SymmetricExcept(other);

    /// <inheritdoc />
    public bool TryGetValue(string equalValue, out string actualValue) =>
        _features.TryGetValue(equalValue, out actualValue);

    /// <inheritdoc />
    public IImmutableSet<string> Union(IEnumerable<string> other) => ((IImmutableSet<string>)_features).Union(other);
}
