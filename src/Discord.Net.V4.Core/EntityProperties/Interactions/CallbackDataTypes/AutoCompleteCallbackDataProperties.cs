using Discord.Models;
using Discord.Models.Json;

namespace Discord;

public sealed class AutoCompleteCallbackDataProperties<TChoice> :
    ICallbackDataProperties
    where TChoice : notnull
{
    public required IEnumerable<ApplicationCommandOptionChoice<TChoice>> Choices { get; set; }

    public InteractionCallbackData ToApiModel(InteractionCallbackData? existing = default)
    {
        return new InteractionCallbackData()
        {
            Choices = Optional.Some(
                Choices.Select(v => v.ToApiModel()).ToArray<IApplicationCommandOptionChoiceModel>()
            )
        };
    }
}