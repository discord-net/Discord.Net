using Discord.Models;

namespace Discord;

public sealed class RoleApplicationCommandOption(
    IDiscordClient client,
    IRoleApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<IRoleApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;
    
    protected override IRoleApplicationCommandOptionModel Model => model;

    public override IRoleApplicationCommandOptionModel GetModel()
        => Model;
}