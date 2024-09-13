namespace Discord;

public readonly struct ApplicationCommandPermissions(
    ulong targetId,
    ApplicationCommandPermissionTarget targetType,
    bool allow
) :
    IEntityProperties<Models.Json.ApplicationCommandPermissions>
{
    public ulong TargetId { get; init; } = targetId;
    public ApplicationCommandPermissionTarget TargetType { get; init; } = targetType;
    public bool Allow { get; init; } = allow;

    public static ApplicationCommandPermissions User(
        EntityOrId<ulong, IUserActor> target,
        bool allow
    ) => new(target.Id, ApplicationCommandPermissionTarget.User, allow);

    public static ApplicationCommandPermissions Role(
        EntityOrId<ulong, IRoleActor> target,
        bool allow
    ) => new(target.Id, ApplicationCommandPermissionTarget.Role, allow);

    public static ApplicationCommandPermissions Channel(
        EntityOrId<ulong, IChannelActor> target,
        bool allow
    ) => new(target.Id, ApplicationCommandPermissionTarget.Channel, allow);

    public Models.Json.ApplicationCommandPermissions ToApiModel(
        Models.Json.ApplicationCommandPermissions? existing = default)
    {
        return new Models.Json.ApplicationCommandPermissions()
        {
            Permission = Allow,
            Id = TargetId,
            Type = (int) TargetType
        };
    }
}