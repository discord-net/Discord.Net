using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestForumChannelActor :
    RestThreadableChannelActor,
    IForumChannelActor,
    IRestActor<RestForumChannelActor, ulong, RestForumChannel, IGuildForumChannelModel>,
    IRestIncomingIntegrationChannelTrait
{
    [SourceOfTruth] internal override ForumChannelIdentity Identity { get; }

    [method: TypeFactory]
    public RestForumChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        ForumChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
    }

    [CovariantOverride]
    [SourceOfTruth]
    internal RestForumChannel CreateEntity(IGuildForumChannelModel model)
        => RestForumChannel.Construct(Client, this, model);
}

public partial class RestForumChannel :
    RestThreadableChannel,
    IForumChannel,
    IRestConstructable<RestForumChannel, RestForumChannelActor, IGuildForumChannelModel>
{
    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration) Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; }

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public DiscordEmojiId? DefaultReactionEmoji => Model.DefaultReactionEmoji;

    public SortOrder? DefaultSortOrder => (SortOrder?) Model.DefaultSortOrder;

    public ForumLayout DefaultLayout => (ForumLayout) Model.DefaultForumLayout;

    [ProxyInterface(
        typeof(IForumChannelActor),
        typeof(IEntityProvider<IForumChannel, IGuildForumChannelModel>)
    )]
    internal override RestForumChannelActor Actor { get; }

    internal override IGuildForumChannelModel Model => _model;

    private IGuildForumChannelModel _model;

    internal RestForumChannel(
        DiscordRestClient client,
        IGuildForumChannelModel model,
        RestForumChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;

        AvailableTags = model.AvailableTags
            .Select(x => ForumTag.Construct(client, x))
            .ToList()
            .AsReadOnly();
    }

    public static RestForumChannel Construct(
        DiscordRestClient client,
        RestForumChannelActor actor,
        IGuildForumChannelModel model
    ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildForumChannelModel model, CancellationToken token = default)
    {
        if (!_model.AvailableTags.SequenceEqual(model.AvailableTags))
        {
            AvailableTags = Model.AvailableTags
                .Select(x => ForumTag.Construct(Client, x))
                .ToList()
                .AsReadOnly();
        }

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildForumChannelModel GetModel() => Model;
}