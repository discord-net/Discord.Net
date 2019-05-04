using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based news channel in a guild that has the same properties as a <see cref="RestTextChannel"/>.
    /// </summary>
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
        public override int SlowModeInterval
        {
            get { throw new NotSupportedException("News channels do not support Slow Mode."); }
        }
        public override Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null)
        {
            throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        }
        public override Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null)
        {
            throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        }
        public override IReadOnlyCollection<Overwrite> PermissionOverwrites
            => throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        public override Task SyncPermissionsAsync(RequestOptions options = null)
        {
            throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        }
        public override Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
        {
            throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        }
        public override Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null)
        {
            throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        }
    }
}
