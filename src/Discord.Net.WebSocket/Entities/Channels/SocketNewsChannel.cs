using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based news channel in a guild that has the same properties as a <see cref="SocketTextChannel"/>.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         Most of the properties and methods featured may not be supported due to the nature of the channel.
    ///     </note>
    /// </remarks>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketNewsChannel : SocketTextChannel
    {
        internal SocketNewsChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            :base(discord, id, guild)
        {
        }
        internal new static SocketNewsChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketNewsChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
        /// <inheritdoc />
        /// <remarks>
        ///     <note type="important">
        ///     This property is not supported by this type. Attempting to use this property will result in a <see cref="NotSupportedException"/>.
        ///     </note>
        /// </remarks>
        public override int SlowModeInterval
            => throw new NotSupportedException("News channels do not support Slow Mode.");
        /// <inheritdoc />
        /// <remarks>
        ///     <note type="important">
        ///     This method is not supported by this type. Attempting to use this method will result in a <see cref="NotSupportedException"/>.
        ///     </note>
        /// </remarks>
        public override Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null)
            => throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        /// <inheritdoc />
        /// <remarks>
        ///     <note type="important">
        ///     This method is not supported by this type. Attempting to use this method will result in a <see cref="NotSupportedException"/>.
        ///     </note>
        /// </remarks>
        public override Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null)
            => throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        /// <inheritdoc />
        /// <remarks>
        ///     <note type="important">
        ///     This property is not supported by this type. Attempting to use this property will result in a <see cref="NotSupportedException"/>.
        ///     </note>
        /// </remarks>
        public override IReadOnlyCollection<Overwrite> PermissionOverwrites
            => throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        /// <inheritdoc />
        /// <remarks>
        ///     <note type="important">
        ///     This method is not supported by this type. Attempting to use this method will result in a <see cref="NotSupportedException"/>.
        ///     </note>
        /// </remarks>
        public override Task SyncPermissionsAsync(RequestOptions options = null)
            => throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        /// <inheritdoc />
        /// <remarks>
        ///     <note type="important">
        ///     This method is not supported by this type. Attempting to use this method will result in a <see cref="NotSupportedException"/>.
        ///     </note>
        /// </remarks>
        public override Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
            => throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        /// <inheritdoc />
        /// <remarks>
        ///     <note type="important">
        ///     This method is not supported by this type. Attempting to use this method will result in a <see cref="NotSupportedException"/>.
        ///     </note>
        /// </remarks>
        public override Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null)
            => throw new NotSupportedException("News channels do not support Overwrite Permissions.");
    }
}
