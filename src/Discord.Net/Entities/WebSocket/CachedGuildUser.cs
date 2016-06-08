using Model = Discord.API.GuildMember;

namespace Discord
{
    internal class CachedGuildUser : GuildUser, ICachedEntity<ulong>
    {
        public VoiceChannel VoiceChannel { get; private set; }

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;

        public CachedGuildUser(CachedGuild guild, CachedPublicUser user, Model model) 
            : base(guild, user, model)
        {
        }

        public CachedGuildUser Clone() => MemberwiseClone() as CachedGuildUser;
    }
}
