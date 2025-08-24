using Discord.Rest.Api;

namespace Discord.Rest;

public class RestChannelActor : 
    RestActor<Snowflake, RestChannel>,
    IChannelActor
{
    internal RestChannelActor(DiscordRestClient client, Snowflake id) : base(client, id)
    {
    }

    public ValueTask<RestChannel> GetAsync(RequestOptions options = default)
        => new Routes.GetChannel(ChannelId: Id)
            .AsPipeline()
            .Map(RestChannel.Create)
            .RunAsync(Client, options);

    async ValueTask<IChannel> ILoadable<IChannel>.GetAsync(RequestOptions options) => await GetAsync(options);
}