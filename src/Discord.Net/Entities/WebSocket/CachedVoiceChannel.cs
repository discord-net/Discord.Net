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
            => Guild.VoiceStates.Where(x => x.Value.VoiceChannel.Id == Id).Select(x => Guild.GetUser(x.Key)).ToImmutableArray();

        public CachedVoiceChannel(CachedGuild guild, Model model)
            : base(guild, model)
        {
        }

        public override Task<IGuildUser> GetUserAsync(ulong id) 
            => Task.FromResult(GetUser(id));
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync() 
            => Task.FromResult(Members);
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(int limit, int offset) 
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members.OrderBy(x => x.Id).Skip(offset).Take(limit).ToImmutableArray());
        public IGuildUser GetUser(ulong id)
        {
            var user = Guild.GetUser(id);
            if (user != null && user.VoiceChannel.Id == Id)
                return user;
            return null;
        }

        public CachedVoiceChannel Clone() => MemberwiseClone() as CachedVoiceChannel;

        ICachedChannel ICachedChannel.Clone() => Clone();
    }
}
