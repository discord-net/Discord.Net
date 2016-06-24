using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    //TODO: C#7 Candidate for record type
    internal struct Presence : IPresence
    {
        public Game Game { get; }
        public UserStatus Status { get; }

        public Presence(Game game, UserStatus status)
        {
            Game = game;
            Status = status;
        }

        public Presence Clone() => this;
    }

    internal class CachedGuildUser : GuildUser, ICachedUser
    {
        public Presence Presence { get; private set; }

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedGuild Guild => base.Guild as CachedGuild;
        public new CachedGlobalUser User => base.User as CachedGlobalUser;

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
        }
        public CachedGuildUser(CachedGuild guild, CachedGlobalUser user, PresenceModel model)
            : base(guild, user, model)
        {
        }

        public override void Update(PresenceModel model, UpdateSource source)
        {
            base.Update(model, source);

            var game = model.Game != null ? new Game(model.Game) : null;
            Presence = new Presence(game, model.Status);
        }

        public CachedGuildUser Clone() => MemberwiseClone() as CachedGuildUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
