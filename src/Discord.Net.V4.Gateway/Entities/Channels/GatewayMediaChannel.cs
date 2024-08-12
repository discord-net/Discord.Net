using Discord.Gateway.State;
using Discord.Models;
using System.Collections.Immutable;
using static Discord.Template;

namespace Discord.Gateway;

using IncomingIntegrationChannelTrait = GatewayIntegrationChannelTrait<
    GatewayMediaChannelActor,
    GatewayMediaChannel,
    MediaChannelIdentity
>;

[ExtendInterfaceDefaults]
public sealed partial class GatewayMediaChannelActor :
    GatewayThreadableChannelActor,
    IMediaChannelActor,
    IGatewayCachedActor<ulong, GatewayMediaChannel, MediaChannelIdentity, IGuildMediaChannelModel>
{
    [SourceOfTruth] internal override MediaChannelIdentity Identity { get; }

    [ProxyInterface(typeof(IIncomingIntegrationChannelTrait))]
    internal IncomingIntegrationChannelTrait IntegrationChannelTrait { get; }

    [method: TypeFactory]
    public GatewayMediaChannelActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        MediaChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
        IntegrationChannelTrait = new(client, this, channel);
    }

    [SourceOfTruth]
    internal GatewayMediaChannel CreateEntity(IGuildMediaChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

[ExtendInterfaceDefaults]
public sealed partial class GatewayMediaChannel :
    GatewayThreadableChannel,
    IMediaChannel,
    ICacheableEntity<GatewayMediaChannel, ulong, IGuildMediaChannelModel>
{
    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; }

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public SortOrder? DefaultSortOrder => (SortOrder?)Model.DefaultSortOrder;

    [ProxyInterface] internal override GatewayMediaChannelActor Actor { get; }

    internal override IGuildMediaChannelModel Model => _model;

    private IGuildMediaChannelModel _model;

    public GatewayMediaChannel(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildMediaChannelModel model,
        GatewayMediaChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;
        Actor = actor ?? new(client, guild, MediaChannelIdentity.Of(this));

        AvailableTags = model.AvailableTags
            .Select(x => ForumTag.Construct(client, new(guild.Id), x))
            .ToImmutableList();
    }

    public static GatewayMediaChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGuildMediaChannelModel model
    ) => new(
        client,
        context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
        model,
        context.TryGetActor<GatewayMediaChannelActor>()
    );

    [CovariantOverride]
    public ValueTask UpdateAsync(
        IGuildMediaChannelModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        if (!Model.AvailableTags.SequenceEqual(model.AvailableTags))
            AvailableTags = model.AvailableTags
                .Select(x => ForumTag.Construct(Client, new(Guild.Id), x))
                .ToImmutableList();

        _model = model;

        return base.UpdateAsync(model, false, token);
    }

    public override IGuildMediaChannelModel GetModel() => Model;
}
