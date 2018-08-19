using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord.Rest;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSelfUser : SocketUser, ISelfUser
    {
        internal SocketSelfUser(DiscordSocketClient discord, SocketGlobalUser globalUser)
            : base(discord, globalUser.Id)
        {
            GlobalUser = globalUser;
        }

        internal override SocketGlobalUser GlobalUser { get; }

        internal override SocketPresence Presence
        {
            get => GlobalUser.Presence;
            set => GlobalUser.Presence = value;
        }

        public string Email { get; private set; }
        public bool IsVerified { get; private set; }
        public bool IsMfaEnabled { get; private set; }

        public override bool IsBot
        {
            get => GlobalUser.IsBot;
            internal set => GlobalUser.IsBot = value;
        }

        public override string Username
        {
            get => GlobalUser.Username;
            internal set => GlobalUser.Username = value;
        }

        public override ushort DiscriminatorValue
        {
            get => GlobalUser.DiscriminatorValue;
            internal set => GlobalUser.DiscriminatorValue = value;
        }

        public override string AvatarId
        {
            get => GlobalUser.AvatarId;
            internal set => GlobalUser.AvatarId = value;
        }

        public override bool IsWebhook => false;

        public Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions options = null)
            => UserHelper.ModifyAsync(this, Discord, func, options);

        internal static SocketSelfUser Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            var entity = new SocketSelfUser(discord, discord.GetOrCreateSelfUser(state, model));
            entity.Update(state, model);
            return entity;
        }

        internal override bool Update(ClientState state, Model model)
        {
            var hasGlobalChanges = base.Update(state, model);
            if (model.Email.IsSpecified)
            {
                Email = model.Email.Value;
                hasGlobalChanges = true;
            }

            if (model.Verified.IsSpecified)
            {
                IsVerified = model.Verified.Value;
                hasGlobalChanges = true;
            }

            if (!model.MfaEnabled.IsSpecified) return hasGlobalChanges;
            IsMfaEnabled = model.MfaEnabled.Value;

            return true;
        }

        internal new SocketSelfUser Clone() => MemberwiseClone() as SocketSelfUser;
    }
}
