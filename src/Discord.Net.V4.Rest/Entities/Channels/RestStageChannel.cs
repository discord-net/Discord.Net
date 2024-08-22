using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

using ChannelFollowerIntegrationChannelTrait =
    RestChannelFollowerIntegrationChannelTrait<RestStageChannelActor, RestStageChannel, StageChannelIdentity>;

[ExtendInterfaceDefaults]
public sealed partial class RestStageChannelActor :
    RestVoiceChannelActor,
    IStageChannelActor,
    IRestActor<ulong, RestStageChannel, StageChannelIdentity>
{
    [SourceOfTruth] public RestStageInstanceActor StageInstance { get; }

    [ProxyInterface(typeof(IChannelFollowerIntegrationChannelTrait))]
    internal ChannelFollowerIntegrationChannelTrait ChannelFollowerIntegrationChannelTrait {  get; }

    [SourceOfTruth] internal override StageChannelIdentity Identity { get; }

    [TypeFactory]
    public RestStageChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        StageChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
        StageInstance = new RestStageInstanceActor(client, Guild.Identity, Identity, StageInstanceIdentity.Of(channel.Id));

        ChannelFollowerIntegrationChannelTrait = new(client, this, channel);
    }

    [SourceOfTruth]
    public RestStageChannel CreateEntity(IGuildStageChannelModel model)
        => RestStageChannel.Construct(Client, Guild.Identity, model);

    [SourceOfTruth]
    internal RestStageInstance CreateEntity(IStageInstanceModel model)
        => RestStageInstance.Construct(Client, new(Guild.Identity, Identity), model);
}

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
        GuildIdentity guild,
        IGuildStageChannelModel model,
        RestStageChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        Actor = actor ?? new(client, guild, StageChannelIdentity.Of(this));
    }

    public static RestStageChannel Construct(DiscordRestClient client, GuildIdentity guild,
        IGuildStageChannelModel model)
        => new(client, guild, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildStageChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildStageChannelModel GetModel() => Model;
}
