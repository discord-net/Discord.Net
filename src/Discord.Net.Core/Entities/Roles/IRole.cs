using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic role object.
    /// </summary>
    public interface IRole : ISnowflakeEntity, IDeletable, IMentionable, IComparable<IRole>
    {
        /// <summary>
        ///     Gets the guild owning this role.
        /// </summary>
        IGuild Guild { get; }

        /// <summary>
        ///     Gets the color given to users of this role.
        /// </summary>
        Color Color { get; }
        /// <summary>
        ///     Determines whether the role can be separated in the user list.
        /// </summary>
        /// <returns>
        ///     Returns <see langword="true"/> if users of this role are separated in the user list; otherwise, returns 
        ///     <see langword="false"/>.
        /// </returns>
        bool IsHoisted { get; }
        /// <summary>
        ///     Determines whether the role is managed by Discord.
        /// </summary>
        /// <returns>
        ///     Returns <see langword="true"/> if this role is automatically managed by Discord; otherwise, returns 
        ///     <see langword="false"/>.
        /// </returns>
        bool IsManaged { get; }
        /// <summary>
        ///     Determines whether the role is mentionable.
        /// </summary>
        /// <returns>
        ///     Returns <see langword="true"/> if this role may be mentioned in messages; otherwise, returns 
        ///     <see langword="false"/>.
        /// </returns>
        bool IsMentionable { get; }
        /// <summary>
        ///     Gets the name of this role.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Gets the permissions granted to members of this role.
        /// </summary>
        GuildPermissions Permissions { get; }
        /// <summary>
        ///     Gets this role's position relative to other roles in the same guild.
        /// </summary>
        int Position { get; }

        /// <summary>
        ///     Modifies this role.
        /// </summary>
        /// <param name="func">The properties to modify the role with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null);
    }
}
