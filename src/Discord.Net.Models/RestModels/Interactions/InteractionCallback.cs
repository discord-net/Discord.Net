using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionCallback : IInteractionCallbackModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }
    
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("activity_instance_id")]
    public Optional<string> ActivityInstanceId { get; set; }
    
    [JsonPropertyName("response_message_id")]
    public Optional<ulong> ResponseMessageId { get; set; }
    
    [JsonPropertyName("response_message_loading")]
    public Optional<bool> ResponseMessageLoading { get; set; }
    
    [JsonPropertyName("response_message_ephemeral")]
    public Optional<bool> ResponseMessageEphemeral { get; set; }

    string? IInteractionCallbackModel.ActivityInstanceId => ~ActivityInstanceId;

    ulong? IInteractionCallbackModel.ResponseMessageId => ResponseMessageId.ToNullable();

    bool? IInteractionCallbackModel.ResponseMessageLoading => ResponseMessageLoading.ToNullable();

    bool? IInteractionCallbackModel.ResponseMessageEphemeral => ResponseMessageEphemeral.ToNullable();
}