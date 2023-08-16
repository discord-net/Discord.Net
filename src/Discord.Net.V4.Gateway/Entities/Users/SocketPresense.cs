using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public sealed class SocketPresense : SocketCacheableEntity<ulong, IPresenseModel>, IPresence
    {
        public UserStatus Status
            => _source.Status;

        public IReadOnlyCollection<ClientType> ActiveClients
            => _source.ClientStatus.ToImmutableArray();

        public IReadOnlyCollection<IActivity> Activities => throw new NotImplementedException(); // TODO: model -> activity

        private IPresenseModel _source;

        public SocketPresense(DiscordGatewayClient discord, IPresenseModel model)
            : base(discord, model.UserId)
        {
            _source = model;
        }

        internal override void Update(IPresenseModel model)
        {
            _source = model;
        }

        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
        internal override IPresenseModel GetModel() => throw new NotImplementedException();
    }
}
