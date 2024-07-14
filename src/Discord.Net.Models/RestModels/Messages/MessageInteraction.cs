using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MessageInteractionMetadata :
    IModelSource,
    IMessageInteractionMetadataModel,
    IModelSourceOf<IUserModel>
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("authorizing_integration_owners")]
    public required Dictionary<int, ulong> AuthorizingIntegrationOwners { get; set; }

    [JsonPropertyName("original_response_message_id")]
    public Optional<ulong> OriginalResponseMessageId { get; set; }

    [JsonPropertyName("interacted_message_id")]
    public Optional<ulong> InteractedMessageId { get; set; }

    [JsonPropertyName("triggering_interaction_metadata")]
    public Optional<MessageInteractionMetadata> TriggeringInteractionMetadata { get; set; }


    ulong? IMessageInteractionMetadataModel.OriginalResponseMessageId => OriginalResponseMessageId;
    ulong? IMessageInteractionMetadataModel.InteractedMessageId => InteractedMessageId;

    IMessageInteractionMetadataModel? IMessageInteractionMetadataModel.TriggeringInteractionMetadata =>
        ~TriggeringInteractionMetadata;

    IDictionary<int, ulong> IMessageInteractionMetadataModel.AuthorizingIntegrationOwners =>
        AuthorizingIntegrationOwners;
    ulong IMessageInteractionMetadataModel.UserId => User.Id;

    public IEnumerable<IEntityModel> GetDefinedModels()
    {
        yield return User;
    }

    IUserModel IModelSourceOf<IUserModel>.Model => User;
}
