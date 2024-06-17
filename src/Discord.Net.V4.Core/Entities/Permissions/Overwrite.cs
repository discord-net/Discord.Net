using Discord.Models;

namespace Discord;

/// <summary>
///     Represent a permission overwrite object.
/// </summary>
public readonly struct Overwrite : IEntityProperties<Models.Json.Overwrite>, IConstructable<Overwrite, IOverwriteModel>
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

    public Models.Json.Overwrite ToApiModel(Models.Json.Overwrite? existing = default)
    {
        existing ??= new();

        existing.TargetId = TargetId;
        existing.Type = (int)TargetType;
        existing.Allow = (ulong)Allowed;
        existing.Deny = (ulong)Denied;

        return existing;
    }

    public static Overwrite Construct(IDiscordClient client, IOverwriteModel model)
        => new(model.TargetId, (PermissionTarget)model.Type, (GuildPermission)model.Allow,
            (GuildPermission)model.Deny);
}
