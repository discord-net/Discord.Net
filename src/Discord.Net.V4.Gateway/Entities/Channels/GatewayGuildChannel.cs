using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using static Discord.Template;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayGuildChannelActor :
    GatewayChannelActor,
    IGuildChannelActor,
    IGatewayCachedActor<ulong, GatewayGuildChannel, GuildChannelIdentity, IGuildChannelModel>
{
    [SourceOfTruth] internal override GuildChannelIdentity Identity { get; }

    [SourceOfTruth] [StoreRoot] public GatewayGuildActor Guild { get; }

    [SourceOfTruth]
    public GatewayGuildChannelInvites Invites { get; }

    [TypeFactory]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public GatewayGuildChannelActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel
    ) : base(client, channel)
    {
        Identity = channel | this;
        Guild = client.Guilds >> guild;

        Invites = GatewayActors.ChannelInvites(client, guild, channel, CachePath);
    }

    [SourceOfTruth]
    internal GatewayGuildChannelInvite CreateEntity(IInviteModel model)
        => Client.StateController.CreateLatent(Invites[model.Id], model, CachePath);

    [SourceOfTruth]
    internal GatewayGuildChannel CreateEntity(IGuildChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

[ExtendInterfaceDefaults]
public partial class GatewayGuildChannel :
    GatewayChannel,
    IGuildChannel,
    ICacheableEntity<GatewayGuildChannel, ulong, IGuildChannelModel>
{
    public string Name => Model.Name;

    public int Position => Model.Position;

    public ChannelFlags Flags => (ChannelFlags?)Model.Flags ?? ChannelFlags.None;

    public IReadOnlyCollection<Overwrite> PermissionOverwrites { get; private set; }

    [ProxyInterface] internal override GatewayGuildChannelActor Actor { get; }
    internal override IGuildChannelModel Model => _model;

    private IGuildChannelModel _model;

    public GatewayGuildChannel(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildChannelModel model,
        GatewayGuildChannelActor? actor = null
    ) : base(client, model, actor)
    {
        _model = model;

        Actor = actor ?? new(client, guild, GuildChannelIdentity.Of(this));

        PermissionOverwrites = model.Permissions
            .Select(x => Overwrite.Construct(client, x))
            .ToImmutableList();
    }

    public static GatewayGuildChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGuildChannelModel model)
    {
        switch (model)
        {
            case IGuildCategoryChannelModel guildCategoryChannelModel:
                return GatewayCategoryChannel.Construct(client, context, guildCategoryChannelModel);
            case IGuildForumChannelModel guildForumChannelModel:
                return GatewayForumChannel.Construct(client, context, guildForumChannelModel);
            case IGuildMediaChannelModel guildMediaChannelModel:
                return GatewayMediaChannel.Construct(client, context, guildMediaChannelModel);
            case IGuildNewsChannelModel guildNewsChannelModel:
                return GatewayNewsChannel.Construct(client, context, guildNewsChannelModel);
            case IGuildStageChannelModel guildStageChannelModel:
                return GatewayStageChannel.Construct(client, context, guildStageChannelModel);
            case IGuildTextChannelModel guildTextChannelModel:
                return GatewayTextChannel.Construct(client, context, guildTextChannelModel);
            case IGuildVoiceChannelModel guildVoiceChannelModel:
                return GatewayVoiceChannel.Construct(client, context, guildVoiceChannelModel);
            case IThreadableChannelModel threadableChannelModel:
                return GatewayThreadableChannel.Construct(client, context, threadableChannelModel);
            case IThreadChannelModel threadChannelModel:
                return GatewayThreadChannel.Construct(client, context, threadChannelModel);
            default:
                return new GatewayGuildChannel(
                    client,
                    context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
                    model,
                    context.TryGetActor<GatewayGuildChannelActor>()
                );
        }
    }

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IGuildChannelModel model, bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        if (!Model.Permissions.SequenceEqual(model.Permissions))
            PermissionOverwrites = model.Permissions
                .Select(x => Overwrite.Construct(Client, x))
                .ToImmutableList();

        _model = model;

        return base.UpdateAsync(model, false, token);
    }

    public override IGuildChannelModel GetModel() => Model;
}