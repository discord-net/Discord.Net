namespace Discord;

/// <summary>
///     Represent a permission overwrite object.
/// </summary>
public readonly struct Overwrite : IEntityProperties<Models.Json.Overwrite>, IConstructable<Overwrite, Models.Json.Overwrite>
{
    /// <summary>
    ///     The unique identifier for the object this overwrite is targeting.
    /// </summary>
    public readonly ulong TargetId;

    /// <summary>
    ///     The type of object this overwrite is targeting.
    /// </summary>
    public readonly PermissionTarget TargetType;

    /// <summary>
    ///     The allowed permissions associated with this overwrite entry.
    /// </summary>
    public readonly GuildPermission Allowed;

    /// <summary>
    ///     The denied permissions associated with this overwrite entry.
    /// </summary>
    public readonly GuildPermission Denied;

    /// <summary>
    ///     Initializes a new <see cref="Overwrite" /> with provided target information and modified permissions.
    /// </summary>
    public Overwrite(ulong targetId, PermissionTarget targetType, GuildPermission allowed, GuildPermission denied)
    {
        TargetId = targetId;
        TargetType = targetType;
        Allowed = allowed;
        Denied = denied;
    }

    public Models.Json.Overwrite ToApiModel()
        => new()
        {
            TargetId = TargetId, Allow = (ulong)Allowed, Deny = (ulong)Denied, Type = (int)TargetType
        };

    public static Overwrite Construct(IDiscordClient client, Models.Json.Overwrite model)
        => new(model.TargetId, (PermissionTarget)model.Type, (GuildPermission)model.Allow,
            (GuildPermission)model.Deny);
}
