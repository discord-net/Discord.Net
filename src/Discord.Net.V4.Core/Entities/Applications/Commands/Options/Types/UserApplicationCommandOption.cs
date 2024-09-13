using Discord.Models;

namespace Discord;

public class UserApplicationCommandOption(
    IDiscordClient client,
    IUserApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<IUserApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;
    
    protected override IUserApplicationCommandOptionModel Model => model;

    public override IUserApplicationCommandOptionModel GetModel()
        => Model;
}