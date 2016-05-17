using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    public class SelfUser : User, ISelfUser
    {
        /// <inheritdoc />
        public string Email { get; private set; }
        /// <inheritdoc />
        public bool IsVerified { get; private set; }

        internal SelfUser(DiscordClient discord, Model model)
            : base(discord, model)
        {
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Email = model.Email;
            IsVerified = model.IsVerified;
        }
        
        /// <inheritdoc />
        public async Task Modify(Action<ModifyCurrentUserParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyCurrentUserParams();
            func(args);
            await Discord.ApiClient.ModifyCurrentUser(args).ConfigureAwait(false);
        }

        Task IUpdateable.Update() 
            => Task.CompletedTask;
    }
}
