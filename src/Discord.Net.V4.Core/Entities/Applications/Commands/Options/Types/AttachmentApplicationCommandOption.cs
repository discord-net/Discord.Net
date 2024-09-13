using Discord.Models;

namespace Discord;

public sealed class AttachmentApplicationCommandOption(
    IDiscordClient client,
    IAttachmentApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<IAttachmentApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;
    
    protected override IAttachmentApplicationCommandOptionModel Model => model;

    public override IAttachmentApplicationCommandOptionModel GetModel()
        => Model;
}