using Discord.Models;

namespace Discord;

public partial interface IInteractionCallbackResponse : 
    ISnowflakeEntity<IInteractionCallbackResponseModel>,
    IInteractionMessageActor
{
    InteractionType Type { get; }
    InteractionResponseType ResponseType { get; }
    string? ActivityInstanceId { get; }
    IMessageActor? Message { get; }
    bool IsResponseMessageLoading { get; }
    bool IsResponseMessageEphemeral { get; }
}