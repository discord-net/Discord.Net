using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a guild object.
    /// </summary>
    public record Guild
    {
        /// <summary>
        ///     Creates a <see cref="Guild"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Guild id.</param>
        /// <param name="name">Guild name (2-100 characters, excluding trailing and leading whitespace).</param>
        /// <param name="icon">Icon hash.</param>
        /// <param name="iconHash">Icon hash, returned when in the template object.</param>
        /// <param name="splash">Splash hash.</param>
        /// <param name="discoverySplash">Discovery splash hash; only present for guilds with the "DISCOVERABLE" feature.</param>
        /// <param name="owner">True if the user is the owner of the guild.</param>
        /// <param name="ownerId">Id of owner.</param>
        /// <param name="permissions">Total permissions for the user in the guild (excludes overwrites).</param>
        /// <param name="region">Voice region id for the guild.</param>
        /// <param name="afkChannelId">Id of afk channel.</param>
        /// <param name="afkTimeout">Afk timeout in seconds.</param>
        /// <param name="widgetEnabled">True if the server widget is enabled.</param>
        /// <param name="widgetChannelId">The channel id that the widget will generate an invite to, or null if set to no invite.</param>
        /// <param name="verificationLevel">Verification level required for the guild.</param>
        /// <param name="defaultMessageNotifications">Default message notifications level.</param>
        /// <param name="explicitContentFilter">Explicit content filter level.</param>
        /// <param name="roles">Roles in the guild.</param>
        /// <param name="emojis">Custom guild emojis.</param>
        /// <param name="features">Enabled guild features.</param>
        /// <param name="mfaLevel">Required MFA level for the guild.</param>
        /// <param name="applicationId">Application id of the guild creator if it is bot-created.</param>
        /// <param name="systemChannelId">The id of the channel where guild notices such as welcome messages and boost events are posted.</param>
        /// <param name="systemChannelFlags">System channel flags.</param>
        /// <param name="rulesChannelId">The id of the channel where Community guilds can display rules and/or guidelines.</param>
        /// <param name="joinedAt">When this guild was joined at.</param>
        /// <param name="large">True if this is considered a large guild.</param>
        /// <param name="unavailable">True if this guild is unavailable due to an outage.</param>
        /// <param name="memberCount">Total number of members in this guild.</param>
        /// <param name="voiceStates">States of members currently in voice channels; lacks the guild_id key.</param>
        /// <param name="members">Users in the guild.</param>
        /// <param name="channels">Channels in the guild.</param>
        /// <param name="threads">All active threads in the guild that current user has permission to view.</param>
        /// <param name="presences">Presences of the members in the guild, will only include non-offline members if the size is greater than large threshold.</param>
        /// <param name="maxPresences">The maximum number of presences for the guild (the default value, currently 25000, is in effect when null is returned).</param>
        /// <param name="maxMembers">The maximum number of members for the guild.</param>
        /// <param name="vanityUrlCode">The vanity url code for the guild.</param>
        /// <param name="description">The description of a Community guild.</param>
        /// <param name="banner">Banner hash.</param>
        /// <param name="premiumTier">Premium tier (Server Boost level).</param>
        /// <param name="premiumSubscriptionCount">The number of boosts this guild currently has.</param>
        /// <param name="preferredLocale">The preferred locale of a Community guild; used in server discovery and notices from Discord; defaults to "en-US".</param>
        /// <param name="publicUpdatesChannelId">The id of the channel where admins and moderators of Community guilds receive notices from Discord.</param>
        /// <param name="maxVideoChannelUsers">The maximum amount of users in a video channel.</param>
        /// <param name="approximateMemberCount">Approximate number of members in this guild.</param>
        /// <param name="approximatePresenceCount">Approximate number of non-offline members in this guild.</param>
        /// <param name="welcomeScreen">The welcome screen of a Community guild, shown to new members, returned in an Invite's guild object.</param>
        /// <param name="nsfwLevel">Guild NSFW level.</param>
        /// <param name="stageInstances">Stage instances in the guild.</param>
        [JsonConstructor]
        public Guild(Snowflake id, string name, string? icon, Optional<string?> iconHash, string? splash, string? discoverySplash, Optional<bool> owner, Snowflake ownerId, Optional<Permissions> permissions, string region, Snowflake? afkChannelId, int afkTimeout, Optional<bool> widgetEnabled, Optional<Snowflake?> widgetChannelId, int verificationLevel, int defaultMessageNotifications, int explicitContentFilter, Role[] roles, Emoji[] emojis, string[] features, int mfaLevel, Snowflake? applicationId, Snowflake? systemChannelId, int systemChannelFlags, Snowflake? rulesChannelId, Optional<DateTimeOffset> joinedAt, Optional<bool> large, Optional<bool> unavailable, Optional<int> memberCount, Optional<VoiceState[]> voiceStates, Optional<GuildMember[]> members, Optional<Channel[]> channels, Optional<Channel[]> threads, Optional<PresenceUpdate[]> presences, Optional<int?> maxPresences, Optional<int> maxMembers, string? vanityUrlCode, string? description, string? banner, int premiumTier, Optional<int> premiumSubscriptionCount, string preferredLocale, Snowflake? publicUpdatesChannelId, Optional<int> maxVideoChannelUsers, Optional<int> approximateMemberCount, Optional<int> approximatePresenceCount, Optional<WelcomeScreen> welcomeScreen, int nsfwLevel, Optional<StageInstance[]> stageInstances)
        {
            Id = id;
            Name = name;
            Icon = icon;
            IconHash = iconHash;
            Splash = splash;
            DiscoverySplash = discoverySplash;
            Owner = owner;
            OwnerId = ownerId;
            Permissions = permissions;
            Region = region;
            AfkChannelId = afkChannelId;
            AfkTimeout = afkTimeout;
            WidgetEnabled = widgetEnabled;
            WidgetChannelId = widgetChannelId;
            VerificationLevel = verificationLevel;
            DefaultMessageNotifications = defaultMessageNotifications;
            ExplicitContentFilter = explicitContentFilter;
            Roles = roles;
            Emojis = emojis;
            Features = features;
            MfaLevel = mfaLevel;
            ApplicationId = applicationId;
            SystemChannelId = systemChannelId;
            SystemChannelFlags = systemChannelFlags;
            RulesChannelId = rulesChannelId;
            JoinedAt = joinedAt;
            Large = large;
            Unavailable = unavailable;
            MemberCount = memberCount;
            VoiceStates = voiceStates;
            Members = members;
            Channels = channels;
            Threads = threads;
            Presences = presences;
            MaxPresences = maxPresences;
            MaxMembers = maxMembers;
            VanityUrlCode = vanityUrlCode;
            Description = description;
            Banner = banner;
            PremiumTier = premiumTier;
            PremiumSubscriptionCount = premiumSubscriptionCount;
            PreferredLocale = preferredLocale;
            PublicUpdatesChannelId = publicUpdatesChannelId;
            MaxVideoChannelUsers = maxVideoChannelUsers;
            ApproximateMemberCount = approximateMemberCount;
            ApproximatePresenceCount = approximatePresenceCount;
            WelcomeScreen = welcomeScreen;
            NsfwLevel = nsfwLevel;
            StageInstances = stageInstances;
        }

        /// <summary>
        ///     Guild id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Guild name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     Icon hash.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; }

        /// <summary>
        ///     Icon hash, returned when in the template object.
        /// </summary>
        [JsonPropertyName("icon_hash")]
        public Optional<string?> IconHash { get; }

        /// <summary>
        ///     Splash hash.
        /// </summary>
        [JsonPropertyName("splash")]
        public string? Splash { get; }

        /// <summary>
        ///     Discovery splash hash; only present for guilds with the "DISCOVERABLE" feature.
        /// </summary>
        [JsonPropertyName("discovery_splash")]
        public string? DiscoverySplash { get; }

        /// <summary>
        ///     True if the user is the owner of the guild.
        /// </summary>
        [JsonPropertyName("owner")]
        public Optional<bool> Owner { get; }

        /// <summary>
        ///     Id of owner.
        /// </summary>
        [JsonPropertyName("owner_id")]
        public Snowflake OwnerId { get; }

        /// <summary>
        ///     Total permissions for the user in the guild (excludes overwrites).
        /// </summary>
        [JsonPropertyName("permissions")]
        public Optional<Permissions> Permissions { get; }

        /// <summary>
        ///     Voice region id for the guild.
        /// </summary>
        [JsonPropertyName("region")]
        public string Region { get; }

        /// <summary>
        ///     Id of afk channel.
        /// </summary>
        [JsonPropertyName("afk_channel_id")]
        public Snowflake? AfkChannelId { get; }

        /// <summary>
        ///     Afk timeout in seconds.
        /// </summary>
        [JsonPropertyName("afk_timeout")]
        public int AfkTimeout { get; }

        /// <summary>
        ///     True if the server widget is enabled.
        /// </summary>
        [JsonPropertyName("widget_enabled")]
        public Optional<bool> WidgetEnabled { get; }

        /// <summary>
        ///     The channel id that the widget will generate an invite to, or null if set to no invite.
        /// </summary>
        [JsonPropertyName("widget_channel_id")]
        public Optional<Snowflake?> WidgetChannelId { get; }

        /// <summary>
        ///     Verification level required for the guild.
        /// </summary>
        [JsonPropertyName("verification_level")]
        public int VerificationLevel { get; }

        /// <summary>
        ///     Default message notifications level.
        /// </summary>
        [JsonPropertyName("default_message_notifications")]
        public int DefaultMessageNotifications { get; }

        /// <summary>
        ///     Explicit content filter level.
        /// </summary>
        [JsonPropertyName("explicit_content_filter")]
        public int ExplicitContentFilter { get; }

        /// <summary>
        ///     Roles in the guild.
        /// </summary>
        [JsonPropertyName("roles")]
        public Role[] Roles { get; }

        /// <summary>
        ///     Custom guild emojis.
        /// </summary>
        [JsonPropertyName("emojis")]
        public Emoji[] Emojis { get; }

        /// <summary>
        ///     Enabled guild features.
        /// </summary>
        [JsonPropertyName("features")]
        public string[] Features { get; }

        /// <summary>
        ///     Required MFA level for the guild.
        /// </summary>
        [JsonPropertyName("mfa_level")]
        public int MfaLevel { get; }

        /// <summary>
        ///     Application id of the guild creator if it is bot-created.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Snowflake? ApplicationId { get; }

        /// <summary>
        ///     The id of the channel where guild notices such as welcome messages and boost events are posted.
        /// </summary>
        [JsonPropertyName("system_channel_id")]
        public Snowflake? SystemChannelId { get; }

        /// <summary>
        ///     System channel flags.
        /// </summary>
        [JsonPropertyName("system_channel_flags")]
        public int SystemChannelFlags { get; }

        /// <summary>
        ///     The id of the channel where Community guilds can display rules and/or guidelines.
        /// </summary>
        [JsonPropertyName("rules_channel_id")]
        public Snowflake? RulesChannelId { get; }

        /// <summary>
        ///     When this guild was joined at.
        /// </summary>
        [JsonPropertyName("joined_at")]
        public Optional<DateTimeOffset> JoinedAt { get; }

        /// <summary>
        ///     True if this is considered a large guild.
        /// </summary>
        [JsonPropertyName("large")]
        public Optional<bool> Large { get; }

        /// <summary>
        ///     True if this guild is unavailable due to an outage.
        /// </summary>
        [JsonPropertyName("unavailable")]
        public Optional<bool> Unavailable { get; }

        /// <summary>
        ///     Total number of members in this guild.
        /// </summary>
        [JsonPropertyName("member_count")]
        public Optional<int> MemberCount { get; }

        /// <summary>
        ///     States of members currently in voice channels; lacks the guild_id key.
        /// </summary>
        [JsonPropertyName("voice_states")]
        public Optional<VoiceState[]> VoiceStates { get; }

        /// <summary>
        ///     Users in the guild.
        /// </summary>
        [JsonPropertyName("members")]
        public Optional<GuildMember[]> Members { get; }

        /// <summary>
        ///     Channels in the guild.
        /// </summary>
        [JsonPropertyName("channels")]
        public Optional<Channel[]> Channels { get; }

        /// <summary>
        ///     All active threads in the guild that current user has permission to view.
        /// </summary>
        [JsonPropertyName("threads")]
        public Optional<Channel[]> Threads { get; }

        /// <summary>
        ///     Presences of the members in the guild, will only include non-offline members if the size is greater than large threshold.
        /// </summary>
        [JsonPropertyName("presences")]
        public Optional<PresenceUpdate[]> Presences { get; }

        /// <summary>
        ///     The maximum number of presences for the guild (the default value, currently 25000, is in effect when null is returned).
        /// </summary>
        [JsonPropertyName("max_presences")]
        public Optional<int?> MaxPresences { get; }

        /// <summary>
        ///     The maximum number of members for the guild.
        /// </summary>
        [JsonPropertyName("max_members")]
        public Optional<int> MaxMembers { get; }

        /// <summary>
        ///     The vanity url code for the guild.
        /// </summary>
        [JsonPropertyName("vanity_url_code")]
        public string? VanityUrlCode { get; }

        /// <summary>
        ///     The description of a Community guild.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; }

        /// <summary>
        ///     Banner hash.
        /// </summary>
        [JsonPropertyName("banner")]
        public string? Banner { get; }

        /// <summary>
        ///     Premium tier (Server Boost level).
        /// </summary>
        [JsonPropertyName("premium_tier")]
        public int PremiumTier { get; }

        /// <summary>
        ///     The number of boosts this guild currently has.
        /// </summary>
        [JsonPropertyName("premium_subscription_count")]
        public Optional<int> PremiumSubscriptionCount { get; }

        /// <summary>
        ///     The preferred locale of a Community guild; used in server discovery and notices from Discord; defaults to "en-US".
        /// </summary>
        [JsonPropertyName("preferred_locale")]
        public string PreferredLocale { get; }

        /// <summary>
        ///     The id of the channel where admins and moderators of Community guilds receive notices from Discord.
        /// </summary>
        [JsonPropertyName("public_updates_channel_id")]
        public Snowflake? PublicUpdatesChannelId { get; }

        /// <summary>
        ///     The maximum amount of users in a video channel.
        /// </summary>
        [JsonPropertyName("max_video_channel_users")]
        public Optional<int> MaxVideoChannelUsers { get; }

        /// <summary>
        ///     Approximate number of members in this guild.
        /// </summary>
        [JsonPropertyName("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; }

        /// <summary>
        ///     Approximate number of non-offline members in this guild.
        /// </summary>
        [JsonPropertyName("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; }

        /// <summary>
        ///     The welcome screen of a Community guild, shown to new members, returned in an Invite's guild object.
        /// </summary>
        [JsonPropertyName("welcome_screen")]
        public Optional<WelcomeScreen> WelcomeScreen { get; }

        /// <summary>
        ///     Guild NSFW level.
        /// </summary>
        [JsonPropertyName("nsfw_level")]
        public int NsfwLevel { get; }

        /// <summary>
        ///     Stage instances in the guild.
        /// </summary>
        [JsonPropertyName("stage_instances")]
        public Optional<StageInstance[]> StageInstances { get; }
    }
}
