using Discord.Models.Json;

namespace Discord;

public sealed class ModelCallbackDataProperties :
    ICallbackDataProperties
{
    public required string CustomId { get; set; }
    public required string Title { get; set; }
    public required IEnumerable<IMessageComponent> Components { get; set; } 
    
    public InteractionCallbackData ToApiModel(InteractionCallbackData? existing = default)
    {
        return new InteractionCallbackData()
        {
            CustomId = CustomId,
            Title = Title,
            Components = Optional.Some(
                Components.Select(v => v.ToApiModel()).ToArray()
            )
        };
    }
}