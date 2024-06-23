using Discord.Models;
using Discord.Models.Json;
using PropertyChanged;
using System.Collections.Immutable;

namespace Discord.Rest.Channels;

[ExtendInterfaceDefaults(
    typeof(IForumChannelActor),
    typeof(IModifiable<ulong, IForumChannelActor, ModifyForumChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestForumChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildChannelActor(client, guildId, id),
    IForumChannelActor
{
    [ProxyInterface(typeof(IThreadableGuildChannelActor))]
    internal RestThreadableGuildChannelActor ThreadableGuildChannelActor { get; } = new(client, guildId, id);
}

public partial class RestForumChannel(DiscordRestClient client, ulong guildId, IGuildForumChannelModel model, RestForumChannelActor? actor = null) :
    RestGuildChannel(client, guildId, model, actor),
    IForumChannel,
    IContextConstructable<RestForumChannel, IGuildForumChannelModel, ulong, DiscordRestClient>
{
    [OnChangedMethod(nameof(OnModelUpdated))]
    internal new IGuildForumChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IForumChannelActor), typeof(IThreadableGuildChannelActor))]
    internal override RestForumChannelActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    public static RestForumChannel Construct(DiscordRestClient client, IGuildForumChannelModel model, ulong context)
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

    public ForumLayout DefaultLayout => (ForumLayout)Model.DefaultForumLayout;
}
