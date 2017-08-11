using Discord.API.Gateway;
using Discord.Serialization.Json.Converters;
using System;

namespace Discord.Serialization.Json
{
    public class DiscordSocketJsonSerializer : JsonSerializer
    {
        private static readonly Lazy<DiscordSocketJsonSerializer> _singleton = new Lazy<DiscordSocketJsonSerializer>();
        public static DiscordSocketJsonSerializer Global => _singleton.Value;

        public DiscordSocketJsonSerializer()
            : this((JsonSerializer)null) { }
        public DiscordSocketJsonSerializer(JsonSerializer parent)
            : base(parent ?? DiscordRestJsonSerializer.Global)
        {
            AddSelectorConverter<GatewayOpCode, ObjectPropertyConverter<HelloEvent>>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.Hello);
            AddSelectorConverter<GatewayOpCode, BooleanPropertyConverter>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.InvalidSession);

            AddSelectorConverter<GatewayOpCode, Int32PropertyConverter>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.Heartbeat);
            AddSelectorConverter<GatewayOpCode, Int32PropertyConverter>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.HeartbeatAck);
            AddSelectorConverter<GatewayOpCode, ObjectPropertyConverter<IdentifyParams>>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.Identify);
            AddSelectorConverter<GatewayOpCode, ObjectPropertyConverter<ResumeParams>>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.Resume);
            AddSelectorConverter<GatewayOpCode, ObjectPropertyConverter<StatusUpdateParams>>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.StatusUpdate);
            AddSelectorConverter<GatewayOpCode, ObjectPropertyConverter<RequestMembersParams>>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.RequestGuildMembers);
            AddSelectorConverter<GatewayOpCode, ObjectPropertyConverter<VoiceStateUpdateParams>>(
                ModelSelectorGroups.GatewayFrame, GatewayOpCode.VoiceStateUpdate);

            AddSelectorConverter<string, ObjectPropertyConverter<ReadyEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "READY");

            AddSelectorConverter<string, ObjectPropertyConverter<ExtendedGuild>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_CREATE");
            AddSelectorConverter<string, ObjectPropertyConverter<API.Guild>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_UPDATE");
            AddSelectorConverter<string, ObjectPropertyConverter<GuildEmojiUpdateEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_EMOJIS_UPDATE");
            AddSelectorConverter<string, ObjectPropertyConverter<GuildSyncEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_SYNC");
            AddSelectorConverter<string, ObjectPropertyConverter<ExtendedGuild>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_DELETE");

            AddSelectorConverter<string, ObjectPropertyConverter<API.Channel>>(
                ModelSelectorGroups.GatewayDispatchFrame, "CHANNEL_CREATE");
            AddSelectorConverter<string, ObjectPropertyConverter<API.Channel>>(
                ModelSelectorGroups.GatewayDispatchFrame, "CHANNEL_UPDATE");
            AddSelectorConverter<string, ObjectPropertyConverter<API.Channel>>(
                ModelSelectorGroups.GatewayDispatchFrame, "CHANNEL_DELETE");

            AddSelectorConverter<string, ObjectPropertyConverter<GuildMemberAddEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_MEMBER_ADD");
            AddSelectorConverter<string, ObjectPropertyConverter<GuildMemberUpdateEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_MEMBER_UPDATE");
            AddSelectorConverter<string, ObjectPropertyConverter<GuildMemberRemoveEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_MEMBER_REMOVE");
            AddSelectorConverter<string, ObjectPropertyConverter<GuildMembersChunkEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_MEMBERS_CHUNK");

            AddSelectorConverter<string, ObjectPropertyConverter<RecipientEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "CHANNEL_RECIPIENT_ADD");
            AddSelectorConverter<string, ObjectPropertyConverter<RecipientEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "CHANNEL_RECIPIENT_REMOVE");

            AddSelectorConverter<string, ObjectPropertyConverter<GuildRoleCreateEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_ROLE_CREATE");
            AddSelectorConverter<string, ObjectPropertyConverter<GuildRoleUpdateEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_ROLE_UPDATE");
            AddSelectorConverter<string, ObjectPropertyConverter<GuildRoleDeleteEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_ROLE_UPDATE");

            AddSelectorConverter<string, ObjectPropertyConverter<GuildBanEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_BAN_ADD");
            AddSelectorConverter<string, ObjectPropertyConverter<GuildBanEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "GUILD_BAN_REMOVE");

            AddSelectorConverter<string, ObjectPropertyConverter<API.Message>>(
                ModelSelectorGroups.GatewayDispatchFrame, "MESSAGE_CREATE");
            AddSelectorConverter<string, ObjectPropertyConverter<API.Message>>(
                ModelSelectorGroups.GatewayDispatchFrame, "MESSAGE_UPDATE");
            AddSelectorConverter<string, ObjectPropertyConverter<API.Message>>(
                ModelSelectorGroups.GatewayDispatchFrame, "MESSAGE_DELETE");
            AddSelectorConverter<string, ObjectPropertyConverter<MessageDeleteBulkEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "MESSAGE_DELETE_BULK");

            AddSelectorConverter<string, ObjectPropertyConverter<Reaction>>(
                ModelSelectorGroups.GatewayDispatchFrame, "MESSAGE_REACTION_ADD");
            AddSelectorConverter<string, ObjectPropertyConverter<Reaction>>(
                ModelSelectorGroups.GatewayDispatchFrame, "MESSAGE_REACTION_REMOVE");
            AddSelectorConverter<string, ObjectPropertyConverter<RemoveAllReactionsEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "MESSAGE_REACTION_REMOVE_ALL");

            AddSelectorConverter<string, ObjectPropertyConverter<API.Presence>>(
                ModelSelectorGroups.GatewayDispatchFrame, "PRESENCE_UPDATE");

            AddSelectorConverter<string, ObjectPropertyConverter<API.User>>(
                ModelSelectorGroups.GatewayDispatchFrame, "USER_UPDATE");

            AddSelectorConverter<string, ObjectPropertyConverter<TypingStartEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "TYPING_START");

            AddSelectorConverter<string, ObjectPropertyConverter<API.VoiceState>>(
                ModelSelectorGroups.GatewayDispatchFrame, "VOICE_STATE_UPDATE");
            AddSelectorConverter<string, ObjectPropertyConverter<VoiceServerUpdateEvent>>(
                ModelSelectorGroups.GatewayDispatchFrame, "VOICE_SERVER_UPDATE");
        }

        private DiscordSocketJsonSerializer(DiscordSocketJsonSerializer parent)
            : base(parent) { }
        public DiscordSocketJsonSerializer CreateScope() => new DiscordSocketJsonSerializer(this);
    }
}
