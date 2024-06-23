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
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, guildId, id);

    [ProxyInterface(typeof(IThreadableGuildChannelActor))]
    internal RestThreadableGuildChannelActor ThreadableGuildChannelActor { get; } = new(client, guildId, id);
}

public partial class RestTextChannel(DiscordRestClient client, ulong guildId, IGuildTextChannelModel model, RestTextChannelActor? actor = null) :
    RestGuildChannel(client, guildId, model, actor),
    ITextChannel,
    IContextConstructable<RestTextChannel, IGuildTextChannelModel, ulong, DiscordRestClient>
{
    internal new IGuildTextChannelModel Model { get; } = model;

    [ProxyInterface(
        typeof(ITextChannelActor),
        typeof(IMessageChannelActor),
        typeof(IThreadableGuildChannelActor)
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
