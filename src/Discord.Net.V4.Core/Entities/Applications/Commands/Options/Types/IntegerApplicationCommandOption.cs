using Discord.Models;

namespace Discord;

public sealed class IntegerApplicationCommandOption(
    IDiscordClient client,
    IIntegerApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<IIntegerApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;

    public IReadOnlyCollection<ApplicationCommandOptionChoice<long>> Choices { get; } =
        (IReadOnlyCollection<ApplicationCommandOptionChoice<long>>?) model.Choices
            ?.Select(option =>
                new ApplicationCommandOptionChoice<long>(option)
            )
            .ToList()
            .AsReadOnly()
        ?? Array.Empty<ApplicationCommandOptionChoice<long>>();

    public long? MinValue => Model.MinValue;
    
    public long? MaxValue => Model.MaxValue;
    
    public bool Autocomplete => Model.Autocomplete ?? false;
    
    protected override IIntegerApplicationCommandOptionModel Model => model;

    public override IIntegerApplicationCommandOptionModel GetModel()
        => Model;
}