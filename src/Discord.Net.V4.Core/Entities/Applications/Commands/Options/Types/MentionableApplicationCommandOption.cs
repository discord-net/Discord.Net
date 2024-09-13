using Discord.Models;

namespace Discord;

public sealed class MentionableApplicationCommandOption(
    IDiscordClient client,
    IMentionableApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<IMentionableApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;
    
    protected override IMentionableApplicationCommandOptionModel Model => model;

    public override IMentionableApplicationCommandOptionModel GetModel()
        => Model;
}