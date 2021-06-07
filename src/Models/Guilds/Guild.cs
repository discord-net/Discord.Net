using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord guild object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-object-guild-structure"/>
    /// </remarks>
    public record Guild
    {
        /// <summary>
        /// <see cref="Guild"/> id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// <see cref="Guild"/> name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// Icon hash.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; init; }

        /// <summary>
        /// Icon hash, returned when in the template object.
        /// </summary>
        [JsonPropertyName("icon_hash")]
        public Optional<string?> IconHash { get; init; }

        /// <summary>
        /// Splash hash.
        /// </summary>
        [JsonPropertyName("splash")]
        public string? Splash { get; init; }

        /// <summary>
        /// Discovery splash hash; only present for <see cref="Guild"/>s
        /// with the "DISCOVERABLE" feature.
        /// </summary>
        [JsonPropertyName("discovery_splash")]
        public string? DiscoverySplash { get; init; }

        /// <summary>
        /// True if the <see cref="User"/> is the owner of the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("owner")]
        public Optional<bool> Owner { get; init; }

        /// <summary>
        /// Id of owner.
        /// </summary>
        [JsonPropertyName("owner_id")]
        public Snowflake OwnerId { get; init; }

        /// <summary>
        /// <see cref="Models.Permissions"/> for the <see cref="User"/> in the
        /// <see cref="Guild"/> (excludes overwrites).
        /// </summary>
        [JsonPropertyName("permissions")]
        public Optional<Permissions> Permissions { get; init; }

        /// <summary>
        /// Voice region id for the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("region")]
        public string? Region { get; init; } // Required property candidate

        /// <summary>
        /// Id of afk <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("afk_channel_id")]
        public Snowflake? AfkChannelId { get; init; }

        /// <summary>
        /// Afk timeout in seconds.
        /// </summary>
        [JsonPropertyName("afk_timeout")]
        public int AfkTimeout { get; init; }

        /// <summary>
        /// True if the server widget is enabled.
        /// </summary>
        [JsonPropertyName("widget_enabled")]
        public Optional<bool> WidgetEnabled { get; init; }

        /// <summary>
        /// The <see cref="Channel"/> id that the widget will generate an invite to, or null if set to no invite.
        /// </summary>
        [JsonPropertyName("widget_channel_id")]
        public Optional<Snowflake?> WidgetChannelId { get; init; }

        /// <summary>
        /// Verification level required for the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("verification_level")]
        public VerificationLevel VerificationLevel { get; init; }

        /// <summary>
        /// Default message notifications level.
        /// </summary>
        [JsonPropertyName("default_message_notifications")]
        public DefaultMessageNotificationLevel DefaultMessageNotifications { get; init; }

        /// <summary>
        /// Explicit content filter level.
        /// </summary>
        [JsonPropertyName("explicit_content_filter")]
        public ExplicitContentFilterLevel ExplicitContentFilter { get; init; }

        /// <summary>
        /// <see cref="Role"/>s in the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("roles")]
        public Role[]? Roles { get; init; } // Required property candidate

        /// <summary>
        /// Custom <see cref="Guild"/> <see cref="Emoji"/>s.
        /// </summary>
        [JsonPropertyName("emojis")]
        public Emoji[]? Emojis { get; init; } // Required property candidate

        /// <summary>
        /// Enabled <see cref="Guild"/> features.
        /// </summary>
        [JsonPropertyName("features")]
        public string[]? Features { get; init; } // Required property candidate

        /// <summary>
        /// Required MFA level for the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("mfa_level")]
        public MfaLevel MfaLevel { get; init; }

        /// <summary>
        /// <see cref="Application"/> id of the <see cref="Guild"/> creator if it is bot-created.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Snowflake? ApplicationId { get; init; }

        /// <summary>
        /// The id of the <see cref="Channel"/> where <see cref="Guild"/> notices such
        /// as welcome messages and boost events are posted.
        /// </summary>
        [JsonPropertyName("system_channel_id")]
        public Snowflake? SystemChannelId { get; init; }

        /// <summary>
        /// System <see cref="Channel"/> flags.
        /// </summary>
        [JsonPropertyName("system_channel_flags")]
        public int SystemChannelFlags { get; init; }

        /// <summary>
        /// The id of the <see cref="Channel"/> where Community <see cref="Guild"/>s
        /// can display rules and/or guidelines.
        /// </summary>
        [JsonPropertyName("rules_channel_id")]
        public Snowflake? RulesChannelId { get; init; }

        /// <summary>
        /// When this <see cref="Guild"/> was joined at.
        /// </summary>
        [JsonPropertyName("joined_at")]
        public Optional<DateTimeOffset> JoinedAt { get; init; }

        /// <summary>
        /// True if this is considered a large <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("large")]
        public Optional<bool> Large { get; init; }

        /// <summary>
        /// True if this <see cref="Guild"/> is unavailable due to an outage.
        /// </summary>
        [JsonPropertyName("unavailable")]
        public Optional<bool> Unavailable { get; init; }

        /// <summary>
        /// Total number of <see cref="GuildMember"/>s in this <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("member_count")]
        public Optional<int> MemberCount { get; init; }

        /// <summary>
        /// States of <see cref="GuildMember"/>s currently in voice <see cref="Channel"/>s.
        /// </summary>
        /// <remarks>
        /// Lacks the <see cref="VoiceState.GuildId"/>.
        /// </remarks>
        [JsonPropertyName("voice_states")]
        public Optional<VoiceState[]> VoiceStates { get; init; }

        /// <summary>
        /// <see cref="GuildMember"/>s in the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("members")]
        public Optional<GuildMember[]> Members { get; init; }

        /// <summary>
        /// <see cref="Channel"/>s in the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("channels")]
        public Optional<Channel[]> Channels { get; init; }

        /// <summary>
        /// All active thread <see cref="Channel"/>s in the <see cref="Guild"/> that current <see cref="User"/>
        /// has permission to view.
        /// </summary>
        [JsonPropertyName("threads")]
        public Optional<Channel[]> Threads { get; init; }

        /// <summary>
        /// <see cref="Presence"/>s of the <see cref="GuildMember"/>s in the <see cref="Guild"/>,
        /// will only include non-offline <see cref="GuildMember"/>s if the size is greater than large threshold.
        /// </summary>
        [JsonPropertyName("presences")]
        public Optional<Presence[]> Presences { get; init; }

        /// <summary>
        /// The maximum number of <see cref="Presences"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// The default value, currently 25000, is in effect when null is returned.
        /// </remarks>
        [JsonPropertyName("max_presences")]
        public Optional<int?> MaxPresences { get; init; }

        /// <summary>
        /// The maximum number of <see cref="GuildMember"/>s for the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("max_members")]
        public Optional<int> MaxMembers { get; init; }

        /// <summary>
        /// The vanity url code for the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("vanity_url_code")]
        public string? VanityUrlCode { get; init; }

        /// <summary>
        /// The description of a Community <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; }

        /// <summary>
        /// Banner hash.
        /// </summary>
        [JsonPropertyName("banner")]
        public string? Banner { get; init; }

        /// <summary>
        /// Premium tier (Server Boost level).
        /// </summary>
        [JsonPropertyName("premium_tier")]
        public PremiumTier PremiumTier { get; init; }

        /// <summary>
        /// The number of boosts this <see cref="Guild"/> currently has.
        /// </summary>
        [JsonPropertyName("premium_subscription_count")]
        public Optional<int> PremiumSubscriptionCount { get; init; }

        /// <summary>
        /// The preferred locale of a Community <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// Used in server discovery and notices from Discord, defaults to "en-US".
        /// </remarks>
        [JsonPropertyName("preferred_locale")]
        public string? PreferredLocale { get; init; } // Required property candidate

        /// <summary>
        /// The id of the <see cref="Channel"/> where admins and moderators of Community
        /// <see cref="Guild"/> receive notices from Discord.
        /// </summary>
        [JsonPropertyName("public_updates_channel_id")]
        public Snowflake? PublicUpdatesChannelId { get; init; }

        /// <summary>
        /// The maximum amount of <see cref="User"/>s in a video <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("max_video_channel_users")]
        public Optional<int> MaxVideoChannelUsers { get; init; }

        /// <summary>
        /// Approximate number of <see cref="GuildMember"/>s in this <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; init; }

        /// <summary>
        /// Approximate number of non-offline <see cref="GuildMember"/>s in this <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; init; }

        /// <summary>
        /// The welcome screen of a Community <see cref="Guild"/>, shown to new <see cref="GuildMember"/>s,
        /// returned in <see cref="Invite.Guild"/>.
        /// </summary>
        [JsonPropertyName("welcome_screen")]
        public Optional<WelcomeScreen> WelcomeScreen { get; init; }

        /// <summary>
        /// Guild NSFW level.
        /// </summary>
        [JsonPropertyName("nsfw_level")]
        public GuildNsfwLevel NsfwLevel { get; init; }

        /// <summary>
        /// <see cref="StageInstance"/>s in the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("stage_instances")]
        public Optional<StageInstance[]> StageInstances { get; init; }
    }
}
