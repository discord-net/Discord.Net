using Discord.Net.V4.Core.API.Models.Channels;
using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal sealed class SocketDMChannel : SocketMessageChannel, IDMChannel 
    {
        public UserCacheable Recipient { get; }

        protected override IDMChannelModel Model
            => _source;

        private IDMChannelModel _source;

        public SocketDMChannel(DiscordSocketClient discord, ulong id, IDMChannelModel model) : base(discord, id, model)
        {
            _source = model;

            Recipient = new(
                model.RecipientId,
                discord,
                discord.State.Users.SourceSpecific(model.RecipientId)
            );
        }


        internal override void Update(IChannelModel model)
        {
            if (model is IDMChannelModel dmChannelModel)
                _source = dmChannelModel;
        }

        public IReadOnlyCollection<IUser> Recipients => throw new NotImplementedException(); // TODO: concat current user

        public Task CloseAsync(RequestOptions? options = null) => throw new NotImplementedException();
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();

        IUser? IDMChannel.Recipient => Recipient.Value;
    }
}
