using Discord.API.Rest;
using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GameEntity = Discord.Game;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSelfUser : SocketUser, ISelfUser
    {
        private DateTimeOffset? _statusSince;

        public string Email { get; private set; }
        public bool IsVerified { get; private set; }
        public bool IsMfaEnabled { get; private set; }
        internal override SocketGlobalUser GlobalUser { get; }

        public override bool IsBot { get { return GlobalUser.IsBot; } internal set { GlobalUser.IsBot = value; } }
        public override string Username { get { return GlobalUser.Username; } internal set { GlobalUser.Username = value; } }
        public override ushort DiscriminatorValue { get { return GlobalUser.DiscriminatorValue; } internal set { GlobalUser.DiscriminatorValue = value; } }
        public override string AvatarId { get { return GlobalUser.AvatarId; } internal set { GlobalUser.AvatarId = value; } }
        internal override SocketPresence Presence { get { return GlobalUser.Presence; } set { GlobalUser.Presence = value; } }

        internal SocketSelfUser(DiscordSocketClient discord, SocketGlobalUser globalUser)
            : base(discord, globalUser.Id)
        {
            GlobalUser = globalUser;
        }
        internal static SocketSelfUser Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            var entity = new SocketSelfUser(discord, discord.GetOrCreateSelfUser(state, model));
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);

            if (model.Email.IsSpecified)
                Email = model.Email.Value;
            if (model.Verified.IsSpecified)
                IsVerified = model.Verified.Value;
            if (model.MfaEnabled.IsSpecified)
                IsMfaEnabled = model.MfaEnabled.Value;
        }
        
        public Task ModifyAsync(Action<ModifyCurrentUserParams> func, RequestOptions options = null)
            => UserHelper.ModifyAsync(this, Discord, func, options);
        public async Task ModifyStatusAsync(Action<ModifyPresenceParams> func, RequestOptions options = null)
        {
            var args = new ModifyPresenceParams();
            func(args);            

            UserStatus status;
            if (args.Status.IsSpecified)
            {
                status = args.Status.Value;
                if (status == UserStatus.AFK)
                    _statusSince = DateTimeOffset.UtcNow;
                else
                    _statusSince = null;
            }
            else
                status = Status;

            GameEntity? game;
            if (args.Game.IsSpecified)
            {
                var model = args.Game.Value;
                if (model != null)
                    game = GameEntity.Create(model);
                else
                    game = null;
            }
            else
                game = Game;

            Presence = new SocketPresence(status, game);

            await SendStatus(status, game).ConfigureAwait(false);
        }
        internal async Task SendStatus(UserStatus status, GameEntity? game)
        {
            var gameModel = game != null ? new API.Game
            {
                Name = game.Value.Name,
                StreamType = game.Value.StreamType,
                StreamUrl = game.Value.StreamUrl
            } : null;

            await Discord.ApiClient.SendStatusUpdateAsync(
                status,
                status == UserStatus.AFK,
                _statusSince != null ? _statusSince.Value.ToUnixTimeMilliseconds() : (long?)null,
                gameModel).ConfigureAwait(false);
        }

        internal new SocketSelfUser Clone() => MemberwiseClone() as SocketSelfUser;
    }
}
