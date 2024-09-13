using Discord.Models;

namespace Discord;

public sealed class BooleanApplicationCommandOption(
    IDiscordClient client,
    IBooleanApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<IBooleanApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;
    
    protected override IBooleanApplicationCommandOptionModel Model => model;

    public override IBooleanApplicationCommandOptionModel GetModel()
        => Model;
}