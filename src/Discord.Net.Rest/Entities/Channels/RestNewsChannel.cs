using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based news channel in a guild that has the same properties as a <see cref="RestTextChannel"/>.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestNewsChannel : RestTextChannel
    {
        internal RestNewsChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            :base(discord, guild, id)
        {
        }
        internal new static RestNewsChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestNewsChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
        public override int SlowModeInterval => throw new NotSupportedException("News channels do not support Slow Mode.");
        public override Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null)
        {
            throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        }
        public override Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null)
        {
            throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        }
        public override OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            throw new NotSupportedException("News channels do not support Overwrite Permissions.");
        }
        public override OverwritePermissions? GetPermissionOverwrite(IUser user)
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
