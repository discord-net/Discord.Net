using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using System.Collections.Immutable;

namespace Discord.Rest.Channels;

public partial class RestLoadableMediaChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    IdentifiableEntityOrModel<ulong, RestMediaChannel, IGuildMediaChannelModel> channel
):
    RestMediaChannelActor(client, guild, channel),
    ILoadableMediaChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IMediaChannel>))]
    internal RestLoadable<ulong, RestMediaChannel, IMediaChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestMediaChannel, IGuildMediaChannelModel>(
                (_, model) => RestMediaChannel.Construct(client, model, guild)
            ).Invoke
        );
}

[ExtendInterfaceDefaults(
    typeof(IModifiable<ulong, IMediaChannelActor, ModifyMediaChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestMediaChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    IdentifiableEntityOrModel<ulong, RestMediaChannel, IGuildMediaChannelModel> channel
):
    RestThreadableChannelActor(client, guild, channel),
    IMediaChannelActor,
    IActor<ulong, RestMediaChannel>
{
    public IMediaChannel CreateEntity(IGuildMediaChannelModel model)
        => RestMediaChannel.Construct(Client, model, Guild.Identity);
}

public partial class RestMediaChannel :
    RestThreadableChannel,
    IMediaChannel,
    IContextConstructable<RestMediaChannel, IGuildMediaChannelModel, GuildIdentity, DiscordRestClient>
{
    public ILoadableEntity<ulong, ICategoryChannel>? Category => throw new NotImplementedException();

    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; }

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public SortOrder? DefaultSortOrder => (SortOrder?)Model.DefaultSortOrder;

    internal override IGuildMediaChannelModel Model => _model;

    [ProxyInterface(
        typeof(IMediaChannelActor),
        typeof(IThreadableChannelActor),
        typeof(IEntityProvider<IMediaChannel, IGuildMediaChannelModel>)
    )]
    internal override RestMediaChannelActor Actor { get; }

    private IGuildMediaChannelModel _model;

    internal RestMediaChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildMediaChannelModel model,
        RestMediaChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        Actor = actor ?? new(client, guild, this);

        AvailableTags = model.AvailableTags
            .Select(x => ForumTag.Construct(client, x, new ForumTag.Context(guild.Id)))
            .ToImmutableArray();
    }

    public static RestMediaChannel Construct(DiscordRestClient client, IGuildMediaChannelModel model, GuildIdentity guild)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IGuildMediaChannelModel model, CancellationToken token = default)
    {
        if (!_model.AvailableTags.SequenceEqual(model.AvailableTags))
        {
            AvailableTags = model.AvailableTags
                .Select(x => ForumTag.Construct(Client, x, new ForumTag.Context(Guild.Id)))
                .ToImmutableArray();
        }

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildMediaChannelModel GetModel() => Model;
}
