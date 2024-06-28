using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using Discord.Stage;

namespace Discord.Rest.Channels;

using ChannelIdentity = IdentifiableEntityOrModel<ulong, RestStageChannel, IGuildStageChannelModel>;

public sealed partial class RestLoadableStageChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ChannelIdentity channel
):
    RestStageChannelActor(client, guild, channel),
    ILoadableStageChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IStageChannel>))]
    internal RestLoadable<ulong, RestStageChannel, IStageChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestStageChannel, IGuildStageChannelModel>(
                (_, model) => RestStageChannel.Construct(client, model, guild)
            )
        );
}

[ExtendInterfaceDefaults(typeof(IStageChannelActor))]
public partial class RestStageChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ChannelIdentity channel
):
    RestVoiceChannelActor(client, guild, channel),
    IStageChannelActor,
    IActor<ulong, RestStageChannel>
{
    public ILoadableStageInstanceActor StageInstance => throw new NotImplementedException();
}

public partial class RestStageChannel :
    RestVoiceChannel,
    IStageChannel,
    IContextConstructable<RestStageChannel, IGuildStageChannelModel, GuildIdentity, DiscordRestClient>
{
    internal override IGuildStageChannelModel Model => _model;

    [ProxyInterface(typeof(IStageChannelActor), typeof(IStageInstanceRelationship))]
    internal override RestStageChannelActor ChannelActor { get; }

    private IGuildStageChannelModel _model;

    internal RestStageChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildStageChannelModel model,
        RestStageChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        ChannelActor = actor ?? new(client, guild, model.Id);
    }

    public ValueTask UpdateAsync(IGuildStageChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public static RestStageChannel Construct(DiscordRestClient client, IGuildStageChannelModel model, GuildIdentity guild)
        => new(client, guild, model);
}
