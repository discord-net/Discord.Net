using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

public sealed partial class RestLoadableTextChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestTextChannelActor(client, guildId, id),
    ILoadableTextChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<ITextChannel>))]
    internal RestLoadable<ulong, RestTextChannel, ITextChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestTextChannel, GuildTextChannel>(
                (_, model) => RestTextChannel.Construct(client, model, guildId)
            )
        );
}

[ExtendInterfaceDefaults(
    typeof(ITextChannelActor),
    typeof(IModifiable<ulong, ITextChannelActor, ModifyTextChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestTextChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildChannelActor(client, guildId, id),
    ITextChannelActor
{
    public IRootActor<ILoadableMessageActor<IMessage>, ulong, IMessage> Messages => throw new NotImplementedException();

    public IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> PublicArchivedThreads => throw new NotImplementedException();

    public IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> PrivateArchivedThreads => throw new NotImplementedException();

    public IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> JoinedPrivateArchivedThreads => throw new NotImplementedException();
}

public partial class RestTextChannel(DiscordRestClient client, ulong guildId, IGuildTextChannelModel model, RestTextChannelActor? actor = null) :
    RestGuildChannel(client, guildId, model, actor),
    ITextChannel,
    IContextConstructable<RestTextChannel, IGuildTextChannelModel, ulong, DiscordRestClient>
{
    internal override IGuildTextChannelModel Model { get; } = model;

    [ProxyInterface(
        typeof(ITextChannelActor),
        typeof(IMessageChannelActor),
        typeof(IThreadableGuildChannelActor),
        typeof(IGuildChannelActor)
    )]
    internal override RestTextChannelActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    public ILoadableEntity<ulong, ICategoryChannel>? Category => throw new NotImplementedException();

    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public int SlowModeInterval => Model.RatelimitPerUser;

    public ThreadArchiveDuration DefaultArchiveDuration => (ThreadArchiveDuration)Model.DefaultArchiveDuration;

    public static RestTextChannel Construct(DiscordRestClient client, IGuildTextChannelModel model, ulong context)
        => new(client, context, model);
}
