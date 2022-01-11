using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic role object to be given to a guild user.
    /// </summary>
    public interface IRole : ISnowflakeEntity, IDeletable, IMentionable, IComparable<IRole>
    {
        /// <summary>
        ///     Gets the guild that owns this role.
        /// </summary>
        /// <returns>
        ///     A guild representing the parent guild of this role.
        /// </returns>
        IGuild Guild { get; }

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
        ///     <c>true</c> if users of this role are separated in the user list; otherwise <c>false</c>.
        /// </returns>
        bool IsHoisted { get; }
        /// <summary>
        ///     Gets a value that indicates whether the role is managed by Discord.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this role is automatically managed by Discord; otherwise <c>false</c>.
        /// </returns>
        bool IsManaged { get; }
        /// <summary>
        ///     Gets a value that indicates whether the role is mentionable.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this role may be mentioned in messages; otherwise <c>false</c>.
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
        ///     Gets the icon of this role.
        /// </summary>
        /// <returns>
        ///     A string containing the hash of this role's icon.
        /// </returns>
        string Icon { get; }
        /// <summary>
        ///     Gets the unicode emoji of this role.
        /// </summary>
        /// <remarks>
        ///     This field is mutually exclusive with <see cref="Icon"/>, either icon is set or emoji is set.
        /// </remarks>
        Emoji Emoji { get; }
        /// <summary>
        ///     Gets the permissions granted to members of this role.
        /// </summary>
        /// <returns>
        ///     A <see cref="GuildPermissions"/> struct that this role possesses.
        /// </returns>
        GuildPermissions Permissions { get; }
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
        ///     Modifies this role.
        /// </summary>
        /// <remarks>
        ///     This method modifies this role with the specified properties. To see an example of this
        ///     method and what properties are available, please refer to <see cref="RoleProperties"/>.
        /// </remarks>
        /// <param name="func">A delegate containing the properties to modify the role with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Gets the image url of the icon role.
        /// </summary>
        /// <returns>
        ///     An image url of the icon role.
        /// </returns>
        string GetIconUrl();
    }
}
