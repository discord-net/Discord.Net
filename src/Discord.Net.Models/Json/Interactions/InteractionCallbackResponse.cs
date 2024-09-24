using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionCallbackResponse : IInteractionCallbackResponseModel
{
    [JsonPropertyName("interaction")]
    public required InteractionCallback Interaction { get; set; }

    [JsonPropertyName("resource")]
    public Optional<InteractionCallbackResource> Resource { get; set; }

    IInteractionCallbackResourceModel? IInteractionCallbackResponseModel.Resource => ~Resource;
    IInteractionCallbackModel IInteractionCallbackResponseModel.Interaction => Interaction;
}