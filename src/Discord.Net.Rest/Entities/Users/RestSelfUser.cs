using Discord.API.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestSelfUser : RestUser, ISelfUser
    {
        public string Email { get; private set; }
        public bool IsVerified { get; private set; }
        public bool IsMfaEnabled { get; private set; }

        internal RestSelfUser(DiscordRestClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal new static RestSelfUser Create(DiscordRestClient discord, Model model)
        {
            var entity = new RestSelfUser(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            if (model.Email.IsSpecified)
                Email = model.Email.Value;
            if (model.Verified.IsSpecified)
                IsVerified = model.Verified.Value;
            if (model.MfaEnabled.IsSpecified)
                IsMfaEnabled = model.MfaEnabled.Value;
        }

        public override async Task UpdateAsync()
            => Update(await UserHelper.GetAsync(this, Discord));
        public Task ModifyAsync(Action<ModifyCurrentUserParams> func)
            => UserHelper.ModifyAsync(this, Discord, func);

        Task ISelfUser.ModifyStatusAsync(Action<ModifyPresenceParams> func) { throw new NotSupportedException(); }
    }
}
