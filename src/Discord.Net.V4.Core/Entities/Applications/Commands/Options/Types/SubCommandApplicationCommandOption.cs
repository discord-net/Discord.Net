using Discord.Models;

namespace Discord;

public sealed class SubCommandApplicationCommandOption(
    IDiscordClient client,
    ISubCommandApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<ISubCommandApplicationCommandOptionModel>
{
    public IReadOnlyCollection<ApplicationCommandOption> Options { get; } =
        (IReadOnlyCollection<ApplicationCommandOption>?)model.Options
             ?.Select(x => Construct(client, x))
             .ToList()
             .AsReadOnly()
         ?? Array.Empty<ApplicationCommandOption>();
    
    protected override ISubCommandApplicationCommandOptionModel Model => model;

    public override ISubCommandApplicationCommandOptionModel GetModel()
        => Model;
}