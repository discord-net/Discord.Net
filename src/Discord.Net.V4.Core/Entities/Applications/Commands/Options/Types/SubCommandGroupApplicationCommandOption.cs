using Discord.Models;

namespace Discord;

public sealed class SubCommandGroupApplicationCommandOption(
    IDiscordClient client,
    ISubCommandGroupApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<ISubCommandGroupApplicationCommandOptionModel>
{
    public IReadOnlyCollection<ApplicationCommandOption> Options { get; } =
        (IReadOnlyCollection<ApplicationCommandOption>?)model.Options
            ?.Select(x => Construct(client, x))
            .ToList()
            .AsReadOnly()
        ?? Array.Empty<ApplicationCommandOption>();
    
    protected override ISubCommandGroupApplicationCommandOptionModel Model => model;

    public override ISubCommandGroupApplicationCommandOptionModel GetModel()
        => Model;
}