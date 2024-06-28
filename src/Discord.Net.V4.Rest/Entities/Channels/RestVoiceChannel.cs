using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

public partial class RestLoadableVoiceChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestTextChannelActor(client, guildId, id),
    ILoadableVoiceChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IVoiceChannel>))]
    internal RestLoadable<ulong, RestVoiceChannel, IVoiceChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestVoiceChannel, GuildVoiceChannel>(
                (_, model) => RestVoiceChannel.Construct(client, model, guildId)
            )
        );
}

[ExtendInterfaceDefaults(
    typeof(IModifiable<ulong, IVoiceChannelActor, ModifyVoiceChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestVoiceChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestTextChannelActor(client, guildId, id),
    IVoiceChannelActor;

public partial class RestVoiceChannel(
    DiscordRestClient client,
    ulong guildId,
    IGuildVoiceChannelModel model,
    RestVoiceChannelActor? actor = null
):
    RestTextChannel(client, guildId, model),
    IVoiceChannel,
    IContextConstructable<RestVoiceChannel, IGuildVoiceChannelModel, ulong, DiscordRestClient>
{
    internal new IGuildVoiceChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IVoiceChannelActor))]
    internal override RestVoiceChannelActor ChannelActor { get; } = actor ?? new(client, guildId, model.Id);

    public static RestVoiceChannel Construct(DiscordRestClient client, IGuildVoiceChannelModel model, ulong context)
        => new(client, context, model);

    public string? RTCRegion => Model.RTCRegion;

    public int Bitrate => Model.Bitrate;

    public int? UserLimit => Model.UserLimit;

    public VideoQualityMode VideoQualityMode => (VideoQualityMode?)Model.VideoQualityMode ?? VideoQualityMode.Auto;


}
