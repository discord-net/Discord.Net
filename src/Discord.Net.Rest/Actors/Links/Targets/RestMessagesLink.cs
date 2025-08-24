namespace Discord.Rest;

public abstract class RestMessagesLink :
    IMessagesLink
{
    public abstract IChannelPinsLink Pins { get; }
    public abstract RestMessageActor this[Snowflake id] { get; }

    public abstract IAsyncEnumerator<IMessage> GetAsyncEnumerator(CancellationToken cancellationToken = default);

    IMessageActor IIndexableLink<Snowflake, IMessageActor>.this[Snowflake id] => this[id];
}

public sealed class RestMessagesLink<TMessageActor> : 
    RestMessagesLink,
    IMessagesLink
    where TMessageActor : RestMessageActor
{
    public override IChannelPinsLink Pins => throw new NotImplementedException();
    
    public override TMessageActor this[Snowflake id] => _indexable[id];

    private readonly RestIndexableLink<Snowflake, TMessageActor> _indexable;

    public RestMessagesLink(
        IRestMessageChannelTrait channel,
        Func<DiscordRestClient, Snowflake, TMessageActor> messageActorFactory
    )
    {
        _indexable = new(channel.Client, messageActorFactory);
    }

    IMessageActor IIndexableLink<Snowflake, IMessageActor>.this[Snowflake id] => this[id];

    public override IAsyncEnumerator<IMessage> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

}