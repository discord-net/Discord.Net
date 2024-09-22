using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestStageChannelActor :
    RestVoiceChannelActor,
    IStageChannelActor,
    IRestActor<RestStageChannelActor, ulong, RestStageChannel, IGuildStageChannelModel>,
    IRestChannelFollowerIntegrationChannelTrait
{
    [SourceOfTruth] public RestStageInstanceActor StageInstance { get; }

    [SourceOfTruth] internal override StageChannelIdentity Identity { get; }

    [TypeFactory]
    public RestStageChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        StageChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
        StageInstance =
            new RestStageInstanceActor(client, Guild.Identity, Identity, StageInstanceIdentity.Of(channel.Id));
    }

    [SourceOfTruth]
    public RestStageChannel CreateEntity(IGuildStageChannelModel model)
        => RestStageChannel.Construct(Client, this, model);

    [SourceOfTruth]
    internal RestStageInstance CreateEntity(IStageInstanceModel model)
        => RestStageInstance.Construct(Client, new(Guild.Identity, Identity), model);
}

[ExtendInterfaceDefaults]
public partial class RestStageChannel :
    RestVoiceChannel,
    IStageChannel,
    IRestConstructable<RestStageChannel, RestStageChannelActor, IGuildStageChannelModel>
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
        IGuildStageChannelModel model,
        RestStageChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;
    }

    public static RestStageChannel Construct(
        DiscordRestClient client,
        RestStageChannelActor actor,
        IGuildStageChannelModel model
    ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildStageChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildStageChannelModel GetModel() => Model;
}