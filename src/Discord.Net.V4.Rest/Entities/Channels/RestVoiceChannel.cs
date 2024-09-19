using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Extensions;

namespace Discord.Rest;

using MessageChannelTrait = RestMessageChannelTrait<RestVoiceChannelActor, VoiceChannelIdentity>;
using IncomingIntegrationChannelTrait =
    RestIncomingIntegrationChannelTrait<RestVoiceChannelActor, RestVoiceChannel, VoiceChannelIdentity>;

[ExtendInterfaceDefaults]
public partial class RestVoiceChannelActor :
    RestGuildChannelActor,
    IVoiceChannelActor,
    IRestActor<ulong, RestVoiceChannel, VoiceChannelIdentity, IGuildVoiceChannelModel>
{
    [SourceOfTruth]
    public RestGuildChannelInviteActor.Enumerable.Indexable.BackLink<RestGuildChannelActor> Invites { get; }
    
    [ProxyInterface(typeof(IMessageChannelTrait))]
    internal MessageChannelTrait MessageChannelTrait { get; }

    [ProxyInterface(typeof(IIncomingIntegrationChannelTrait))]
    internal IncomingIntegrationChannelTrait IncomingIntegrationChannelTrait { get; }
    
    internal RestInvitableTrait<RestGuildChannelInviteActor, RestGuildChannelInvite> InvitableTrait { get; }

    [SourceOfTruth] internal override VoiceChannelIdentity Identity { get; }

    [method: TypeFactory]
    public RestVoiceChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        VoiceChannelIdentity channel
    ) : base(client, guild, channel)
    {
        channel = Identity = channel | this;

        MessageChannelTrait = new(client, this, channel);
        IncomingIntegrationChannelTrait = new(client, this, channel);
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestVoiceChannel CreateEntity(IGuildVoiceChannelModel model)
        => RestVoiceChannel.Construct(Client, this, model);
}

public partial class RestVoiceChannel :
    RestGuildChannel,
    IVoiceChannel,
    IRestConstructable<RestVoiceChannel, RestVoiceChannelActor, IGuildVoiceChannelModel>
{
    [SourceOfTruth] public RestCategoryChannelActor? Category { get; private set; }

    public string? RTCRegion => Model.RTCRegion;

    public int Bitrate => Model.Bitrate;

    public int? UserLimit => Model.UserLimit;

    public VideoQualityMode VideoQualityMode => (VideoQualityMode?) Model.VideoQualityMode ?? VideoQualityMode.Auto;

    internal override IGuildVoiceChannelModel Model => _model;

    [ProxyInterface(
        typeof(IVoiceChannelActor),
        typeof(IMessageChannelTrait),
        typeof(IEntityProvider<IVoiceChannel, IGuildVoiceChannelModel>)
    )]
    internal override RestVoiceChannelActor Actor { get; }

    private IGuildVoiceChannelModel _model;

    internal RestVoiceChannel(
        DiscordRestClient client,
        IGuildVoiceChannelModel model,
        RestVoiceChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;

        Category = model.ParentId.HasValue
            ? actor.Guild.Channels.Category[model.ParentId.Value]
            : null;
    }

    public static RestVoiceChannel Construct(
        DiscordRestClient client,
        RestVoiceChannelActor actor,
        IGuildVoiceChannelModel model)
    {
        switch (model)
        {
            case IGuildStageChannelModel stage:
                return RestStageChannel.Construct(
                    client,
                    actor as RestStageChannelActor ?? actor.Guild.Channels.Stage[actor.Id],
                    stage
                );
            default:
                return new(client, model, actor);
        }
    }

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IGuildVoiceChannelModel model, CancellationToken token = default)
    {
        Category = Category.UpdateFrom(
            model.ParentId,
            RestCategoryChannelActor.Factory,
            Client,
            Actor.Guild.Identity
        );

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildVoiceChannelModel GetModel() => Model;
}