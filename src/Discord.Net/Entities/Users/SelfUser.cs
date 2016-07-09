using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord
{
    internal class SelfUser : User, ISelfUser
    {
        private long _idleSince;
        private UserStatus _status;
        private Game _game;

        public string Email { get; private set; }
        public bool IsVerified { get; private set; }
        public bool IsMfaEnabled { get; private set; }

        public override UserStatus Status => _status;
        public override Game Game => _game;

        public override DiscordClient Discord { get; }

        public SelfUser(DiscordClient discord, Model model)
            : base(model)
        {
            Discord = discord;
        }
        public override void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            base.Update(model, source);

            Email = model.Email;
            IsVerified = model.Verified;
            IsMfaEnabled = model.MfaEnabled;
        }
        
        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetSelfAsync().ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyAsync(Action<ModifyCurrentUserParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyCurrentUserParams();
            func(args);

            if (!args.Username.IsSpecified)
                args.Username = Username;
            if (args.Avatar.IsSpecified && _avatarId != null)
                args.AvatarHash = _avatarId;

            var model = await Discord.ApiClient.ModifySelfAsync(args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyStatusAsync(Action<ModifyPresenceParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));
            
            var args = new ModifyPresenceParams();
            func(args);

            var game = args.Game.GetValueOrDefault(_game);
            var status = args.Status.GetValueOrDefault(_status);

            long idleSince = _idleSince;
            if (status == UserStatus.Idle && _status != UserStatus.Idle)
                idleSince = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var apiGame = new API.Game { Name = game.Name, StreamType = game.StreamType, StreamUrl = game.StreamUrl };

            await Discord.ApiClient.SendStatusUpdateAsync(status == UserStatus.Idle ? _idleSince : (long?)null, apiGame).ConfigureAwait(false);
            
            //Save values
            _idleSince = idleSince;
            _game = game;
            _status = status;
        }
    }
}
