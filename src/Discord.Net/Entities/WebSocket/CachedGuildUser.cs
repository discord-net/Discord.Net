using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class CachedGuildUser : GuildUser, ICachedUser
    {
        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedGuild Guild => base.Guild as CachedGuild;
        public new CachedPublicUser User => base.User as CachedPublicUser;

        public Presence? Presence => Guild.GetPresence(Id);
        public override Game? Game => Presence?.Game;
        public override UserStatus Status => Presence?.Status ?? UserStatus.Offline;

        public VoiceState? VoiceState => Guild.GetVoiceState(Id);
        public bool IsSelfDeafened => VoiceState?.IsSelfDeafened ?? false;
        public bool IsSelfMuted => VoiceState?.IsSelfMuted ?? false;
        public bool IsSuppressed => VoiceState?.IsSuppressed ?? false;
        public CachedVoiceChannel VoiceChannel => VoiceState?.VoiceChannel;

        public CachedGuildUser(CachedGuild guild, CachedPublicUser user, Model model) 
            : base(guild, user, model)
        {
        }
        public CachedGuildUser(CachedGuild guild, CachedPublicUser user, PresenceModel model)
            : base(guild, user, model)
        {
        }

        public CachedGuildUser Clone() => MemberwiseClone() as CachedGuildUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
