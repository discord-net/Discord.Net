using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

[ExtendInterfaceDefaults(
    typeof(IRoleActor),
    typeof(IDeletable<ulong, IRoleActor>),
    typeof(IModifiable<ulong, IRoleActor, ModifyRoleProperties, ModifyGuildRoleParams>)
)]
public partial class RestRoleActor(
    DiscordRestClient client,
    GuildIdentity guild,
    RoleIdentity role
):
    RestActor<ulong, RestRole, RoleIdentity>(client, role),
    IRoleActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guild);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;

    IRole IEntityProvider<IRole, IRoleModel>.CreateEntity(IRoleModel model)
        => new RestRole(Client, guild, model);
}

public sealed partial class RestRole :
    RestEntity<ulong>,
    IRole,
    IContextConstructable<RestRole, IRoleModel, GuildIdentity, DiscordRestClient>
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
        GuildIdentity guild,
        IRoleModel model,
        RestRoleActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, guild, RoleIdentity.Of(this));
        Model = model;

        Emoji = model.UnicodeEmoji is not null
            ? new(model.UnicodeEmoji)
            : null;
        Tags = model.Tags is not null
            ? RoleTags.Construct(Client, model.Tags)
            : null;
    }

    public static RestRole Construct(DiscordRestClient client, IRoleModel model, GuildIdentity context)
        => new(client, context, model);

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

    public int CompareTo(IRole? other)
        => other is null ? int.MaxValue : other.Position - Position;
}
