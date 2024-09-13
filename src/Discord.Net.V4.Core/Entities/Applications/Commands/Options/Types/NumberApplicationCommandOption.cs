using Discord.Models;

namespace Discord;

public sealed class NumberApplicationCommandOption(
    IDiscordClient client,
    INumberApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<INumberApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;

    public IReadOnlyCollection<ApplicationCommandOptionChoice<double>> Choices { get; } =
        (IReadOnlyCollection<ApplicationCommandOptionChoice<double>>?) model.Choices
            ?.Select(option =>
                new ApplicationCommandOptionChoice<double>(option)
            )
            .ToList()
            .AsReadOnly()
        ?? Array.Empty<ApplicationCommandOptionChoice<double>>();

    public double? MinValue => Model.MinValue;
    
    public double? MaxValue => Model.MaxValue;
    
    public bool Autocomplete => Model.Autocomplete ?? false;
    
    protected override INumberApplicationCommandOptionModel Model => model;

    public override INumberApplicationCommandOptionModel GetModel()
        => Model;
}