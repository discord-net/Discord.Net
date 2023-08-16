using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public sealed class SocketCategoryChannel : SocketGuildChannel, ICategoryChannel
    {
        protected override IGuildChannelModel Model
            => _source;

        private IGuildChannelModel _source;

        public SocketCategoryChannel(DiscordGatewayClient discord, ulong guildId, IGuildChannelModel model)
            : base(discord, guildId, model)
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
