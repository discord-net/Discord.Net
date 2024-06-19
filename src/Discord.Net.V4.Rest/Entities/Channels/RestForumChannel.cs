using Discord.Models;
using Discord.Models.Json;
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
    public IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> PublicArchivedThreads => throw new NotImplementedException();

    public IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> PrivateArchivedThreads => throw new NotImplementedException();

    public IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> JoinedPrivateArchivedThreads => throw new NotImplementedException();
}

public partial class RestForumChannel(DiscordRestClient client, ulong guildId, IGuildForumChannelModel model, RestForumChannelActor? actor = null) :
    RestGuildChannel(client, guildId, model, actor),
    IForumChannel
{
    internal override IGuildForumChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IForumChannelActor), typeof(IThreadableGuildChannelActor))]
    internal override RestGuildChannelActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    public ILoadableEntity<ulong, ICategoryChannel>? Category => throw new NotImplementedException();

    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags => Model.AvailableTags
        .Select(x => ForumTag.Construct(Client, x, new ForumTag.Context(Guild.Id))).ToImmutableArray();

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public ForumSortOrder? DefaultSortOrder => (ForumSortOrder?)Model.DefaultSortOrder;

    public ForumLayout DefaultLayout => (ForumLayout)Model.DefaultForumLayout;
}
