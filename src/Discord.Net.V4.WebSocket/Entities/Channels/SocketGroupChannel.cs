using Discord.Audio;
using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal sealed class SocketGroupChannel : SocketMessageChannel, IGroupChannel, ICacheableEntity<ulong, IGroupDMChannelModel>
    {
        public IReadOnlyCollection<UserCacheable> Recipients
            => _recipients.ToImmutableArray();

        public string? RTCRegion
            => _source.RTCRegion;

        protected override IGroupDMChannelModel Model
            => _source;

        private IGroupDMChannelModel _source;

        private UserCacheable[] _recipients;

        public SocketGroupChannel(DiscordSocketClient discord, ulong id, IGroupDMChannelModel model) : base(discord, id, model)
        {
            _source = model;
            _recipients = _source.Recipients
                .Select(x => new UserCacheable(x, Discord, Discord.State.Users.SourceSpecific(x)))
                .ToArray();
        }

        public void Update(IGroupDMChannelModel model)
        {
            if(!_source.Recipients.SequenceEqual(model.Recipients))
            {
                _recipients = _source.Recipients
                    .Select(x => new UserCacheable(x, Discord, Discord.State.Users.SourceSpecific(x)))
                    .ToArray();
            }

            _source = model;
        }

        internal override void Update(IChannelModel model)
        {
            if (model is IGroupDMChannelModel groupModel)
                Update(groupModel);
        }

        IGroupDMChannelModel ICacheableEntity<ulong, IGroupDMChannelModel>.GetModel()
            => _source;

        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();

        public Task LeaveAsync(RequestOptions options = null) => throw new NotImplementedException();
        public Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false) => throw new NotImplementedException();
        public Task DisconnectAsync() => throw new NotImplementedException();
        public Task ModifyAsync(Action<AudioChannelProperties> func, RequestOptions options = null) => throw new NotImplementedException();

        IReadOnlyCollection<IUser> IPrivateChannel.Recipients
            => _recipients.Select(x => x.Value).ToImmutableArray();
    }
}
