using Discord.Models;

namespace Discord.Rest;

public class RestMessage :
    RestEntity<Snowflake, IMessageModel>,
    IRestEntity<RestMessage, Snowflake, IMessageModel>,
    IMessage
{
    public IReactionsLink Reactions => Actor.Reactions;
    public IRestMessageChannelTrait Channel => Actor.Channel;
    public RestUserActor Author => Client.Users[Model.Author.Id];
    
    public IReadOnlyList<IUserActor> MentionedUsers
        => [..Model.Mentions.Select(x => Client.Users[x])];

    public IReadOnlyList<IChannelActor> MentionedChannels => throw new NotImplementedException();

    public IReadOnlyList<IAttachment> Attachments => throw new NotImplementedException();

    
    protected virtual RestMessageActor Actor { get; }
    
    
    protected RestMessage(
        DiscordRestClient client,
        IMessageModel model,
        RestMessageActor? actor = null
    ) : base(client, model)
    {
        Actor = actor ?? new IRestMessageChannelTrait.Sentinel(client, model.ChannelId).Messages[model.Id];
        
    }

    public static RestMessage Create(DiscordRestClient client, IMessageModel model)
    {
        throw new NotImplementedException();
    }


    public Task<IMessage> ModifyAsync(IModifyMessageParams properties, RequestOptions options = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(RequestOptions options = default)
    {
        throw new NotImplementedException();
    }

    IMessageChannelTrait IMessageActor.Channel => Channel;
    IUserActor IMessage.Author => Author;
    ValueTask<IMessage> ILoadable<IMessage>.GetAsync(RequestOptions options) => ValueTask.FromResult<IMessage>(this);
}