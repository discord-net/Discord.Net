using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Extensions;

namespace Discord.Rest;

using MessageChannelTrait = RestMessageChannelTrait<RestVoiceChannelActor, VoiceChannelIdentity>;
using IncomingIntegrationChannelTrait = RestIncomingIntegrationChannelTrait<RestVoiceChannelActor, RestVoiceChannel, VoiceChannelIdentity>;

[ExtendInterfaceDefaults]
public partial class RestVoiceChannelActor :
    RestGuildChannelActor,
    IVoiceChannelActor,
    IRestActor<ulong, RestVoiceChannel, VoiceChannelIdentity, IGuildVoiceChannelModel>
{
    [ProxyInterface(typeof(IMessageChannelTrait))]
    internal MessageChannelTrait MessageChannelTrait { get; }

    [ProxyInterface(typeof(IIncomingIntegrationChannelTrait))]
    internal IncomingIntegrationChannelTrait IncomingIntegrationChannelTrait { get; }

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
        => RestVoiceChannel.Construct(Client, Guild.Identity, model);
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

    public VideoQualityMode VideoQualityMode => (VideoQualityMode?)Model.VideoQualityMode ?? VideoQualityMode.Auto;

    internal override IGuildVoiceChannelModel Model => _model;

    [ProxyInterface(
        typeof(IVoiceChannelActor),
        typeof(IMessageChannelTrait),
        typeof(IEntityProvider<IVoiceChannel, IGuildVoiceChannelModel>)
    )]
    internal override RestVoiceChannelActor Actor { get; }

    private IGuildVoiceChannelModel _model;

    internal RestVoiceChannel(DiscordRestClient client,
        GuildIdentity guild,
        IGuildVoiceChannelModel model,
        RestVoiceChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        Category = model.ParentId.Map(
            static (id, client, guild) => new RestCategoryChannelActor(client, guild, CategoryChannelIdentity.Of(id)),
            client,
            guild
        );

        Actor = actor ?? new(
            client,
            guild,
            VoiceChannelIdentity.Of(this)
        );
    }

    public static RestVoiceChannel Construct(DiscordRestClient client,
        GuildIdentity guild,
        IGuildVoiceChannelModel model)
    {
        switch (model)
        {
            case IGuildStageChannelModel stage:
                return RestStageChannel.Construct(client, guild, stage);
            default:
                return new(client, guild, model);
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
