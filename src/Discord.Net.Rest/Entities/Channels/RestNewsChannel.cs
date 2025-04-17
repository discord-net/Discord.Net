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
    public class RestNewsChannel : RestTextChannel, INewsChannel
    {
        internal RestNewsChannel(BaseDiscordClient discord, IGuild guild, ulong id, ulong guildId)
            : base(discord, guild, id, guildId)
        {
        }
        internal new static RestNewsChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestNewsChannel(discord, guild, model.Id, guild?.Id ?? model.GuildId.Value);
            entity.Update(model);
            return entity;
        }
        public override int SlowModeInterval => throw new NotSupportedException("News channels do not support Slow Mode.");

        private string DebuggerDisplay => $"{Name} ({Id}, News)";

        /// <inheritdoc />
        public Task<ulong> FollowAnnouncementChannelAsync(ulong channelId, RequestOptions options = null)
            => ChannelHelper.FollowAnnouncementChannelAsync(this, channelId, Discord, options);
    }
}
