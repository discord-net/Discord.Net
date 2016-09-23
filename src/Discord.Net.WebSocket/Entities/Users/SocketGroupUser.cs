using Discord.Rest;
using System.Diagnostics;

namespace Discord.WebSocket
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class SocketGroupUser : GroupUser, ISocketUser
    {
        internal override bool IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new SocketGroupChannel Channel => base.Channel as SocketGroupChannel;
        public new SocketGlobalUser User => base.User as SocketGlobalUser;
        public Presence Presence => User.Presence; //{ get; private set; }

        public override Game Game => Presence.Game;
        public override UserStatus Status => Presence.Status;

        public VoiceState? VoiceState => Channel.GetVoiceState(Id);
        public bool IsSelfDeafened => VoiceState?.IsSelfDeafened ?? false;
        public bool IsSelfMuted => VoiceState?.IsSelfMuted ?? false;
        public bool IsSuppressed => VoiceState?.IsSuppressed ?? false;
        public SocketVoiceChannel VoiceChannel => VoiceState?.VoiceChannel;

        public SocketGroupUser(SocketGroupChannel channel, SocketGlobalUser user)
            : base(channel, user)
        {
        }

        public SocketGroupUser Clone() => MemberwiseClone() as SocketGroupUser;
        ISocketUser ISocketUser.Clone() => Clone();

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";
    }
}
