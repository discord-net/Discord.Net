using Discord.Gateway.State;
using Discord.Models;
using System.Collections.Immutable;
using static Discord.Template;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
[method: TypeFactory]
public sealed partial class GatewayForumChannelActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    ForumChannelIdentity channel
) :
    GatewayThreadableChannelActor(client, guild, channel),
    IForumChannelActor,
    IGatewayCachedActor<ulong, GatewayForumChannel, ForumChannelIdentity, IGuildForumChannelModel>
{
    [SourceOfTruth] internal override ForumChannelIdentity Identity { get; } = channel;

    [ProxyInterface(typeof(IIntegrationChannelActor))]
    internal GatewayIntegrationChannelActor IntegrationChannelActor { get; } = new(client, guild, channel);

    [SourceOfTruth]
    internal GatewayForumChannel CreateEntity(IGuildForumChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayForumChannel :
    GatewayThreadableChannel,
    IForumChannel,
    ICacheableEntity<GatewayForumChannel, ulong, IGuildForumChannelModel>
{
    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; }

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public SortOrder? DefaultSortOrder => (SortOrder?)Model.DefaultSortOrder;

    public ForumLayout DefaultLayout => (ForumLayout)Model.DefaultForumLayout;

    [ProxyInterface]
    internal override GatewayForumChannelActor Actor { get; }

    internal override IGuildForumChannelModel Model => _model;

    private IGuildForumChannelModel _model;

    public GatewayForumChannel(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildForumChannelModel model,
        GatewayForumChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;

        Actor = actor ?? new(client, guild, ForumChannelIdentity.Of(this));

        AvailableTags = model.AvailableTags
            .Select(x => ForumTag.Construct(client, new(guild.Id), x))
            .ToImmutableList();
    }

    public static GatewayForumChannel Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext context,
        IGuildForumChannelModel model
    ) => new(
        client,
        context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
        model,
        context.TryGetActor<GatewayForumChannelActor>()
    );

    [CovariantOverride]
    public ValueTask UpdateAsync(
        IGuildForumChannelModel model,
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

    public override IGuildForumChannelModel GetModel() => Model;
}
