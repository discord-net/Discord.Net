using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using Discord.Stage;

namespace Discord.Rest.Channels;

[ExtendInterfaceDefaults(typeof(IStageChannelActor))]
public partial class RestStageChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    StageChannelIdentity channel
):
    RestVoiceChannelActor(client, guild, channel),
    IStageChannelActor,
    IActor<ulong, RestStageChannel>
{
    public IStageInstanceActor StageInstance => throw new NotImplementedException();

    [SourceOfTruth]
    public RestStageChannel CreateEntity(IGuildStageChannelModel model)
        => RestStageChannel.Construct(Client, model, Guild.Identity);
}

public partial class RestStageChannel :
    RestVoiceChannel,
    IStageChannel,
    IContextConstructable<RestStageChannel, IGuildStageChannelModel, GuildIdentity, DiscordRestClient>
{

    [ProxyInterface(
        typeof(IStageChannelActor),
        typeof(IStageInstanceRelationship),
        typeof(IEntityProvider<IStageChannel, IGuildStageChannelModel>)
    )]
    internal override RestStageChannelActor Actor { get; }

    internal override IGuildStageChannelModel Model => _model;

    private IGuildStageChannelModel _model;

    internal RestStageChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildStageChannelModel model,
        RestStageChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        Actor = actor ?? new(client, guild, StageChannelIdentity.Of(this));
    }

    public static RestStageChannel Construct(DiscordRestClient client, IGuildStageChannelModel model, GuildIdentity guild)
        => new(client, guild, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildStageChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildStageChannelModel GetModel() => Model;
}
