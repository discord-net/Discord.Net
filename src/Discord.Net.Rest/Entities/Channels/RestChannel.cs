using Discord.Models;

namespace Discord.Rest;

public class RestChannel :
    RestEntity<Snowflake, IChannelModel>,
    IRestEntity<RestChannel, Snowflake, IChannelModel>,
    IChannel
{
    
    public RestChannel(DiscordRestClient client, IChannelModel model) : base(client, model)
    {
    }

    
    
    public static RestChannel Create(DiscordRestClient client, IChannelModel model)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IChannel> GetAsync(RequestOptions options = default)
    {
        throw new NotImplementedException();
    }
}