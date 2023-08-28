using System;

namespace Discord.Gateway
{
    public static class EventNames
    {
        #region Interactions
        public const string ApplicationCommandPermissionUpdate = "APPLICATION_COMMAND_PERMISSIONS_UPDATE";
        public const string InteractionCreate = "INTERACTION_CREATE";
        #endregion

        #region Automod
        public const string AutoModerationRuleCreate = "AUTO_MODERATION_RULE_CREATE";
        public const string AutoModerationRuleUpdate = "AUTO_MODERATION_RULE_UPDATE";
        public const string AutoModerationRuleDelete = "AUTO_MODERATION_RULE_DELETE";
        public const string AutoModerationActionExecution = "AUTO_MODERATION_ACTION_EXECUTION";
        #endregion

        #region Channel
        public const string ChannelCreate = "CHANNEL_CREATE";
        public const string ChannelUpdate = "CHANNEL_UPDATE";
        public const string ChannelDelete = "CHANNEL_DELETE";

        public const string ChannelPinsUpdated = "CHANNEL_PINS_UPDATED";
        #endregion

        #region Threads
        public const string ThreadCreate = "THREAD_CREATE";
        public const string ThreadUpdate = "THREAD_UPDATE";
        public const string ThreadDelete = "THREAD_DELETE";
        public const string ThreadListSync = "THREAD_LIST_SYNC";
        public const string ThreadMemberUpdate = "THREAD_MEMBER_UPDATE";
        public const string ThreadMembersUpdate = "THREAD_MEMBERS_UPDATE";
        #endregion

        #region Guild
        public const string GuildCreate = "GUILD_CREATE";
        public const string GuildUpdate = "GUILD_UPDATE";
        public const string GuildDelete = "GUILD_DELETE";

        public const string GuildAuditLogEntryCreate = "GUILD_AUDIT_LOG_ENTRY_CREATE";

        public const string GuildBanAdd = "GUILD_BAN_ADD";
        public const string GuildBanRemove = "GUILD_BAN_REMOVE";

        public const string GuildEmojisUpdate = "GUILD_EMOJIS_UPDATE";
        public const string GuildStickersUpdate = "GUILD_STICKERS_UPDATE";
        public const string GuildIntegrationsUpdate = "GUILD_INTEGRATIONS_UPDATE";

        public const string GuildMemberAdd = "GUILD_MEMBER_ADD";
        public const string GuildMemberRemove = "GUILD_MEMBER_REMOVE";
        public const string GuildMemberUpdate = "GUILD_MEMBER_UPDATE";
        public const string GuildMembersChunk = "GUILD_MEMBERS_CHUNK";

        public const string GuildRoleCreate = "GUILD_ROLE_CREATE";
        public const string GuildRoleUpdate = "GUILD_ROLE_UPDATE";
        public const string GuildRoleDelete = "GUILD_ROLE_DELETE";

        public const string GuildScheduledEventCreate = "GUILD_SCHEDULED_EVENT_CREATE";
        public const string GuildScheduledEventUpdate = "GUILD_SCHEDULED_EVENT_UPDATE";
        public const string GuildScheduledEventDelete = "GUILD_SCHEDULED_EVENT_DELETE";

        public const string GuildScheduledEventUserAdd = "GUILD_SCHEDULED_EVENT_USER_ADD";
        public const string GuildScheduledEventUserRemove = "GUILD_SCHEDULED_EVENT_USER_REMOVE";

        public const string IntegrationCreate = "INTEGRATION_CREATE";
        public const string IntegrationUpdate = "INTEGRATION_UPDATE";
        public const string IntegrationDelete = "INTEGRATION_DELETE";
        #endregion

        #region Invites
        public const string InviteCreate = "INVITE_CREATE";
        public const string InviteDelete = "INVITE_DELETE";
        #endregion

        #region Messages
        public const string MessageCreate = "MESSAGE_CREATE";
        public const string MessageUpdate = "MESSAGE_UPDATE";
        public const string MessageDelete = "MESSAGE_DELETE";
        public const string MessageDeleteBulk = "MESSAGE_DELETE_BULK";

        public const string MessageReactionAdd = "MESSAGE_REACTION_ADD";
        public const string MessageReactionRemove = "MESSAGE_REACTION_REMOVE";
        public const string MessageReactionRemoveAll = "MESSAGE_REACTION_REMOVE_ALL";
        public const string MessageReactionRemoveEmoji = "MESSAGE_REACTION_REMOVE_EMOJI";
        #endregion

        #region Presence
        public const string PresenceUpdate = "PRESENCE_UPDATE";
        #endregion

        #region Stage Instance
        public const string StageInstanceCreate = "STAGE_INSTANCE_CREATE";
        public const string StageInstanceUpdate = "STAGE_INSTANCE_UPDATE";
        public const string StageInstanceDelete = "STAGE_INSTANCE_DELETE";
        #endregion

        #region Typing
        public const string TypingStart = "TYPING_START";
        #endregion

        #region User
        public const string UserUpdated = "USER_UPDATED";
        #endregion

        #region Voice
        public const string VoiceStateUpdate = "VOICE_STATE_UPDATE";
        public const string VoiceServerUpdate = "VOICE_SERVER_UPDATE";
        #endregion

        #region Webhook
        public const string WebhooksUpdate = "WEBHOOKS_UPDATE";
        #endregion
    }
}

