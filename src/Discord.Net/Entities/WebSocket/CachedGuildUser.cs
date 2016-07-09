using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class CachedGuildUser : GuildUser, ICachedUser
    {
        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedGuild Guild => base.Guild as CachedGuild;
        public new CachedGlobalUser User => base.User as CachedGlobalUser;
        public Presence Presence => User.Presence; //{ get; private set; }

        public override Game Game => Presence.Game;
        public override UserStatus Status => Presence.Status;

        public VoiceState? VoiceState => Guild.GetVoiceState(Id);
        public bool IsSelfDeafened => VoiceState?.IsSelfDeafened ?? false;
        public bool IsSelfMuted => VoiceState?.IsSelfMuted ?? false;
        public bool IsSuppressed => VoiceState?.IsSuppressed ?? false;
        public CachedVoiceChannel VoiceChannel => VoiceState?.VoiceChannel;

        public CachedGuildUser(CachedGuild guild, CachedGlobalUser user, Model model) 
            : base(guild, user, model)
        {
            //Presence = new Presence(null, UserStatus.Offline);
        }
        public CachedGuildUser(CachedGuild guild, CachedGlobalUser user, PresenceModel model)
            : base(guild, user, model)
        {
        }

        public override void Update(PresenceModel model, UpdateSource source)
        {
            base.Update(model, source);

            var game = model.Game != null ? new Game(model.Game) : null;
            //Presence = new Presence(game, model.Status);

            User.Update(model, source);
        }

        public CachedGuildUser Clone() => MemberwiseClone() as CachedGuildUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
