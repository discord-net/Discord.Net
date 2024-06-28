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
    GuildIdentity guild,
    IdentifiableEntityOrModel<ulong, RestForumChannel, IGuildForumChannelModel> channel) :
    RestGuildChannelActor(client, guild, channel),
    IForumChannelActor
{
    [ProxyInterface(typeof(IThreadableGuildChannelActor))]
    internal RestThreadableGuildChannelActor ThreadableGuildChannelActor { get; } = new(client, guild, channel);

    public IForumChannel CreateEntity(IGuildForumChannelModel model)
        => RestForumChannel.Construct(Client, model, Guild.Identity);
}

public partial class RestForumChannel :
    RestGuildChannel,
    IForumChannel,
    IContextConstructable<RestForumChannel, IGuildForumChannelModel, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel>, DiscordRestClient>
{
    internal override IGuildForumChannelModel Model => _model;

    [ProxyInterface(
        typeof(IForumChannelActor),
        typeof(IThreadableGuildChannelActor),
        typeof(IEntityProvider<IForumChannel, IGuildForumChannelModel>)
    )]
    internal override RestForumChannelActor ChannelActor { get; }

    private IGuildForumChannelModel _model;

    internal RestForumChannel(DiscordRestClient client,
        IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
        IGuildForumChannelModel model,
        RestForumChannelActor? actor = null) : base(client, guild, model, actor)
    {
        _model = model;
        ChannelActor = actor ?? new(client, guild, model.Id);

        AvailableTags = model.AvailableTags
            .Select(x => ForumTag.Construct(client, x, new ForumTag.Context(guild.Id)))
            .ToImmutableArray();
    }

    public ValueTask UpdateAsync(IGuildForumChannelModel model, CancellationToken token = default)
    {
        _model = model;

        AvailableTags = Model.AvailableTags
            .Select(x => ForumTag.Construct(Client, x, new ForumTag.Context(Guild.Id)))
            .ToImmutableArray();

        return base.UpdateAsync(model, token);
    }

    public static RestForumChannel Construct(
        DiscordRestClient client,
        IGuildForumChannelModel model,
        GuildIdentity guild
    ) => new(client, guild, model);

    public ILoadableEntity<ulong, ICategoryChannel>? Category => throw new NotImplementedException();

    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; }

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public SortOrder? DefaultSortOrder => (SortOrder?)Model.DefaultSortOrder;

    public ForumLayout DefaultLayout => (ForumLayout)Model.DefaultForumLayout;
}
