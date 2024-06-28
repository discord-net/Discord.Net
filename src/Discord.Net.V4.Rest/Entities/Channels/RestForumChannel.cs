using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using PropertyChanged;
using System.Collections.Immutable;

namespace Discord.Rest.Channels;

[ExtendInterfaceDefaults(
    typeof(IForumChannelActor),
    typeof(IModifiable<ulong, IForumChannelActor, ModifyForumChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestForumChannelActor(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IdentifiableEntityOrModel<ulong, RestForumChannel, IGuildForumChannelModel> channel) :
    RestGuildChannelActor(client, guild, channel),
    IForumChannelActor
{
    [ProxyInterface(typeof(IThreadableGuildChannelActor))]
    internal RestThreadableGuildChannelActor ThreadableGuildChannelActor { get; } = new(client, guild, channel);
}

public partial class RestForumChannel(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IGuildForumChannelModel model,
    RestForumChannelActor? actor = null
):
    RestGuildChannel(client, guild, model, actor),
    IForumChannel,
    IContextConstructable<RestForumChannel, IGuildForumChannelModel, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel>, DiscordRestClient>
{
    [OnChangedMethod(nameof(OnModelUpdated))]
    internal new IGuildForumChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IForumChannelActor), typeof(IThreadableGuildChannelActor))]
    internal override RestForumChannelActor ChannelActor { get; } = actor ?? new(client, guild, model.Id);

    public static RestForumChannel Construct(DiscordRestClient client, IGuildForumChannelModel model,
        IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild)
        => new(client, guild, model);

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
            .Select(x => ForumTag.Construct(client, x, new ForumTag.Context(guild.Id)))
            .ToImmutableArray();

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public SortOrder? DefaultSortOrder => (SortOrder?)Model.DefaultSortOrder;

    public ForumLayout DefaultLayout => (ForumLayout)Model.DefaultForumLayout;
}
