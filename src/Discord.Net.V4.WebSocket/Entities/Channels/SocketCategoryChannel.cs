using Discord.Net.V4.Core.API.Models.Channels;
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
        protected override IGuildChannelModel Model
            => _source;

        private IGuildChannelModel _source;

        public SocketCategoryChannel(DiscordSocketClient discord, ulong guildId, ulong id, IGuildChannelModel model)
            : base(discord, guildId, id, model)
        {
            _source = model;
        }

        internal override void Update(IChannelModel model)
        {
            if (model is IGuildChannelModel gcmodel)
                _source = gcmodel;
        }

        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
        
    }
}
