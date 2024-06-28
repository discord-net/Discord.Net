using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;

namespace Discord.Rest.Channels;

public partial class RestLoadableChannelActor(
    DiscordRestClient client,
    IChannelIdentity channel) :
    RestChannelActor(client, channel),
    ILoadableChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IChannel>))]
    internal RestLoadable<ulong, RestChannel, IChannel, IChannelModel> Loadable { get; } =
        RestLoadable<ulong, RestChannel, IChannel, IChannelModel>.FromConstructable<RestChannel>(
            client,
            channel,
            Routes.GetChannel
        );
}

[ExtendInterfaceDefaults(typeof(IChannelActor))]
public partial class RestChannelActor(
    DiscordRestClient client,
    IChannelIdentity channel) :
    RestActor<ulong, RestChannel>(client, channel.Id),
    IChannelActor;

public partial class RestChannel :
    RestEntity<ulong>,
    IChannel,
    IConstructable<RestChannel, IChannelModel, DiscordRestClient>,
    IContextConstructable<RestChannel, IChannelModel, GuildIdentity, DiscordRestClient>
{
    public ChannelType Type => (ChannelType)Model.Type;

    internal virtual IChannelModel Model => _model;

    [ProxyInterface(typeof(IChannelActor))]
    internal virtual RestChannelActor ChannelActor { get; }

    private IChannelModel _model;

    internal RestChannel(
        DiscordRestClient client,
        IChannelModel model,
        RestChannelActor? actor = null
    ) : base(client, model.Id)
    {
        _model = model;
        ChannelActor = actor ?? new(client, this.Identity<ulong, RestChannel, IChannelModel>());
    }

    public ValueTask UpdateAsync(IChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return ValueTask.CompletedTask;
    }

    public static RestChannel Construct(DiscordRestClient client, IChannelModel model)
    {
        return model switch
        {
            DMChannelModel dmChannelModel => RestDMChannel.Construct(client, dmChannelModel),
            GroupDMChannel groupDMChannel => RestGroupChannel.Construct(client, groupDMChannel),
            GuildChannelBase guildChannelBase => Construct(client, model, guildChannelBase.GuildId),
            _ => new RestChannel(client, model)
        };
    }


    public static RestChannel Construct(DiscordRestClient client, IChannelModel model, GuildIdentity guild)
    {
        return model switch
        {
            GuildChannelBase guildChannel => guildChannel switch
            {
                IGuildNewsChannelModel guildAnnouncementChannel => RestNewsChannel.Construct(client,
                    guildAnnouncementChannel, guild),
                IGuildCategoryChannelModel guildCategoryChannel => RestCategoryChannel.Construct(client, guildCategoryChannel,
                    guild),
                IGuildDirectoryChannel guildDirectoryChannel => RestGuildChannel.Construct(client, guildDirectoryChannel,
                    guild),
                IGuildForumChannelModel guildForumChannel => RestForumChannel.Construct(client, guildForumChannel, guild),
                IGuildMediaChannelModel guildMediaChannel => RestMediaChannel.Construct(client, guildMediaChannel, guild),
                IGuildStageChannelModel guildStageVoiceChannel => RestStageChannel.Construct(client,
                    guildStageVoiceChannel, guild),
                IGuildVoiceChannelModel guildVoiceChannel => RestVoiceChannel.Construct(client, guildVoiceChannel, guild),
                IThreadChannelModel threadChannel => RestThreadChannel.Construct(client, threadChannel, new(guild)),
                IGuildTextChannelModel guildTextChannel => RestTextChannel.Construct(client, guildTextChannel, guild),
                _ => throw new ArgumentOutOfRangeException(nameof(guildChannel))
            },
            _ => Construct(client, model)
        };
    }
}
