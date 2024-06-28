using Discord.Models;

namespace Discord.Rest.Channels;

public class RestCategoryChannel :
    RestGuildChannel,
    ICategoryChannel,
    IContextConstructable<RestCategoryChannel, IGuildCategoryChannelModel, GuildIdentity, DiscordRestClient>
{
    internal RestCategoryChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildCategoryChannelModel model
    ) : base(client, guild, model)
    { }

    public static RestCategoryChannel Construct(
        DiscordRestClient client,
        IGuildCategoryChannelModel model,
        GuildIdentity guild
    ) => new(client, guild, model);
}
