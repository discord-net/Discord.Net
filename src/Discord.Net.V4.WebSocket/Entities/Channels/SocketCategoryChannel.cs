using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public sealed class SocketCategoryChannel : SocketGuildChannel, ICategoryChannel
    {
        public SocketCategoryChannel(DiscordSocketClient discord, ulong guildId, ulong id, IGuildChannelModel model)
            : base(discord, guildId, id, model)
        {
        }

        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
    }
}
