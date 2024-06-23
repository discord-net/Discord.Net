using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

public partial class RestLoadableChannelActor(DiscordRestClient client, ulong id) :
    RestChannelActor(client, id),
    ILoadableChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IChannel>))]
    internal RestLoadable<ulong, RestChannel, IChannel, Channel> Loadable { get; } =
        RestLoadable<ulong, RestChannel, IChannel, Channel>.FromConstructable<RestChannel>(
            client,
            id,
            Routes.GetChannel
        );
}

[ExtendInterfaceDefaults(typeof(IChannelActor))]
public partial class RestChannelActor(DiscordRestClient client, ulong id) :
    RestActor<ulong, RestChannel>(client, id),
    IChannelActor;

public partial class RestChannel(DiscordRestClient client, IChannelModel model, RestChannelActor? actor = null) :
    RestEntity<ulong>(client, model.Id),
    IChannel,
    IConstructable<RestChannel, IChannelModel, DiscordRestClient>,
    IContextConstructable<RestChannel, IChannelModel, ulong, DiscordRestClient>
{
    internal IChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IChannelActor))]
    internal virtual RestChannelActor Actor { get; } = actor ?? new(client, model.Id);

    public static RestChannel Construct(DiscordRestClient client, IChannelModel model)
    {
        return model switch
        {
            DMChannelModel dmChannelModel => RestDMChannel.Construct(client, dmChannelModel),
            GroupDMChannel groupDMChannel => RestGroupChannel.Construct(client, groupDMChannel),
            GuildChannelBase => throw new InvalidOperationException(
                $"Cannot construct a channel from {model.GetType()}: missing guild id"),
            _ => new RestChannel(client, model)
        };
    }

    public ChannelType Type => (ChannelType)Model.Type;

    public static RestChannel Construct(DiscordRestClient client, IChannelModel model, ulong guildId)
    {
        return model switch
        {
            GuildChannelBase guildChannel => guildChannel switch
            {
                IGuildNewsChannelModel guildAnnouncementChannel => RestNewsChannel.Construct(client,
                    guildAnnouncementChannel, guildId),
                IGuildCategoryChannelModel guildCategoryChannel => RestCategoryChannel.Construct(client, guildCategoryChannel,
                    guildId),
                IGuildDirectoryChannel guildDirectoryChannel => RestGuildChannel.Construct(client, guildDirectoryChannel,
                    guildId),
                IGuildForumChannelModel guildForumChannel => RestForumChannel.Construct(client, guildForumChannel, guildId),
                IGuildMediaChannelModel guildMediaChannel => RestMediaChannel.Construct(client, guildMediaChannel, guildId),
                IGuildStageChannelModel guildStageVoiceChannel => RestStageChannel.Construct(client,
                    guildStageVoiceChannel, guildId),
                IGuildVoiceChannelModel guildVoiceChannel => RestVoiceChannel.Construct(client, guildVoiceChannel, guildId),
                IThreadChannelModel threadChannel => RestThreadChannel.Construct(client, threadChannel, new(guildId)),
                IGuildTextChannelModel guildTextChannel => RestTextChannel.Construct(client, guildTextChannel, guildId),
                _ => throw new ArgumentOutOfRangeException(nameof(guildChannel))
            },
            _ => Construct(client, model)
        };
    }
}
