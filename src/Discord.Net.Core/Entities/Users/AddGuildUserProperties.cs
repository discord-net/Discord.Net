using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Properties that are used to add a new <see cref="IGuildUser"/> to the guild with the following parameters.
    /// </summary>
    /// <seealso cref="IGuild.AddGuildUserAsync" />
    public class AddGuildUserProperties
    {
        /// <summary>
        ///     Gets or sets the user's nickname.
        /// </summary>
        /// <remarks>
        ///     To clear the user's nickname, this value can be set to <c>null</c> or
        ///     <see cref="string.Empty"/>.
        /// </remarks>
        public Optional<string> Nickname { get; set; }
        /// <summary>
        ///     Gets or sets whether the user should be muted in a voice channel.
        /// </summary>
        /// <remarks>
        ///     If this value is set to <c>true</c>, no user will be able to hear this user speak in the guild.
        /// </remarks>
        public Optional<bool> Mute { get; set; }
        /// <summary>
        ///     Gets or sets whether the user should be deafened in a voice channel.
        /// </summary>
        /// <remarks>
        ///     If this value is set to <c>true</c>, this user will not be able to hear anyone speak in the guild.
        /// </remarks>
        public Optional<bool> Deaf { get; set; }
        /// <summary>
        ///     Gets or sets the roles the user should have.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         To add a role to a user:
        ///         <see cref="IGuildUser.AddRolesAsync(IEnumerable{IRole},RequestOptions)" />
        ///     </para>
        ///     <para>
        ///         To remove a role from a user:
        ///         <see cref="IGuildUser.RemoveRolesAsync(IEnumerable{IRole},RequestOptions)" />
        ///     </para>
        /// </remarks>
        public Optional<IEnumerable<IRole>> Roles { get; set; }
        /// <summary>
        ///     Gets or sets the roles the user should have.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         To add a role to a user:
        ///         <see cref="IGuildUser.AddRolesAsync(IEnumerable{IRole},RequestOptions)" />
        ///     </para>
        ///     <para>
        ///         To remove a role from a user:
        ///         <see cref="IGuildUser.RemoveRolesAsync(IEnumerable{IRole},RequestOptions)" />
        ///     </para>
        /// </remarks>
        public Optional<IEnumerable<ulong>> RoleIds { get; set; }
    }
}
