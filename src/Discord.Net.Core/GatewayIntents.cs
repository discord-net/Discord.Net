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
        ///     This intent includes all but <see cref="GuildMembers"/> and <see cref="GuildMembers"/>
        ///     that are privileged must be enabled for the application.
        /// </summary>
        AllUnprivileged = Guilds | GuildBans | GuildEmojis | GuildIntegrations | GuildWebhooks | GuildInvites |
            GuildVoiceStates | GuildMessages | GuildMessageReactions | GuildMessageTyping | DirectMessages |
            DirectMessageReactions | DirectMessageTyping,
        /// <summary>
        ///     This intent includes all of them, including privileged ones.
        /// </summary>
        All = AllUnprivileged | GuildMembers | GuildPresences
    }
}
