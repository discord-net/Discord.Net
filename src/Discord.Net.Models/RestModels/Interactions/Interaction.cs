using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Interaction :
    IInteractionModel,
    IModelSource,
    IModelSourceOf<IChannelModel?>,
    IModelSourceOf<IUserModel?>,
    IModelSourceOf<IMemberModel?>,
    IModelSourceOf<IMessageModel?>
{
    [JsonPropertyName("id")] public ulong Id { get; set; }

    [JsonPropertyName("application_id")] public ulong ApplicationId { get; set; }

    [JsonPropertyName("type")] public int Type { get; set; }

    [
        JsonIgnore,
        JsonPropertyName("data"),
        DiscriminatedUnion(nameof(Type)),
        DiscriminatedUnionEntry<ApplicationCommandData>(
            InteractionDataTypes.ApplicationCommand,
            InteractionDataTypes.ApplicationCommandAutocomplete
        ),
        DiscriminatedUnionEntry<MessageComponentData>(InteractionDataTypes.MessageComponent),
        DiscriminatedUnionEntry<ModalSubmitData>(InteractionDataTypes.ModalSubmit),
    ]
    public Optional<InteractionData> Data { get; set; }

    [JsonPropertyName("guild_id")] public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("channel")] public Optional<Channel> Channel { get; set; }

    [JsonPropertyName("channel_id")] public Optional<ulong> ChannelId { get; set; }

    [JsonPropertyName("member")] public Optional<GuildMember> Member { get; set; }

    [JsonPropertyName("user")] public Optional<User> User { get; set; }

    [JsonPropertyName("token")] public required string Token { get; set; }

    [JsonPropertyName("version")] public int Version { get; set; }

    [JsonPropertyName("message")] public Optional<Message> Message { get; set; }

    [JsonPropertyName("app_permissions")] public Optional<string> AppPermission { get; set; }

    [JsonPropertyName("locale")] public Optional<string> UserLocale { get; set; }

    [JsonPropertyName("guild_locale")] public Optional<string> GuildLocale { get; set; }

    [JsonPropertyName("entitlements")]
    public required Entitlement[] Entitlements { get; set; }

    [JsonPropertyName("authorizing_integration_owners")]
    public required ApplicationIntegrationTypes AuthorizingIntegrationOwners { get; set; }

    [JsonPropertyName("context")]
    public int Context { get; set; }

    IInteractionDataModel? IInteractionModel.Data => ~Data;
    ulong? IInteractionModel.GuildId => GuildId.ToNullable();
    ulong? IInteractionModel.ChannelId => ChannelId.ToNullable();
    ulong? IInteractionModel.UserId => User.Map(v => v.Id).ToNullable();
    ulong? IInteractionModel.MessageId => Message.Map(v => v.Id).ToNullable();
    string? IInteractionModel.AppPermissions => ~AppPermission;
    string? IInteractionModel.Locale => ~UserLocale;
    string? IInteractionModel.GuildLocale => ~GuildLocale;
    IEnumerable<IEntitlementModel> IInteractionModel.Entitlements => Entitlements;
    IApplicationIntegrationTypes? IInteractionModel.AuthorizingIntegrationOwners => AuthorizingIntegrationOwners;
    int IInteractionModel.Context => Context;

    public IEnumerable<IModel> GetDefinedModels()
    {
        if (Channel.IsSpecified)
            yield return Channel.Value;

        if (Member.IsSpecified)
            yield return Member.Value;

        if (User.IsSpecified)
            yield return User.Value;

        if (Message.IsSpecified)
            yield return Message.Value;
    }

    IChannelModel? IModelSourceOf<IChannelModel?>.Model => ~Channel;

    IUserModel? IModelSourceOf<IUserModel?>.Model => ~User;

    IMemberModel? IModelSourceOf<IMemberModel?>.Model => ~Member;

    IMessageModel? IModelSourceOf<IMessageModel?>.Model => ~Message;
}
