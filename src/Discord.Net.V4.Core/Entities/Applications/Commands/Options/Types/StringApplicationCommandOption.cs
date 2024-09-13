using Discord.Models;

namespace Discord;

public sealed class StringApplicationCommandOption(
    IDiscordClient client,
    IStringApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<IStringApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;

    public IReadOnlyCollection<ApplicationCommandOptionChoice<string>> Choices { get; } =
        (IReadOnlyCollection<ApplicationCommandOptionChoice<string>>?) model.Choices
            ?.Select(option =>
                new ApplicationCommandOptionChoice<string>(option)
            )
            .ToList()
            .AsReadOnly()
        ?? Array.Empty<ApplicationCommandOptionChoice<string>>();

    public long? MinLength => Model.MinLength;
    
    public long? MaxLength => Model.MaxLength;
    
    public bool Autocomplete => Model.Autocomplete ?? false;
    
    protected override IStringApplicationCommandOptionModel Model => model;

    public override IStringApplicationCommandOptionModel GetModel()
        => Model;
}