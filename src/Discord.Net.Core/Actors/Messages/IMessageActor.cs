using Discord.Models;

namespace Discord;

public interface IMessageActor :
    IActor<Snowflake, IMessage>,
    ILoadable<IMessage>,
    IModifiable<IModifyMessageParams, IMessage>,
    IDeletable
{
    IReactionsLink Reactions { get; }
    IMessageChannelTrait Channel { get; }
}

public static class MessageActorExtensions
{
    public static Task PinAsync(this IMessageActor actor, RequestOptions options = default)
        => actor.Channel.Messages.Pins.AddAsync(actor.Id);
    
    public static Task UnpinAsync(this IMessageActor actor, RequestOptions options = default)
        => actor.Channel.Messages.Pins[actor.Id].DeleteAsync();
}