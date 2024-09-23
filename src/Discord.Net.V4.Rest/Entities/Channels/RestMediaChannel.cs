using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestMediaChannelActor :
    RestThreadableChannelActor,
    IMediaChannelActor,
    IRestActor<RestMediaChannelActor, ulong, RestMediaChannel, IGuildMediaChannelModel>,
    IRestIntegrationChannelTrait.WithIncoming
{
    [SourceOfTruth] internal override MediaChannelIdentity Identity { get; }

    [TypeFactory]
    public RestMediaChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        MediaChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
    }

    [CovariantOverride]
    [SourceOfTruth]
    internal RestMediaChannel CreateEntity(IGuildMediaChannelModel model)
        => RestMediaChannel.Construct(Client, this, model);
}

public partial class RestMediaChannel :
    RestThreadableChannel,
    IMediaChannel,
    IRestConstructable<RestMediaChannel, RestMediaChannelActor, IGuildMediaChannelModel>
{
    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration) Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; }

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public DiscordEmojiId? DefaultReactionEmoji => Model.DefaultReactionEmoji;

    public SortOrder? DefaultSortOrder => (SortOrder?) Model.DefaultSortOrder;

    [ProxyInterface(typeof(IMediaChannelActor))]
    internal override RestMediaChannelActor Actor { get; }

    internal override IGuildMediaChannelModel Model => _model;

    private IGuildMediaChannelModel _model;

    internal RestMediaChannel(
        DiscordRestClient client,
        IGuildMediaChannelModel model,
        RestMediaChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;

        Actor = actor;

        AvailableTags = model.AvailableTags
            .Select(x => ForumTag.Construct(client, x))
            .ToImmutableArray();
    }

    public static RestMediaChannel Construct(
        DiscordRestClient client,
        RestMediaChannelActor actor,
        IGuildMediaChannelModel model
    ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildMediaChannelModel model, CancellationToken token = default)
    {
        if (!_model.AvailableTags.SequenceEqual(model.AvailableTags))
        {
            AvailableTags = model.AvailableTags
                .Select(x => ForumTag.Construct(Client, x))
                .ToImmutableArray();
        }

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildMediaChannelModel GetModel() => Model;
}