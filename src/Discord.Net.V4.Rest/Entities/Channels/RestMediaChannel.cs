using Discord.Models;
using Discord.Models.Json;
using PropertyChanged;
using System.Collections.Immutable;

namespace Discord.Rest.Channels;

public partial class RestLoadableMediaChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestMediaChannelActor(client, guildId, id),
    ILoadableMediaChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IMediaChannel>))]
    internal RestLoadable<ulong, RestMediaChannel, IMediaChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestMediaChannel, GuildMediaChannel>(
                (_, model) => RestMediaChannel.Construct(client, model, guildId)
            )

        );
}

[ExtendInterfaceDefaults(
    typeof(IModifiable<ulong, IMediaChannelActor, ModifyMediaChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestMediaChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildChannelActor(client, guildId, id),
    IMediaChannelActor
{
    [ProxyInterface(typeof(IThreadableGuildChannelActor))]
    internal RestThreadableGuildChannelActor ThreadableGuildChannelActor { get; } = new(client, guildId, id);
}

public partial class RestMediaChannel(DiscordRestClient client, ulong guildId, IGuildMediaChannelModel model, RestMediaChannelActor? actor = null) :
    RestGuildChannel(client, guildId, model),
    IMediaChannel,
    IContextConstructable<RestMediaChannel, IGuildMediaChannelModel, ulong, DiscordRestClient>
{
    [OnChangedMethod(nameof(OnModelUpdated))]
    internal new IGuildMediaChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IMediaChannelActor), typeof(IThreadableGuildChannelActor))]
    internal override RestMediaChannelActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    public static RestMediaChannel Construct(DiscordRestClient client, IGuildMediaChannelModel model, ulong context)
        => new(client, context, model);

    private void OnModelUpdated()
    {
        AvailableTags = Model.AvailableTags
            .Select(x => ForumTag.Construct(Client, x, new ForumTag.Context(Guild.Id)))
            .ToImmutableArray();
    }

    public ILoadableEntity<ulong, ICategoryChannel>? Category => throw new NotImplementedException();

    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; } =
        model.AvailableTags
            .Select(x => ForumTag.Construct(client, x, new ForumTag.Context(guildId)))
            .ToImmutableArray();

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public SortOrder? DefaultSortOrder => (SortOrder?)Model.DefaultSortOrder;
}
