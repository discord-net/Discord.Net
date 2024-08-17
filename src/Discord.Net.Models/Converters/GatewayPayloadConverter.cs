using Discord.Models;
using Discord.Models.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Converters;

public sealed class GatewayPayloadConverter : JsonConverter<GatewayMessage>
{
    public Dictionary<string, Type> DispatchToPayload = new()
    {
        {"READY", typeof(ReadyPayloadData)},
        {"APPLICATION_COMMAND_PERMISSIONS_UPDATE", typeof(ApplicationCommandPermissionUpdated)},
        {"AUTO_MODERATION_RULE_CREATE", typeof(AutoModerationRulePayload)},
        {"AUTO_MODERATION_RULE_UPDATE", typeof(AutoModerationRulePayload)},
        {"AUTO_MODERATION_RULE_DELETE", typeof(AutoModerationRulePayload)},
        {"AUTO_MODERATION_ACTION_EXECUTION", typeof(AutoModerationActionExecuted)},
        {"CHANNEL_CREATE", typeof(ChannelPayload)},
        {"CHANNEL_UPDATE", typeof(ChannelPayload)},
        {"CHANNEL_DELETE", typeof(ChannelPayload)},
        {"THREAD_CREATE", typeof(ThreadCreated)},
        {"THREAD_LIST_SYNC", typeof(ThreadListSynced)},
        {"THREAD_MEMBER_UPDATE", typeof(ThreadMemberUpdated)},
        {"THREAD_MEMBERS_UPDATE", typeof(ThreadMembersUpdated)},
        {"CHANNEL_PINS_UPDATE", typeof(ChannelPinsUpdated)},
        {"ENTITLEMENT_CREATE", typeof(EntitlementPayload)},
        {"ENTITLEMENT_UPDATE", typeof(EntitlementPayload)},
        {"ENTITLEMENT_DELETE", typeof(EntitlementPayload)},
        {"GUILD_CREATE", typeof(GuildCreated)},
        {"GUILD_UPDATE", typeof(GuildUpdated)},
        {"GUILD_DELETE", typeof(UnavailableGuild)},
        {"GUILD_AUDIT_LOG_ENTRY_CREATE", typeof(GuildAuditLogEntryCreated)},
        {"GUILD_BAN_ADD", typeof(GuildBanPayload)},
        {"GUILD_BAN_REMOVE", typeof(GuildBanPayload)},
        {"GUILD_EMOJIS_UPDATE", typeof(GuildEmotesUpdated)},
        {"GUILD_STICKERS_UPDATE", typeof(GuildStickersUpdated)},
        {"GUILD_INTEGRATIONS_UPDATE", typeof(GuildIntegrationsUpdated)},
        {"GUILD_MEMBER_ADD", typeof(GuildMemberAdded)},
        {"GUILD_MEMBER_REMOVE", typeof(GuildMemberRemoved)},
        {"GUILD_MEMBER_UPDATE", typeof(GuildMemberUpdated)},
        {"GUILD_MEMBER_CHUNK", typeof(GuildMembersChunk)},
        {"GUILD_ROLE_CREATE", typeof(GuildRoleCreatedUpdated)},
        {"GUILD_ROLE_UPDATE", typeof(GuildRoleCreatedUpdated)},
        {"GUILD_ROLE_DELETE", typeof(GuildRoleDeleted)},
        {"GUILD_SCHEDULED_EVENT_CREATE", typeof(GuildScheduledEventPayload)},
        {"GUILD_SCHEDULED_EVENT_UPDATE", typeof(GuildScheduledEventPayload)},
        {"GUILD_SCHEDULED_EVENT_DELETE", typeof(GuildScheduledEventPayload)},
        {"GUILD_SCHEDULED_EVENT_USER_ADD", typeof(GuildScheduledEventUserPayload)},
        {"GUILD_SCHEDULED_EVENT_USER_REMOVE", typeof(GuildScheduledEventUserPayload)},
        {"INTEGRATION_CREATE", typeof(IntegrationCreatedUpdated)},
        {"INTEGRATION_UPDATE", typeof(IntegrationCreatedUpdated)},
        {"INTEGRATION_DELETE", typeof(IntegrationDeleted)},
        {"INVITE_CREATE", typeof(InviteCreated)},
        {"INVITE_DELETE", typeof(InviteDeleted)},
        {"MESSAGE_CREATE", typeof(MessageCreatedUpdated)},
        {"MESSAGE_UPDATE", typeof(MessageCreatedUpdated)},
        {"MESSAGE_DELETE", typeof(MessageDeleted)},
        {"MESSAGE_DELETE_BULK", typeof(BulkMessageDeleted)},
        {"MESSAGE_REACTION_ADD", typeof(ReactionAdded)},
        {"MESSAGE_REACTION_REMOVE", typeof(ReactionRemoved)},
        {"MESSAGE_REACTION_REMOVE_ALL", typeof(AllReactionsRemoved)},
        {"MESSAGE_REACTION_REMOVE_EMOJI", typeof(ReactionRemovedEmoji)},
        {"PRESENCE_UPDATE", typeof(PresenceUpdated)},
        {"TYPING_START", typeof(TypingStarted)},
        {"USER_UPDATE", typeof(UserUpdated)},
        {"VOICE_STATE_UPDATE", typeof(VoiceStateUpdated)},
        {"VOICE_SERVER_UPDATE", typeof(VoiceServerUpdated)},
        {"WEBHOOKS_UPDATE", typeof(WebhookUpdated)},
        {"INTERACTION_CREATE", typeof(InteractionCreated)},
        {"STAGE_INSTANCE_CREATE", typeof(StageInstancePayload)},
        {"STAGE_INSTANCE_UPDATE", typeof(StageInstancePayload)},
        {"STAGE_INSTANCE_DELETE", typeof(StageInstancePayload)},
        {"MESSAGE_POLL_VOTE_ADD", typeof(MessagePollVotePayload)},
        {"MESSAGE_POLL_VOTE_REMOVE", typeof(MessagePollVotePayload)},
    };

    public Dictionary<GatewayOpCode, Type?> OpCodeToPayload = new()
    {
        {GatewayOpCode.Heartbeat, typeof(HeartbeatPayloadData)},
        {GatewayOpCode.Identify, typeof(IdentityPayloadData)},
        {GatewayOpCode.PresenceUpdate, typeof(PresenceUpdatePayloadData)},
        {GatewayOpCode.VoiceStateUpdate, typeof(UpdateVoiceStatePayloadData)},
        {GatewayOpCode.Resume, typeof(ResumePayloadData)},
        {GatewayOpCode.Reconnect, null},
        {GatewayOpCode.RequestGuildMembers, typeof(RequestGuildMembersPayloadData)},
        {GatewayOpCode.InvalidSession, typeof(InvalidSessionPayloadData)},
        {GatewayOpCode.Hello, typeof(HelloPayloadData)},
        {GatewayOpCode.HeartbeatAck, null},
    };

    private static JsonTypeInfo<GatewayMessage>? _gatewayMessageTypeInfo;

    private static JsonTypeInfo<GatewayMessage> GetNonConverterTypeInfo(JsonSerializerOptions options)
    {
        if (_gatewayMessageTypeInfo is not null)
            return _gatewayMessageTypeInfo;

        if (options.TypeInfoResolver is ModelJsonContext modelJsonContext)
            return _gatewayMessageTypeInfo = modelJsonContext.CreateGatewayMessageTypeInfoNoConverter(options);

        return _gatewayMessageTypeInfo = options.TypeInfoResolverChain
            .OfType<ModelJsonContext>()
            .First()
            .CreateGatewayMessageTypeInfoNoConverter(options);
    }

    public override GatewayMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var gatewayMessageJson = jsonDoc.RootElement;

        var message = gatewayMessageJson.Deserialize(GetNonConverterTypeInfo(options));

        if (message is null)
            return null;

        if (!gatewayMessageJson.TryGetProperty("d", out var payloadData)) return message;

        var payloadType = message.OpCode switch
        {
            GatewayOpCode.Dispatch when message.EventName is {IsSpecified: true, Value: not null}
                => DispatchToPayload.GetValueOrDefault(message.EventName.Value),
            _ => OpCodeToPayload.GetValueOrDefault(message.OpCode)
        };

        if (payloadType is not null)
            message.Payload = payloadData.Deserialize(payloadType, options) as IGatewayPayloadData;

        return message;
    }

    public override void Write(Utf8JsonWriter writer, GatewayMessage? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        writer.WriteNumber("op", (byte) value.OpCode);
        if (value.Sequence is {IsSpecified: true, Value: not null})
            writer.WriteNumber("s", value.Sequence.Value.Value);

        if (
            value.Payload is not null &&
            OpCodeToPayload.TryGetValue(value.OpCode, out var payloadType) &&
            payloadType is not null)
        {
            writer.WritePropertyName("d");
            var typeInfo = options.GetTypeInfo(payloadType);
            JsonSerializer.Serialize(writer, value.Payload, typeInfo);
        }

        writer.WriteEndObject();
    }
}