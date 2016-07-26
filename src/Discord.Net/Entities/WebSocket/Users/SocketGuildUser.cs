using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class SocketGuildUser : GuildUser, ISocketUser
    {
        internal override bool IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new SocketGuild Guild => base.Guild as SocketGuild;
        public new SocketGlobalUser User => base.User as SocketGlobalUser;
        public Presence Presence => User.Presence; //{ get; private set; }

        public override Game Game => Presence.Game;
        public override UserStatus Status => Presence.Status;

        public VoiceState? VoiceState => Guild.GetVoiceState(Id);
        public bool IsSelfDeafened => VoiceState?.IsSelfDeafened ?? false;
        public bool IsSelfMuted => VoiceState?.IsSelfMuted ?? false;
        public bool IsSuppressed => VoiceState?.IsSuppressed ?? false;
        public SocketVoiceChannel VoiceChannel => VoiceState?.VoiceChannel;

        public SocketGuildUser(SocketGuild guild, SocketGlobalUser user, Model model) 
            : base(guild, user, model)
        {
            //Presence = new Presence(null, UserStatus.Offline);
        }
        public SocketGuildUser(SocketGuild guild, SocketGlobalUser user, PresenceModel model)
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

        public SocketGuildUser Clone() => MemberwiseClone() as SocketGuildUser;
        ISocketUser ISocketUser.Clone() => Clone();
    }
}
