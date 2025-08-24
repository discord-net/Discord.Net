using System.Diagnostics.CodeAnalysis;
using Discord.Rest.Api;

namespace Discord.Rest;

public interface IRestMessageChannelTrait :
    IRestActor<Snowflake, RestMessage>,
    IMessageChannelTrait
{
    new RestMessagesLink Messages { get; }

    IMessagesLink IMessageChannelTrait.Messages => Messages;

    internal sealed record Sentinel(
        DiscordRestClient Client,
        Snowflake Id
    ) : IRestMessageChannelTrait
    {
        [field: MaybeNull]
        public RestMessagesLink Messages
            => field ??= new RestMessagesLink<RestMessageActor>(
                this,
                (client, id) => new RestMessageActor(client, this, id)
            );
        
        public ValueTask<RestChannel> GetAsync(RequestOptions options = default)
            => new Routes.GetChannel(Id)
                .AsPipeline()
                .Map(RestChannel.Create)
                .RunAsync(Client, options);
        

        async ValueTask<IChannel> ILoadable<IChannel>.GetAsync(RequestOptions options)
            => await GetAsync(options);
    }
}