using System;
using System.Diagnostics;
using Model = Discord.API.User;
using PresenceModel = Discord.API.Presence;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSimpleUser : SocketUser
    {
        public override bool IsBot { get; internal set; }
        public override string Username { get; internal set; }
        public override ushort DiscriminatorValue { get; internal set; }
        public override string AvatarId { get; internal set; }
        internal override SocketPresence Presence { get { return new SocketPresence(UserStatus.Offline, null); } set { } }
        internal override SocketGlobalUser GlobalUser {  get { throw new NotSupportedException(); } }

        internal SocketSimpleUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static SocketSimpleUser Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            var entity = new SocketSimpleUser(discord, model.Id);
            entity.Update(state, model);
            return entity;
        }

        internal override void Update(ClientState state, PresenceModel model)
        {
            if (model.User.Avatar.IsSpecified)
                AvatarId = model.User.Avatar.Value;
            if (model.User.Discriminator.IsSpecified)
                DiscriminatorValue = ushort.Parse(model.User.Discriminator.Value);
            if (model.User.Bot.IsSpecified)
                IsBot = model.User.Bot.Value;
            if (model.User.Username.IsSpecified)
                Username = model.User.Username.Value;
        }

        internal new SocketSimpleUser Clone() => MemberwiseClone() as SocketSimpleUser;
    }
}
