using Discord.Models;

namespace Discord.Rest;

public partial class RestReactionActor :
    RestActor<RestReactionActor, DiscordEmojiId, RestReaction, IReactionModel>,
    IReactionActor
{
    [SourceOfTruth]
    public RestUserActor.Paged<PageUserReactionsParams>.Indexable.WithCurrent.BackLink<RestReactionActor> Users { get; }

    [SourceOfTruth] public IRestMessageChannelTrait Channel { get; }

    [SourceOfTruth] public RestMessageActor Message { get; }

    internal override ReactionIdentity Identity { get; }

    [TypeFactory]
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

        Users = new(
            this,
            Client,
            Client.Users,
            new RestPagingProvider<IUserModel, PageUserReactionsParams, RestUser>(
                client,
                (model, _) => client.Users[model.Id].CreateEntity(model),
                this
            ),
            new(this, client, client.Users.Current.Identity)
        );
    }


    [SourceOfTruth]
    internal override RestReaction CreateEntity(IReactionModel model)
        => RestReaction.Construct(Client, this, model);
}

public partial class RestReaction :
    RestEntity<DiscordEmojiId>,
    IReaction,
    IRestConstructable<RestReaction, RestReactionActor, IReactionModel>
{
    public int NormalCount => Model.NormalCount;

    public int SuperReactionsCount => Model.BurstCount;

    public bool HasReacted => Model.Me;

    public bool HasSuperReacted => Model.MeBurst;

    public IReadOnlyCollection<Color> SuperReactionColors { get; }
    
    [ProxyInterface(typeof(IReactionActor))]
    internal RestReactionActor Actor { get; }

    internal IReactionModel Model { get; }

    public RestReaction(
        DiscordRestClient client,
        IReactionModel model,
        RestReactionActor actor
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor;

        SuperReactionColors = model.BurstColors.Select(x => Color.Parse(x)).ToList().AsReadOnly();
    }

    public static RestReaction Construct(DiscordRestClient client, RestReactionActor actor, IReactionModel model)
        => new(client, model, actor);

    public IReactionModel GetModel() => Model;
}