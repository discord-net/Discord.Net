using Discord.Models;

namespace Discord.Rest.Channels;

public class RestCategoryChannel :
    RestGuildChannel,
    ICategoryChannel,
    IContextConstructable<RestCategoryChannel, IGuildCategoryChannelModel, RestGuildIdentifiable, DiscordRestClient>
{
    internal RestCategoryChannel(
        DiscordRestClient client,
        RestGuildIdentifiable guild,
        IGuildCategoryChannelModel model
    ) : base(client, guild, model)
    { }

    public static RestCategoryChannel Construct(
        DiscordRestClient client,
        IGuildCategoryChannelModel model,
        RestGuildIdentifiable guild
    ) => new(client, guild, model);
}
