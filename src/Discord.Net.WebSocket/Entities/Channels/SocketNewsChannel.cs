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
    ///         The <see cref="SlowModeInterval"/> property is not supported for news channels.
    ///     </note>
    /// </remarks>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketNewsChannel : SocketTextChannel, INewsChannel
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

        /// <summary>
        ///     <b>News channels do not support thread creation with bots.</b>
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="NotSupportedException"></exception>
        public override Task<SocketThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
            => throw new NotSupportedException("Cannot create threads in News channels");
    }
}
