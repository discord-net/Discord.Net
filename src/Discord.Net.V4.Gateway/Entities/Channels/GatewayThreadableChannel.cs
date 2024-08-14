using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;
using static Discord.Template;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayThreadableChannelActor :
    GatewayGuildChannelActor,
    IThreadableChannelActor,
    IGatewayCachedActor<ulong, GatewayThreadableChannel, ThreadableChannelIdentity, IThreadableChannelModel>
{
    [SourceOfTruth] public PublicArchivedThreadsPager PublicArchivedThreads { get; }

    [SourceOfTruth] public PrivateArchivedThreadsPager PrivateArchivedThreads { get; }

    [SourceOfTruth] public JoinedPrivateArchivedThreadsPager JoinedPrivateArchivedThreads { get; }

    [SourceOfTruth] internal override ThreadableChannelIdentity Identity { get; }

    public GatewayThreadableChannelActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel) : base(client, guild, channel)
    {
        Identity = channel | this;

        // ReSharper disable VirtualMemberCallInConstructor
        PublicArchivedThreads = GatewayActors.PublicArchivedThreads(client, Guild.Identity, CachePath);
        PrivateArchivedThreads = GatewayActors.PrivateArchivedThreads(client, Guild.Identity, CachePath);
        JoinedPrivateArchivedThreads = GatewayActors.JoinedPrivateArchivedThreads(client, Guild.Identity, CachePath);
        // ReSharper restore VirtualMemberCallInConstructor
    }

    [SourceOfTruth]
    internal GatewayThreadableChannel CreateEntity(IThreadableChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

[ExtendInterfaceDefaults]
public partial class GatewayThreadableChannel :
    GatewayGuildChannel,
    IThreadableChannel,
    ICacheableEntity<GatewayThreadableChannel, ulong, IThreadableChannelModel>
{
    [SourceOfTruth] public GatewayCategoryChannelActor? Category { get; private set; }

    public int? DefaultThreadSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ThreadArchiveDuration DefaultArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    [ProxyInterface] internal override GatewayThreadableChannelActor Actor { get; }

    internal override IThreadableChannelModel Model => _model;

    private IThreadableChannelModel _model;

    public GatewayThreadableChannel(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IThreadableChannelModel model,
        GatewayThreadableChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;

        Actor = actor ?? new(client, guild, ThreadableChannelIdentity.Of(this));

        Category = model.ParentId.Map(
            static (id, client, guild) =>
                new GatewayCategoryChannelActor(client, guild, CategoryChannelIdentity.Of(id)),
            client,
            guild
        );
    }

    public static GatewayThreadableChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IThreadableChannelModel model
    ) => new(
        client,
        context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
        model,
        context.TryGetActor<GatewayThreadableChannelActor>()
    );

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(
        IThreadableChannelModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Category = Category.UpdateFrom(
            model.ParentId,
            GatewayCategoryChannelActor.Factory,
            Client,
            Guild.Identity
        );

        _model = model;

        return base.UpdateAsync(model, false, token);
    }

    public override IThreadableChannelModel GetModel() => Model;
}
