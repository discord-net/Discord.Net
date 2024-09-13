using Discord.Models;

namespace Discord;

public class ChannelApplicationCommandOption(
    IDiscordClient client,
    IChannelApplicationCommandOptionModel model
) :
    ApplicationCommandOption(client, model),
    IEntityOf<IChannelApplicationCommandOptionModel>
{
    public bool IsRequired => Model.IsRequired ?? false;

    public IReadOnlyCollection<ChannelType> ChannelTypes { get; } =
        model.ChannelTypes
            ?.Select(x => (ChannelType) x)
            .ToList()
            .AsReadOnly()
        ??
        (IReadOnlyCollection<ChannelType>) Array.Empty<ChannelType>();

    protected override IChannelApplicationCommandOptionModel Model => model;

    public override IChannelApplicationCommandOptionModel GetModel()
        => Model;
}