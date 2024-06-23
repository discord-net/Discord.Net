using Discord.Models;

namespace Discord.Rest.Channels;

public class RestCategoryChannel(DiscordRestClient client, ulong guildId, IGuildCategoryChannelModel model) :
    RestGuildChannel(client, guildId, model),
    ICategoryChannel,
    IContextConstructable<RestCategoryChannel, IGuildCategoryChannelModel, ulong, DiscordRestClient>
{
    public static RestCategoryChannel Construct(DiscordRestClient client, IGuildCategoryChannelModel model, ulong context)
        => new(client, context, model);
}
