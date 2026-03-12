using System;
using Model = Discord.API.AuditLogs.RoleInfoAuditLogModel;

namespace Discord.Rest;

/// <summary>
///     Represents information for a role edit.
/// </summary>
public struct RoleEditInfo
{
    internal RoleEditInfo(Model model)
    {
        Colors = model.Colors?.ToEntity() ?? model.Color;
        Mentionable = model.IsMentionable;
        Hoist = model.Hoist;
        Name = model.Name;

        if (model.Permissions is not null)
            Permissions = new GuildPermissions(model.Permissions.Value);
        else
            Permissions = null;

        IconId = model.IconHash;
    }

    /// <summary>
    ///     Gets the color of this role.
    /// </summary>
    /// <returns>
    ///     A color object representing the color assigned to this role; <see langword="null" /> if this role does not have a
    ///     color.
    /// </returns>
    /// <remarks>
    ///     This property is deprecated in the API, and its value will always be <see cref="Colors"/>.PrimaryColor
    /// </remarks>
    [Obsolete(
        $"This property is deprecated in the API, and its value will always be {nameof(Colors)}.{nameof(RoleColors.PrimaryColor)}",
        error: false
    )]
    public Color? Color => Colors?.PrimaryColor;

    /// <summary>
    ///     Gets the full color configuration of this role.
    /// </summary>
    public RoleColors? Colors { get; }

    /// <summary>
    ///     Gets a value that indicates whether this role is mentionable.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if other members can mention this role in a text channel; otherwise <see langword="false" />;
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public bool? Mentionable { get; }

    /// <summary>
    ///     Gets a value that indicates whether this role is hoisted (i.e. its members will appear in a separate
    ///     section on the user list).
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this role's members will appear in a separate section in the user list; otherwise
    ///     <see langword="false" />; <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public bool? Hoist { get; }

    /// <summary>
    ///     Gets the name of this role.
    /// </summary>
    /// <returns>
    ///    A string containing the name of this role.
    /// </returns>
    public string Name { get; }

    /// <summary>
    ///     Gets the permissions assigned to this role.
    /// </summary>
    /// <returns>
    ///     A guild permissions object representing the permissions that have been assigned to this role; <see langword="null" />
    ///     if no permissions have been assigned.
    /// </returns>
    public GuildPermissions? Permissions { get; }

    /// <inheritdoc cref="IRole.Icon"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string IconId { get; }
}
