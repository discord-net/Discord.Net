using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

public sealed class RestDMChannelActor : 
    RestChannelActor,
    IDMChannelActor,
    IRestMessageChannelTrait
{
    [field: MaybeNull]
    public RestMessagesLink<RestMessageActor> Messages
        => field ??= new(this, (client, id) => new RestMessageActor(client, this, id));

    internal RestDMChannelActor(DiscordRestClient client, Snowflake id) : base(client, id)
    {
        
    }

    public Task DeleteAsync(RequestOptions options = default)
    {
        throw new NotImplementedException();
    }
    
    RestMessagesLink IRestMessageChannelTrait.Messages => Messages;
}