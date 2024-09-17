using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionCallbackResource : 
    IInteractionCallbackResourceModel
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("activity_instance")]
    public Optional<InteractionCallbackActivityInstance> ActivityInstance { get; set; }
    
    [JsonPropertyName("message")]
    public Optional<Message> Message { get; set; }

    IActivityInstanceModel? IInteractionCallbackResourceModel.ActivityInstance => ~ActivityInstance;

    IMessageModel? IInteractionCallbackResourceModel.Message => ~Message;
}