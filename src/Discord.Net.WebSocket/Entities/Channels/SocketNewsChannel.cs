using System.Diagnostics;
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
        internal new static SocketTextChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketNewsChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
    }
}
