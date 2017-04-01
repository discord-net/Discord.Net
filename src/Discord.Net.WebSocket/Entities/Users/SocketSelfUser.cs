using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSelfUser : SocketUser, ISelfUser
    {
        public string Email { get; private set; }
        public bool IsVerified { get; private set; }
        public bool IsMfaEnabled { get; private set; }
        internal override SocketGlobalUser GlobalUser { get; }

        public override bool IsBot { get { return GlobalUser.IsBot; } internal set { GlobalUser.IsBot = value; } }
        public override string Username { get { return GlobalUser.Username; } internal set { GlobalUser.Username = value; } }
        public override ushort DiscriminatorValue { get { return GlobalUser.DiscriminatorValue; } internal set { GlobalUser.DiscriminatorValue = value; } }
        public override string AvatarId { get { return GlobalUser.AvatarId; } internal set { GlobalUser.AvatarId = value; } }
        internal override SocketPresence Presence { get { return GlobalUser.Presence; } set { GlobalUser.Presence = value; } }

        public override bool IsWebhook => false;

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
        internal override bool Update(ClientState state, Model model)
        {
            bool hasGlobalChanges = base.Update(state, model);
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
            if (model.MfaEnabled.IsSpecified)
            {
                IsMfaEnabled = model.MfaEnabled.Value;
                hasGlobalChanges = true;
            }
            return hasGlobalChanges;
        }
        
        public Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions options = null)
            => UserHelper.ModifyAsync(this, Discord, func, options);

        internal new SocketSelfUser Clone() => MemberwiseClone() as SocketSelfUser;
    }
}
