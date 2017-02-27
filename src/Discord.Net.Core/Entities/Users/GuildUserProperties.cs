using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    /// Modify an IGuildUser with the following parameters.
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// await (Context.User as IGuildUser)?.ModifyAsync(x =>
    /// {
    ///     x.Nickname = $"festive {Context.User.Username}";
    /// });
    /// </code>
    /// </example>
    /// <seealso cref="IGuildUser"/>
    public class GuildUserProperties
    {
        /// <summary>
        /// Should the user be guild-muted in a voice channel?
        /// </summary>
        /// <remarks>
        /// If this value is set to true, no user will be able to hear this user speak in the guild.
        /// </remarks>
        public Optional<bool> Mute { get; set; }
        /// <summary>
        /// Should the user be guild-deafened in a voice channel?
        /// </summary>
        /// <remarks>
        /// If this value is set to true, this user will not be able to hear anyone speak in the guild.
        /// </remarks>
        public Optional<bool> Deaf { get; set; }
        /// <summary>
        /// Should the user have a nickname set? 
        /// </summary>
        /// <remarks>
        /// To clear the user's nickname, this value can be set to null.
        /// </remarks>
        public Optional<string> Nickname { get; set; }
        /// <summary>
        /// What roles should the user have?
        /// </summary>
        /// <remarks>
        /// To add a role to a user: <see cref="IGuildUser.AddRolesAsync(IEnumerable&lt;IRole&gt;, RequestOptions)"/>
        /// To remove a role from a user: <see cref="IGuildUser.RemoveRolesAsync(IEnumerable&lt;IRole&gt;, RequestOptions)"/>
        /// </remarks>
        public Optional<IEnumerable<IRole>> Roles { get; set; }
        /// <summary>
        /// What roles should the user have?
        /// </summary>
        /// <remarks>
        /// To add a role to a user: <see cref="IGuildUser.AddRolesAsync(IEnumerable&lt;IRole&gt;, RequestOptions)"/>
        /// To remove a role from a user: <see cref="IGuildUser.RemoveRolesAsync(IEnumerable&lt;IRole&gt;, RequestOptions)"/>
        /// </remarks>
        public Optional<IEnumerable<ulong>> RoleIds { get; set; }
        /// <summary>
        /// Move a user to a voice channel.
        /// </summary>
        /// <remarks>
        /// This user MUST already be in a Voice Channel for this to work.
        /// </remarks>
        public Optional<IVoiceChannel> Channel { get; set; }
        /// <summary>
        /// Move a user to a voice channel.
        /// </summary>
        /// <remarks>
        /// This user MUST already be in a Voice Channel for this to work.
        /// </remarks>
        public Optional<ulong> ChannelId { get; set; }
    }
}
