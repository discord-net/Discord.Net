using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SocketGroupUser : SocketUser, IGroupUser
    {
        public SocketGroupChannel Channel { get; }
        internal override SocketGlobalUser GlobalUser { get; }

        public override bool IsBot { get { return GlobalUser.IsBot; } internal set { GlobalUser.IsBot = value; } }
        public override string Username { get { return GlobalUser.Username; } internal set { GlobalUser.Username = value; } }
        public override ushort DiscriminatorValue { get { return GlobalUser.DiscriminatorValue; } internal set { GlobalUser.DiscriminatorValue = value; } }
        public override string AvatarId { get { return GlobalUser.AvatarId; } internal set { GlobalUser.AvatarId = value; } }
        internal override SocketPresence Presence { get { return GlobalUser.Presence; } set { GlobalUser.Presence = value; } }

        public override bool IsWebhook => false;

        internal SocketGroupUser(SocketGroupChannel channel, SocketGlobalUser globalUser)
            : base(channel.Discord, globalUser.Id)
        {
            Channel = channel;
            GlobalUser = globalUser;
        }
        internal static SocketGroupUser Create(SocketGroupChannel channel, ClientState state, Model model)
        {
            var entity = new SocketGroupUser(channel, channel.Discord.GetOrCreateUser(state, model));
            entity.Update(state, model);
            return entity;
        }

        internal new SocketGroupUser Clone() => MemberwiseClone() as SocketGroupUser;

        //IVoiceState
        bool IVoiceState.IsDeafened => false;
        bool IVoiceState.IsMuted => false;
        bool IVoiceState.IsSelfDeafened => false;
        bool IVoiceState.IsSelfMuted => false;
        bool IVoiceState.IsSuppressed => false;
        IVoiceChannel IVoiceState.VoiceChannel => null;
        string IVoiceState.VoiceSessionId => null;
    }
}
