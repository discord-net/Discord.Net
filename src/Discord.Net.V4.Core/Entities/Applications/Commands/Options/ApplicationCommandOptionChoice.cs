using System.Collections.Frozen;
using System.Globalization;
using Discord.Models;

namespace Discord;

public readonly struct ApplicationCommandOptionChoice<T>(
    IApplicationCommandOptionChoiceModel<T> model
) :
    IModelConstructable<
        ApplicationCommandOptionChoice<T>,
        IApplicationCommandOptionChoiceModel<T>
    >,
    IEntityProperties<IApplicationCommandOptionChoiceModel<T>>
    where T : notnull
{
    public string Name { get; } = model.Name;

    public IReadOnlyDictionary<CultureInfo, string> NameLocalizations { get; } =
        model.NameLocalization?
            .ToFrozenDictionary(
                x => CultureInfo.GetCultureInfo(x.Key),
                x => x.Value
            ) ?? FrozenDictionary<CultureInfo, string>.Empty;

    public T Value { get; } = model.Value;
    
    public static ApplicationCommandOptionChoice<T> Construct(
        IDiscordClient client,
        IApplicationCommandOptionChoiceModel<T> model
    ) => new(model);

    public IApplicationCommandOptionChoiceModel<T> ToApiModel(
        IApplicationCommandOptionChoiceModel<T>? existing = default
    ) => model;
}