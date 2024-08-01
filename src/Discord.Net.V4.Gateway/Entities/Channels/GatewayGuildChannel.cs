using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using System.Collections.Immutable;
using static Discord.Template;

namespace Discord.Gateway;

[method: TypeFactory]
public partial class GatewayGuildChannelActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    GuildChannelIdentity channel
) :
    GatewayChannelActor(client, channel),
    IGuildChannelActor,
    IGatewayCachedActor<ulong, GatewayGuildChannel, GuildChannelIdentity, IGuildChannelModel>
{
    [SourceOfTruth]
    internal override GuildChannelIdentity Identity { get; } = channel;

    [SourceOfTruth]
    [StoreRoot]
    public GatewayGuildActor Guild { get; } = guild.Actor ?? new GatewayGuildActor(client, guild);

    public IEnumerableIndexableActor<IInviteActor, string, IInvite> Invites => throw new NotImplementedException();

    public IInvite CreateEntity(IInviteModel model) => throw new NotImplementedException();

    [SourceOfTruth]
    internal GatewayGuildChannel CreateEntity(IGuildChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

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
        GatewayGuildChannelActor? actor = null,
        IEntityHandle<ulong, GatewayGuildChannel>? implicitHandle = null
    ) : base(client, model, actor, implicitHandle)
    {
        _model = model;

        Actor = actor ?? new(client, guild, GuildChannelIdentity.Of(this));

        PermissionOverwrites = model.Permissions
            .Select(x => Overwrite.Construct(client, x))
            .ToImmutableList();
    }

    public static GatewayGuildChannel Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayGuildChannel> context,
        IGuildChannelModel model)
    {
        switch (model)
        {
            case IGuildCategoryChannelModel guildCategoryChannelModel:
                throw new NotImplementedException();
            case IGuildForumChannelModel guildForumChannelModel:
                throw new NotImplementedException();
            case IGuildMediaChannelModel guildMediaChannelModel:
                throw new NotImplementedException();
            case IGuildNewsChannelModel guildNewsChannelModel:
                throw new NotImplementedException();
            case IGuildStageChannelModel guildStageChannelModel:
                throw new NotImplementedException();
            case IGuildTextChannelModel guildTextChannelModel:
                throw new NotImplementedException();
            case IGuildVoiceChannelModel guildVoiceChannelModel:
                throw new NotImplementedException();
            case IThreadableChannelModel threadableChannelModel:
                throw new NotImplementedException();
            case IThreadChannelModel threadChannelModel:
                throw new NotImplementedException();
            default:
                return new GatewayGuildChannel(
                    client,
                    context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
                    model,
                    context.TryGetActor(T<GatewayGuildChannelActor>()),
                    context.ImplicitHandle
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
