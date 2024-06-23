using Discord.Models;
using Discord.Models.Json;
using Discord.Stage;

namespace Discord.Rest.Channels;

public sealed partial class RestLoadableStageChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestStageChannelActor(client, guildId, id),
    ILoadableStageChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IStageChannel>))]
    internal RestLoadable<ulong, RestStageChannel, IStageChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestStageChannel, IGuildStageChannelModel>(
                (_, model) => RestStageChannel.Construct(client, model, guildId)
            )
        );
}

[ExtendInterfaceDefaults(typeof(IStageChannelActor))]
public partial class RestStageChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestVoiceChannelActor(client, guildId, id),
    IStageChannelActor
{
    public ILoadableStageInstanceActor StageInstance => throw new NotImplementedException();
}

public partial class RestStageChannel(DiscordRestClient client, ulong guildId, IGuildVoiceChannelModel model, RestStageChannelActor? actor = null) :
    RestVoiceChannel(client, guildId, model),
    IStageChannel,
    IContextConstructable<RestStageChannel, IGuildStageChannelModel, ulong, DiscordRestClient>
{
    [ProxyInterface(typeof(IStageChannelActor), typeof(IStageInstanceRelationship))]
    internal override RestStageChannelActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    public static RestStageChannel Construct(DiscordRestClient client, IGuildStageChannelModel model, ulong context)
        => new(client, context, model);
}
