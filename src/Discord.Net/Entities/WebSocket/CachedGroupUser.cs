using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class CachedGroupUser : GroupUser, ICachedUser
    {
        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedGroupChannel Channel => base.Channel as CachedGroupChannel;
        public new CachedGlobalUser User => base.User as CachedGlobalUser;
        public Presence Presence => User.Presence; //{ get; private set; }

        public override Game Game => Presence.Game;
        public override UserStatus Status => Presence.Status;

        public VoiceState? VoiceState => Channel.GetVoiceState(Id);
        public bool IsSelfDeafened => VoiceState?.IsSelfDeafened ?? false;
        public bool IsSelfMuted => VoiceState?.IsSelfMuted ?? false;
        public bool IsSuppressed => VoiceState?.IsSuppressed ?? false;
        public CachedVoiceChannel VoiceChannel => VoiceState?.VoiceChannel;

        public CachedGroupUser(CachedGroupChannel channel, CachedGlobalUser user)
            : base(channel, user)
        {
        }

        public CachedGroupUser Clone() => MemberwiseClone() as CachedGroupUser;
        ICachedUser ICachedUser.Clone() => Clone();

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";
    }
}
