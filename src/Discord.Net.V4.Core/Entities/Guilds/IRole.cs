namespace Discord;

public interface IRole : ISnowflakeEntity, IDeletable, IMentionable, IComparable<IRole>, IModifyable<ModifyRoleProperties>
{
    IEntitySource<ulong, IGuild> Guild { get; }

    /// <summary>
    ///     Gets the color given to users of this role.
    /// </summary>
    /// <returns>
    ///     A <see cref="Color"/> struct representing the color of this role.
    /// </returns>
    Color Color { get; }

    /// <summary>
    ///     Gets a value that indicates whether the role can be separated in the user list.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if users of this role are separated in the user list; otherwise <see langword="false" />.
    /// </returns>
    bool IsHoisted { get; }

    /// <summary>
    ///     Gets a value that indicates whether the role is managed by Discord.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this role is automatically managed by Discord; otherwise <see langword="false" />.
    /// </returns>
    bool IsManaged { get; }

    /// <summary>
    ///     Gets a value that indicates whether the role is mentionable.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this role may be mentioned in messages; otherwise <see langword="false" />.
    /// </returns>
    bool IsMentionable { get; }

    /// <summary>
    ///     Gets the name of this role.
    /// </summary>
    /// <returns>
    ///     A string containing the name of this role.
    /// </returns>
    string Name { get; }

    /// <summary>
    ///     Gets the icon id of this role.
    /// </summary>
    /// <remarks>
    ///     This field is mutually exclusive with <see cref="Emoji"/>, either icon is set or emoji is set.
    /// </remarks>
    /// <returns>
    ///     A string containing the hash of this role's icon.
    /// </returns>
    string? IconId { get; }

    /// <summary>
    ///     Gets the image url of the icon role.
    /// </summary>
    /// <returns>
    ///     An image url of the icon role.
    /// </returns>
    string? IconUrl
        => CDN.GetGuildRoleIconUrl(Client.Config, Id, IconId);

    /// <summary>
    ///     Gets the unicode emoji of this role.
    /// </summary>
    /// <remarks>
    ///     This field is mutually exclusive with <see cref="Icon"/>, either icon is set or emoji is set.
    /// </remarks>
    Emoji? Emoji { get; }

    /// <summary>
    ///     Gets the permissions granted to members of this role.
    /// </summary>
    GuildPermission Permissions { get; }

    /// <summary>
    ///     Gets this role's position relative to other roles in the same guild.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the position of the role in the role list of the guild.
    /// </returns>
    int Position { get; }

    /// <summary>
    ///     Gets the tags related to this role.
    /// </summary>
    /// <returns>
    ///     A <see cref="RoleTags"/> object containing all tags related to this role.
    /// </returns>
    RoleTags Tags { get; }

    /// <summary>
    ///     Gets flags related to this role.
    /// </summary>
    RoleFlags Flags { get; }
}
