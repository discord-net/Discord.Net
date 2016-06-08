using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    internal class CachedVoiceChannel : VoiceChannel, ICachedGuildChannel
    {
        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedGuild Guild => base.Guild as CachedGuild;

        public IReadOnlyCollection<IGuildUser> Members 
            => Guild.Members.Where(x => x.VoiceChannel.Id == Id).ToImmutableArray();

        public CachedVoiceChannel(CachedGuild guild, Model model)
            : base(guild, model)
        {
        }

        public override Task<IGuildUser> GetUser(ulong id) 
            => Task.FromResult(GetCachedUser(id));
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers() 
            => Task.FromResult(Members);
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers(int limit, int offset) 
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members.OrderBy(x => x.Id).Skip(offset).Take(limit).ToImmutableArray());
        public IGuildUser GetCachedUser(ulong id)
        {
            var user = Guild.GetCachedUser(id);
            if (user != null && user.VoiceChannel.Id == Id)
                return user;
            return null;
        }

        public CachedVoiceChannel Clone() => MemberwiseClone() as CachedVoiceChannel;

        ICachedChannel ICachedChannel.Clone() => Clone();
    }
}
