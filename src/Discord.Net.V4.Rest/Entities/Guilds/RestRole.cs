using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

[ExtendInterfaceDefaults(
    typeof(IRoleActor),
    typeof(IDeletable<ulong, IRoleActor>),
    typeof(IModifiable<ulong, IRoleActor, ModifyRoleProperties, ModifyGuildRoleParams>)
)]
public partial class RestRoleActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestActor<ulong, RestRole>(client, id),
    IRoleActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guildId);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public sealed partial class RestRole(DiscordRestClient client, ulong guildId, IRoleModel model, RestRoleActor? actor = null) :
    RestEntity<ulong>(client, model.Id),
    IRole
{
    [ProxyInterface(typeof(IRoleActor), typeof(IGuildRelationship))]
    internal RestRoleActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    internal IRoleModel Model { get; } = model;

    public Color Color => Model.Color;

    public bool IsHoisted => Model.IsHoisted;

    public bool IsManaged => Model.IsManaged;

    public bool IsMentionable => Model.IsMentionable;

    public string Name => Model.Name;

    public string? IconId => Model.Icon;

    public Emoji? Emoji => Model.UnicodeEmoji is not null ? new Emoji(Model.UnicodeEmoji) : null;

    public PermissionSet Permissions => Model.Permissions;

    public int Position => Model.Position;

    public RoleTags Tags => new(
        Model.BotId,
        Model.IntegrationId,
        Model.IsPremiumSubscriberRole,
        Model.SubscriptionListingId,
        Model.AvailableForPurchase,
        Model.IsGuildConnection
    );

    public RoleFlags Flags => (RoleFlags)Model.Flags;

    public int CompareTo(IRole? other)
        => other is null ? int.MaxValue : other.Position - Position;
}
