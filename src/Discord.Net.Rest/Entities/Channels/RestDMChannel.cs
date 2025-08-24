using Discord.Models;

namespace Discord.Rest;

public sealed class RestDMChannel : 
    RestChannel,
    IRestEntity<RestDMChannel, Snowflake, IDMChannelModel>,
    IDMChannel
{
    public IMessagesLink Messages => throw new NotImplementedException();
    
    public override IDMChannelModel Model => _model;

    private IDMChannelModel _model;

    private RestDMChannel(DiscordRestClient client, IChannelModel model) : base(client, model)
    {
    }


    public static RestDMChannel Create(DiscordRestClient client, IDMChannelModel model)
        => new(client, model);

    public Task DeleteAsync(RequestOptions options = default)
    {
        throw new NotImplementedException();
    }
}