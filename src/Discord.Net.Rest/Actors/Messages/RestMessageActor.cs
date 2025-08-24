using Discord.Models;
using Discord.Rest.Api;

namespace Discord.Rest;

public class RestMessageActor :
    RestActor<Snowflake, RestMessage>,
    IMessageActor
{
    public IReactionsLink Reactions => throw new NotImplementedException();

    public IRestMessageChannelTrait Channel { get; }

    internal RestMessageActor(
        DiscordRestClient client,
        IRestMessageChannelTrait channel,
        Snowflake id
    ) : base(client, id)
    {
        Channel = channel;
    }

    public ValueTask<RestMessage> GetAsync(RequestOptions options = default)
        => new Routes.GetMessage(Channel.Id, Id)
            .AsPipeline()
            .Map(RestMessage.Create)
            .RunAsync(Client, options);

    public async Task<RestMessage> ModifyAsync(IModifyMessageParams properties, RequestOptions options = default)
        => await new Routes.UpdateMessage(Channel.Id, Id)
            .AsPipeline(properties)
            .Map(RestMessage.Create)
            .RunAsync(Client, options);

    public async Task DeleteAsync(RequestOptions options = default)
        => await new Routes.DeleteMessage(Channel.Id, Id)
            .AsPipeline()
            .RunAsync(Client, options);

    async ValueTask<IMessage> ILoadable<IMessage>.GetAsync(RequestOptions options) => await GetAsync(options);

    async Task<IMessage> IModifiable<IModifyMessageParams, IMessage>.ModifyAsync(
        IModifyMessageParams properties,
        RequestOptions options
    ) => await ModifyAsync(properties, options);
    
    IMessageChannelTrait IMessageActor.Channel => Channel;
}