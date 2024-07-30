using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

[method: TypeFactory]
[ExtendInterfaceDefaults(typeof(IChannelActor))]
public partial class RestChannelActor(
    DiscordRestClient client,
    ChannelIdentity channel
) :
    RestActor<ulong, RestChannel, ChannelIdentity>(client, channel),
    IChannelActor
{
    [SourceOfTruth]
    internal virtual RestChannel CreateEntity(IChannelModel model)
        => RestChannel.Construct(Client, model);
}

public partial class RestChannel :
    RestEntity<ulong>,
    IChannel,
    IConstructable<RestChannel, IChannelModel, DiscordRestClient>,
    IContextConstructable<RestChannel, IChannelModel, GuildIdentity?, DiscordRestClient>
{
    public ChannelType Type => (ChannelType)Model.Type;

    [ProxyInterface(
        typeof(IChannelActor),
        typeof(IEntityProvider<IChannel, IChannelModel>)
    )]
    internal virtual RestChannelActor Actor { get; }

    internal virtual IChannelModel Model => _model;

    private IChannelModel _model;

    internal RestChannel(
        DiscordRestClient client,
        IChannelModel model,
        RestChannelActor? actor = null
    ) : base(client, model.Id)
    {
        _model = model;

        Actor = actor ?? new(client, ChannelIdentity.Of(this));
    }

    public static RestChannel Construct(DiscordRestClient client, IChannelModel model)
    {
        return model switch
        {
            DMChannelModel dmChannelModel => RestDMChannel.Construct(client, dmChannelModel),
            GroupDMChannelModel groupDMChannel => RestGroupChannel.Construct(client, groupDMChannel),
            GuildChannelModelBase guildChannelBase => Construct(client,
                GuildIdentity.Of(guildChannelBase.GuildId), model),
            _ => new RestChannel(client, model)
        };
    }

    public static RestChannel Construct(DiscordRestClient client,
        GuildIdentity? guild,
        IChannelModel model)
    {
        switch (guild)
        {
            case null when model is IGuildChannelModel guildChannelModel:
                guild = GuildIdentity.Of(guildChannelModel.GuildId);
                break;
            case null:
                return Construct(client, model);
        }


        return model switch
        {
            IGuildChannelModel guildChannel => guildChannel switch
            {
                IGuildNewsChannelModel guildAnnouncementChannel => RestNewsChannel.Construct(client, guild, guildAnnouncementChannel),
                IGuildCategoryChannelModel guildCategoryChannel => RestCategoryChannel.Construct(client,
                    guild, guildCategoryChannel),
                IGuildDirectoryChannel guildDirectoryChannel => RestGuildChannel.Construct(client,
                    guild, guildDirectoryChannel),
                IGuildForumChannelModel guildForumChannel => RestForumChannel.Construct(client,
                    guild, guildForumChannel),
                IGuildMediaChannelModel guildMediaChannel => RestMediaChannel.Construct(client,
                    guild, guildMediaChannel),
                IGuildStageChannelModel guildStageVoiceChannel => RestStageChannel.Construct(client, guild, guildStageVoiceChannel),
                IGuildVoiceChannelModel guildVoiceChannel => RestVoiceChannel.Construct(client,
                    guild, guildVoiceChannel),
                IThreadChannelModel threadChannel => RestThreadChannel.Construct(client, new(guild), threadChannel),
                IGuildTextChannelModel guildTextChannel => RestTextChannel.Construct(client, guild, guildTextChannel),
                _ => throw new ArgumentOutOfRangeException(nameof(guildChannel))
            },
            _ => Construct(client, model)
        };
    }

    public virtual ValueTask UpdateAsync(IChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return ValueTask.CompletedTask;
    }

    public virtual IChannelModel GetModel() => Model;
}
