using Discord.Models;

namespace Discord.Rest.Reactions;

public partial class RestReactionActor :
    RestActor<RestReactionActor, DiscordEmojiId, RestReaction, IReactionModel>,
    IReactionActor
{
    [SourceOfTruth]
    public IRestMessageChannelTrait Channel { get; }

    [SourceOfTruth]
    public RestMessageActor Message { get; }
    
    internal override ReactionIdentity Identity { get; }

    
    public RestReactionActor(
        DiscordRestClient client,
        MessageChannelIdentity channel,
        MessageIdentity message,
        ReactionIdentity reaction
    ) : base(client, reaction)
    {
        Identity = reaction;

        Channel = channel.Actor ?? IRestMessageChannelTrait.GetContainerized(
            client.Channels[channel.Id]
        );

        Message = Channel.Messages[message];
    }

    
    [SourceOfTruth]
    internal override RestReaction CreateEntity(IReactionModel model)
    {
        throw new NotImplementedException();
    }

    public IReactionActor.UsersLink Users => throw new NotImplementedException();
}

public partial class RestReaction :
    RestEntity<DiscordEmojiId>,
    IReaction,
    IRestConstructable<RestReaction, RestReactionActor, IReactionModel>
{
}