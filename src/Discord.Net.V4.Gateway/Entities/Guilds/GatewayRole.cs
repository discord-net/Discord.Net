using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayRoleActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    RoleIdentity role
) :
    GatewayCachedActor<ulong, GatewayRole, RoleIdentity, IRoleModel>(client, role),
    IRoleActor
{
    [SourceOfTruth, StoreRoot] public GatewayGuildActor Guild { get; } = guild.Actor ?? new(client, guild);

    [SourceOfTruth]
    internal GatewayRole CreateEntity(IRoleModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayRole :
    GatewayCacheableEntity<GatewayRole, ulong, IRoleModel>,
    IRole
{
    public Color Color => Model.Color;

    public bool IsHoisted => Model.IsHoisted;

    public bool IsManaged => Model.IsManaged;

    public bool IsMentionable => Model.IsMentionable;

    public string Name => Model.Name;

    public string? IconId => Model.Icon;

    public Emoji? Emoji { get; private set; }

    public PermissionSet Permissions { get; private set; }

    public int Position => Model.Position;

    public RoleTags? Tags { get; private set; }

    public RoleFlags Flags => (RoleFlags)Model.Flags;

    [ProxyInterface] internal GatewayRoleActor Actor { get; }

    internal IRoleModel Model { get; private set; }

    internal GatewayRole(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IRoleModel model,
        GatewayRoleActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, RoleIdentity.Of(this));

        Emoji = model.UnicodeEmoji is not null
            ? new Emoji(model.UnicodeEmoji)
            : null;

        Tags = model.Tags is not null
            ? RoleTags.Construct(client, model.Tags)
            : null;

        Permissions = model.Permissions;
    }

    public static GatewayRole Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext context,
        IRoleModel model
    ) => new(
        client,
        context.Path.RequireIdentity(T<GuildIdentity>()),
        model,
        context.TryGetActor<GatewayRoleActor>()
    );

    public override ValueTask UpdateAsync(
        IRoleModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        if(!Model.Permissions.Equals(model.Permissions))
            Permissions = model.Permissions;

        if(Model.UnicodeEmoji != model.UnicodeEmoji)
            Emoji = model.UnicodeEmoji is not null
                ? new Emoji(model.UnicodeEmoji)
                : null;

        if(!Model.Tags?.Equals(model.Tags) ?? model.Tags is not null)
            Tags = model.Tags is not null
                ? RoleTags.Construct(Client, model.Tags)
                : null;

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IRoleModel GetModel() => Model;
}
