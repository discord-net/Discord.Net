using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public sealed class GatewayCategoryChannel : GatewayGuildChannel, ICategoryChannel
    {
        protected override IGuildChannelModel Model
            => _source;

        private IGuildChannelModel _source;

        public GatewayCategoryChannel(DiscordGatewayClient discord, ulong guildId, IGuildChannelModel model)
            : base(discord, guildId, model)
        {
            Update(model);
        }

        [MemberNotNull(nameof(_source))]
        internal void Update(IGuildChannelModel model)
        {
            _source = model;
        }

        internal override void Update(IChannelModel model)
        {
            if (model is IGuildChannelModel gcmodel)
                Update(gcmodel);
        }

        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
        
    }
}
