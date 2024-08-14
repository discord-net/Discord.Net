using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Rest;

using IncomingIntegrationChannelTrait = RestIncomingIntegrationChannelTrait<RestForumChannelActor, RestForumChannel, ForumChannelIdentity>;

[ExtendInterfaceDefaults]
public sealed partial class RestForumChannelActor :
    RestThreadableChannelActor,
    IForumChannelActor,
    IRestActor<ulong, RestForumChannel, ForumChannelIdentity>
{
    [SourceOfTruth]
    internal override ForumChannelIdentity Identity { get; }

    [ProxyInterface(typeof(IIncomingIntegrationChannelTrait))]
    internal IncomingIntegrationChannelTrait IncomingIntegrationChannelTrait { get; }

    [method: TypeFactory]
    public RestForumChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        ForumChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;

        IncomingIntegrationChannelTrait = new(client, this, channel);
    }

    [CovariantOverride]
    [SourceOfTruth]
    internal RestForumChannel CreateEntity(IGuildForumChannelModel model)
        => RestForumChannel.Construct(Client, Guild.Identity, model);
}

public partial class RestForumChannel :
    RestThreadableChannel,
    IForumChannel,
    IContextConstructable<RestForumChannel, IGuildForumChannelModel, GuildIdentity, DiscordRestClient>
{
    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; }

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public SortOrder? DefaultSortOrder => (SortOrder?)Model.DefaultSortOrder;

    public ForumLayout DefaultLayout => (ForumLayout)Model.DefaultForumLayout;

    [ProxyInterface(
        typeof(IForumChannelActor),
        typeof(IEntityProvider<IForumChannel, IGuildForumChannelModel>)
    )]
    internal override RestForumChannelActor Actor { get; }

    internal override IGuildForumChannelModel Model => _model;

    private IGuildForumChannelModel _model;

    internal RestForumChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildForumChannelModel model,
        RestForumChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;
        Actor = actor ?? new(client, guild, ForumChannelIdentity.Of(this));

        AvailableTags = model.AvailableTags
            .Select(x => ForumTag.Construct(client, new ForumTag.Context(guild.Id), x))
            .ToImmutableArray();
    }

    public static RestForumChannel Construct(DiscordRestClient client,
        GuildIdentity guild,
        IGuildForumChannelModel model) => new(client, guild, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildForumChannelModel model, CancellationToken token = default)
    {
        if (!_model.AvailableTags.SequenceEqual(model.AvailableTags))
        {
            AvailableTags = Model.AvailableTags
                .Select(x => ForumTag.Construct(Client, new ForumTag.Context(Actor.Guild.Id), x))
                .ToImmutableArray();
        }

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildForumChannelModel GetModel() => Model;
}