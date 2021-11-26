using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents the logged-in REST-based user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestSelfUser : RestUser, ISelfUser
    {
        /// <inheritdoc />
        public string Email { get; private set; }
        /// <inheritdoc />
        public bool IsVerified { get; private set; }
        /// <inheritdoc />
        public bool IsMfaEnabled { get; private set; }
        /// <inheritdoc />
        public UserProperties Flags { get; private set; }
        /// <inheritdoc />
        public PremiumType PremiumType { get; private set; }
        /// <inheritdoc />
        public string Locale { get; private set; }

        internal RestSelfUser(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal new static RestSelfUser Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestSelfUser(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        /// <inheritdoc />
        internal override void Update(Model model)
        {
            base.Update(model);

            if (model.Email.IsSpecified)
                Email = model.Email.Value;
            if (model.Verified.IsSpecified)
                IsVerified = model.Verified.Value;
            if (model.MfaEnabled.IsSpecified)
                IsMfaEnabled = model.MfaEnabled.Value;
            if (model.Flags.IsSpecified)
                Flags = (UserProperties)model.Flags.Value;
            if (model.PremiumType.IsSpecified)
                PremiumType = model.PremiumType.Value;
            if (model.Locale.IsSpecified)
                Locale = model.Locale.Value;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Unable to update this object using a different token.</exception>
        public override async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetMyUserAsync(options).ConfigureAwait(false);
            if (model.Id != Id)
                throw new InvalidOperationException("Unable to update this object using a different token.");
            Update(model);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Unable to modify this object using a different token.</exception>
        public async Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions options = null)
        {
            if (Id != Discord.CurrentUser.Id)
                throw new InvalidOperationException("Unable to modify this object using a different token.");
            var model = await UserHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }
    }
}
