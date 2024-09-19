using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestRoleActor :
    RestActor<RestRoleActor, ulong, RestRole, IRoleModel>,
    IRoleActor
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    internal override RoleIdentity Identity { get; }

    [TypeFactory]
    public RestRoleActor(
        DiscordRestClient client,
        GuildIdentity guild,
        RoleIdentity role
    ) : base(client, role)
    {
        Identity = role | this;

        Guild = guild.Actor ?? new(client, guild);
    }

    [SourceOfTruth]
    internal override RestRole CreateEntity(IRoleModel model)
        => RestRole.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestRole :
    RestEntity<ulong>,
    IRole,
    IRestConstructable<RestRole, RestRoleActor, IRoleModel>
{
    public Color Color => Model.Color;

    public bool IsHoisted => Model.IsHoisted;

    public bool IsManaged => Model.IsManaged;

    public bool IsMentionable => Model.IsMentionable;

    public string Name => Model.Name;

    public string? IconId => Model.Icon;

    public Emoji? Emoji { get; private set; }

    public PermissionSet Permissions => Model.Permissions;

    public int Position => Model.Position;

    public RoleTags? Tags { get; private set; }

    public RoleFlags Flags => (RoleFlags)Model.Flags;

    [ProxyInterface(
        typeof(IRoleActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IRole, IRoleModel>)
    )]
    internal RestRoleActor Actor { get; }

    internal IRoleModel Model { get; private set; }

    internal RestRole(
        DiscordRestClient client,
        IRoleModel model,
        RestRoleActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        Model = model;

        Emoji = model.UnicodeEmoji is not null
            ? new(model.UnicodeEmoji)
            : null;
        
        Tags = model.Tags is not null
            ? RoleTags.Construct(Client, model.Tags)
            : null;
    }

    public static RestRole Construct(DiscordRestClient client, RestRoleActor actor, IRoleModel model)
        => new(client, model, actor);

    public ValueTask UpdateAsync(IRoleModel model, CancellationToken token = default)
    {
        if (!Model.UnicodeEmoji?.Equals(model.UnicodeEmoji) ?? model.UnicodeEmoji is not null)
        {
            Emoji = model.UnicodeEmoji is not null
                ? new(model.UnicodeEmoji)
                : null;
        }

        if (!Model.Tags?.Equals(model.Tags) ?? model.Tags is not null)
        {
            Tags = model.Tags is not null
                ? RoleTags.Construct(Client, model.Tags)
                : null;
        }

        Model = model;
        return ValueTask.CompletedTask;
    }

    public IRoleModel GetModel() => Model;
}
