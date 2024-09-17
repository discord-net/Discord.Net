using Discord.Models.Json;

namespace Discord;

public sealed class CreateInteractionResponseProperties :
    IEntityProperties<InteractionResponse>
{
    public InteractionResponseType Type { get; set; }
    public Optional<ICallbackDataProperties> Data { get; set; }
    
    public InteractionResponse ToApiModel(InteractionResponse? existing = default)
    {
        return new InteractionResponse()
        {
            Type = (int)Type,
            Data = Data.Map(v => v.ToApiModel())
        };
    }
}