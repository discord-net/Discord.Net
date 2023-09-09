namespace Discord;

/// <summary>
///     Represent a permission overwrite object.
/// </summary>
public readonly struct Overwrite 
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
    ///     Initializes a new <see cref="Overwrite"/> with provided target information and modified permissions.
    /// </summary>
    public Overwrite(ulong targetId, PermissionTarget targetType, GuildPermission allowed, GuildPermission denied)
    {
        TargetId = targetId;
        TargetType = targetType;
        Allowed = allowed;
        Denied = denied;
    }
}
