using System;

namespace Discord
{
    [Flags]
    public enum GatewayIntents
    {
        /// <summary> This intent includes no events </summary>
        None = 0,
        /// <summary> This intent includes GUILD_CREATE, GUILD_UPDATE, GUILD_DELETE, GUILD_ROLE_CREATE, GUILD_ROLE_UPDATE, GUILD_ROLE_DELETE, CHANNEL_CREATE, CHANNEL_UPDATE, CHANNEL_DELETE, CHANNEL_PINS_UPDATE </summary>
        Guilds = 1 << 0,
        /// <summary> This intent includes GUILD_MEMBER_ADD, GUILD_MEMBER_UPDATE, GUILD_MEMBER_REMOVE </summary>
        /// <remarks> This is a privileged intent and must be enabled in the Developer Portal. </remarks>
        GuildMembers = 1 << 1,
        /// <summary> This intent includes GUILD_BAN_ADD, GUILD_BAN_REMOVE </summary>
        GuildBans = 1 << 2,
        /// <summary> This intent includes GUILD_EMOJIS_UPDATE </summary>
        GuildEmojis = 1 << 3,
        /// <summary> This intent includes GUILD_INTEGRATIONS_UPDATE </summary>
        GuildIntegrations = 1 << 4,
        /// <summary> This intent includes WEBHOOKS_UPDATE </summary>
        GuildWebhooks = 1 << 5,
        /// <summary> This intent includes INVITE_CREATE, INVITE_DELETE </summary>
        GuildInvites = 1 << 6,
        /// <summary> This intent includes VOICE_STATE_UPDATE </summary>
        GuildVoiceStates = 1 << 7,
        /// <summary> This intent includes PRESENCE_UPDATE </summary>
        /// <remarks> This is a privileged intent and must be enabled in the Developer Portal. </remarks>
        GuildPresences = 1 << 8,
        /// <summary> This intent includes MESSAGE_CREATE, MESSAGE_UPDATE, MESSAGE_DELETE, MESSAGE_DELETE_BULK </summary>
        GuildMessages = 1 << 9,
        /// <summary> This intent includes MESSAGE_REACTION_ADD, MESSAGE_REACTION_REMOVE, MESSAGE_REACTION_REMOVE_ALL, MESSAGE_REACTION_REMOVE_EMOJI </summary>
        GuildMessageReactions = 1 << 10,
        /// <summary> This intent includes TYPING_START </summary>
        GuildMessageTyping = 1 << 11,
        /// <summary> This intent includes CHANNEL_CREATE, MESSAGE_CREATE, MESSAGE_UPDATE, MESSAGE_DELETE, CHANNEL_PINS_UPDATE </summary>
        DirectMessages = 1 << 12,
        /// <summary> This intent includes MESSAGE_REACTION_ADD, MESSAGE_REACTION_REMOVE, MESSAGE_REACTION_REMOVE_ALL, MESSAGE_REACTION_REMOVE_EMOJI </summary>
        DirectMessageReactions = 1 << 13,
        /// <summary> This intent includes TYPING_START </summary>
        DirectMessageTyping = 1 << 14,
        /// <summary>
        ///     This intent defines if the content within messages received by MESSAGE_CREATE is available or not.
        ///     This is a privileged intent and needs to be enabled in the developer portal. 
        /// </summary>
        MessageContent = 1 << 15,
        /// <summary>
        ///     This intent includes GUILD_SCHEDULED_EVENT_CREATE, GUILD_SCHEDULED_EVENT_UPDATE, GUILD_SCHEDULED_EVENT_DELETE, GUILD_SCHEDULED_EVENT_USER_ADD, GUILD_SCHEDULED_EVENT_USER_REMOVE
        /// </summary>
        GuildScheduledEvents = 1 << 16,

        /// <summary>
        ///     This intent includes AUTO_MODERATION_RULE_CREATE, AUTO_MODERATION_RULE_UPDATE, AUTO_MODERATION_RULE_DELETE
        /// </summary>
        AutoModerationConfiguration = 1 << 20,

        /// <summary>
        ///     This intent includes AUTO_MODERATION_ACTION_EXECUTION
        /// </summary>
        AutoModerationActionExecution = 1 << 21,

        /// <summary>
        ///     This intent includes MESSAGE_POLL_VOTE_ADD and MESSAGE_POLL_VOTE_REMOVE
        /// </summary>
        GuildMessagePolls = 1 << 24,

        /// <summary>
        ///     This intent includes MESSAGE_POLL_VOTE_ADD and MESSAGE_POLL_VOTE_REMOVE
        /// </summary>
        DirectMessagePolls = 1 << 25,

        /// <summary>
        ///     This intent includes all but <see cref="GuildMembers"/>, <see cref="GuildPresences"/> and <see cref="MessageContent"/>
        ///     which are privileged and must be enabled in the Developer Portal.
        /// </summary>
        AllUnprivileged = Guilds | GuildBans | GuildEmojis | GuildIntegrations | GuildWebhooks | GuildInvites |
            GuildVoiceStates | GuildMessages | GuildMessageReactions | GuildMessageTyping | DirectMessages |
            DirectMessageReactions | DirectMessageTyping | GuildScheduledEvents | AutoModerationConfiguration |
            AutoModerationActionExecution | GuildMessagePolls | DirectMessagePolls,
        /// <summary>
        ///     This intent includes all of them, including privileged ones.
        /// </summary>
        All = AllUnprivileged | GuildMembers | GuildPresences | MessageContent
    }
}
