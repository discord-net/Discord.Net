using Discord.API.Rest;
using Discord.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    internal class SocketSelfUser : SelfUser, ISocketUser, ISelfUser
    {
        internal override bool IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        SocketGlobalUser ISocketUser.User { get { throw new NotSupportedException(); } }

        public SocketSelfUser(DiscordSocketClient discord, Model model) 
            : base(discord, model)
        {
        }

        public async Task ModifyStatusAsync(Action<ModifyPresenceParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyPresenceParams();
            func(args);

            var game = args._game.GetValueOrDefault(_game);
            var status = args._status.GetValueOrDefault(_status);

            long idleSince = _idleSince;
            if (status == UserStatus.Idle && _status != UserStatus.Idle)
                idleSince = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var apiGame = game != null ? new API.Game { Name = game.Name, StreamType = game.StreamType, StreamUrl = game.StreamUrl } : null;

            await Discord.ApiClient.SendStatusUpdateAsync(status == UserStatus.Idle ? _idleSince : (long?)null, apiGame).ConfigureAwait(false);

            //Save values
            _idleSince = idleSince;
            _game = game;
            _status = status;
        }

        public SocketSelfUser Clone() => MemberwiseClone() as SocketSelfUser;
        ISocketUser ISocketUser.Clone() => Clone();
    }
}
