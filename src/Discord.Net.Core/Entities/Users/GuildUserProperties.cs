using System;
using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify an <see cref="IGuildUser" /> with the following parameters.
    /// </summary>
    /// <seealso cref="IGuildUser.ModifyAsync" />
    public class GuildUserProperties
    {
        /// <summary>
        ///     Gets or sets whether the user should be muted in a voice channel.
        /// </summary>
        /// <remarks>
        ///     If this value is set to <see langword="true" />, no user will be able to hear this user speak in the guild.
        /// </remarks>
        public Optional<bool> Mute { get; set; }
        /// <summary>
        ///     Gets or sets whether the user should be deafened in a voice channel.
        /// </summary>
        /// <remarks>
        ///     If this value is set to <see langword="true" />, this user will not be able to hear anyone speak in the guild.
        /// </remarks>
        public Optional<bool> Deaf { get; set; }
        /// <summary>
        ///     Gets or sets the user's nickname.
        /// </summary>
        /// <remarks>
        ///     To clear the user's nickname, this value can be set to <see langword="null" /> or
        ///     <see cref="string.Empty"/>.
        /// </remarks>
        public Optional<string> Nickname { get; set; }
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
        /// <summary>
        ///     Moves a user to a voice channel. If <see langword="null" />, this user will be disconnected from their current voice channel.
        /// </summary>
        /// <remarks>
        ///     This user MUST already be in a <see cref="IVoiceChannel"/> for this to work.
        ///     When set, this property takes precedence over <see cref="ChannelId"/>.
        /// </remarks>
        public Optional<IVoiceChannel> Channel { get; set; }
        /// <summary>
        ///     Moves a user to a voice channel. Set <see cref="Channel"/> to <see langword="null" /> to disconnect this user from their current voice channel.
        /// </summary>
        /// <remarks>
        ///     This user MUST already be in a <see cref="IVoiceChannel"/> for this to work.
        /// </remarks>
        public Optional<ulong?> ChannelId { get; set; }

        /// <summary>
        ///     Sets a timestamp how long a user should be timed out for.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> or a time in the past to clear a currently existing timeout.
        /// </remarks>
        public Optional<DateTimeOffset?> TimedOutUntil { get; set; }

        /// <summary>
        ///     Gets or sets the flags of the guild member.
        /// </summary>
        /// <remarks>
        ///     Not all flags can be modified, these are reserved for Discord.
        /// </remarks>
        public Optional<GuildUserFlags> Flags { get; set; }
    }
}
