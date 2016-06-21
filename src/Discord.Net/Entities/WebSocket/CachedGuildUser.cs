using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class CachedGuildUser : GuildUser, ICachedUser
    {
        private Game _game;
        private UserStatus _status;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedGuild Guild => base.Guild as CachedGuild;
        public new CachedGlobalUser User => base.User as CachedGlobalUser;

        public override Game Game => _game;
        public override UserStatus Status => _status;

        public VoiceState? VoiceState => Guild.GetVoiceState(Id);
        public bool IsSelfDeafened => VoiceState?.IsSelfDeafened ?? false;
        public bool IsSelfMuted => VoiceState?.IsSelfMuted ?? false;
        public bool IsSuppressed => VoiceState?.IsSuppressed ?? false;
        public CachedVoiceChannel VoiceChannel => VoiceState?.VoiceChannel;

        public CachedGuildUser(CachedGuild guild, CachedGlobalUser user, Model model) 
            : base(guild, user, model)
        {
        }
        public CachedGuildUser(CachedGuild guild, CachedGlobalUser user, PresenceModel model)
            : base(guild, user, model)
        {
        }

        public override void Update(PresenceModel model, UpdateSource source)
        {
            base.Update(model, source);

            _status = model.Status;
            _game = model.Game != null ? new Game(model.Game) : (Game)null;
        }

        public CachedGuildUser Clone() => MemberwiseClone() as CachedGuildUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
