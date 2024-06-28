using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;

namespace Discord.Rest.Channels;

public sealed partial class RestLoadableTextChannelActor(DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IIdentifiableEntityOrModel<ulong, RestTextChannel, IGuildTextChannelModel> channel) :
    RestTextChannelActor(client, guild, channel.Id),
    ILoadableTextChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<ITextChannel>))]
    internal RestLoadable<ulong, RestTextChannel, ITextChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestTextChannel, GuildTextChannel>(
                (_, model) => RestTextChannel.Construct(client, model, guild)
            )
        );
}

[ExtendInterfaceDefaults(
    typeof(ITextChannelActor),
    typeof(IModifiable<ulong, ITextChannelActor, ModifyTextChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestTextChannelActor(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IIdentifiableEntityOrModel<ulong, RestTextChannel, IGuildTextChannelModel> channel) :
    RestGuildChannelActor(client, guild, channel),
    ITextChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, guild, channel);

    [ProxyInterface(typeof(IThreadableGuildChannelActor))]
    internal RestThreadableGuildChannelActor ThreadableGuildChannelActor { get; } = new(client, guild, id);
}

public partial class RestTextChannel(DiscordRestClient client, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild, IGuildTextChannelModel model, RestTextChannelActor? actor = null) :
    RestGuildChannel(client, guild, model, actor),
    ITextChannel,
    IContextConstructable<RestTextChannel, IGuildTextChannelModel, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel>, DiscordRestClient>
{
    internal new IGuildTextChannelModel Model { get; } = model;

    [ProxyInterface(
        typeof(ITextChannelActor),
        typeof(IMessageChannelActor),
        typeof(IThreadableGuildChannelActor)
    )]
    internal override RestTextChannelActor ChannelActor { get; } = actor ?? new(client, guild, model.Id);

    public ILoadableEntity<ulong, ICategoryChannel>? Category => throw new NotImplementedException();

    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public int SlowModeInterval => Model.RatelimitPerUser;

    public ThreadArchiveDuration DefaultArchiveDuration => (ThreadArchiveDuration)Model.DefaultArchiveDuration;

    public static RestTextChannel Construct(DiscordRestClient client, IGuildTextChannelModel model, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild)
        => new(client, guild, model);
}
