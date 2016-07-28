using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord
{
    internal class SelfUser : User, ISelfUser
    {
        protected long _idleSince;
        protected UserStatus _status;
        protected Game _game;

        public string Email { get; private set; }
        public bool IsVerified { get; private set; }
        public bool IsMfaEnabled { get; private set; }

        public override UserStatus Status => _status;
        public override Game Game => _game;

        public override DiscordRestClient Discord { get; }

        public SelfUser(DiscordRestClient discord, Model model)
            : base(model)
        {
            Discord = discord;
        }
        public override void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            base.Update(model, source);

            if (model.Email.IsSpecified)
                Email = model.Email.Value;
            if (model.Verified.IsSpecified)
                IsVerified = model.Verified.Value;
            if (model.MfaEnabled.IsSpecified)
                IsMfaEnabled = model.MfaEnabled.Value;
        }
        
        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetMyUserAsync().ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyAsync(Action<ModifyCurrentUserParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyCurrentUserParams();
            func(args);

            if (!args._username.IsSpecified)
                args._username = Username;
            if (!args._avatar.IsSpecified && _avatarId != null)
                args._avatar = new API.Image(_avatarId);

            var model = await Discord.ApiClient.ModifySelfAsync(args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }

        Task ISelfUser.ModifyStatusAsync(Action<ModifyPresenceParams> func) { throw new NotSupportedException(); }
    }
}
