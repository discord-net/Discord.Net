using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public static class GuildExtensions
    {
        public static async Task<ITextChannel> GetTextChannelAsync(this IGuild guild, ulong id)
            => await guild.GetChannelAsync(id).ConfigureAwait(false) as ITextChannel;
        public static async Task<IEnumerable<ITextChannel>> GetTextChannelsAsync(this IGuild guild)
            => (await guild.GetChannelsAsync().ConfigureAwait(false)).Select(x => x as ITextChannel).Where(x => x != null);

        public static async Task<IVoiceChannel> GetVoiceChannelAsync(this IGuild guild, ulong id)
            => await guild.GetChannelAsync(id).ConfigureAwait(false) as IVoiceChannel;
        public static async Task<IEnumerable<IVoiceChannel>> GetVoiceChannelsAsync(this IGuild guild)
            => (await guild.GetChannelsAsync().ConfigureAwait(false)).Select(x => x as IVoiceChannel).Where(x => x != null);

        public static async Task<IVoiceChannel> GetAFKChannelAsync(this IGuild guild)
        {
            var afkId = guild.AFKChannelId;
            if (afkId.HasValue)
                return await guild.GetChannelAsync(afkId.Value).ConfigureAwait(false) as IVoiceChannel;
            return null;
        }
        public static async Task<ITextChannel> GetDefaultChannelAsync(this IGuild guild)
            => await guild.GetChannelAsync(guild.DefaultChannelId).ConfigureAwait(false) as ITextChannel;
        public static async Task<IVoiceChannel> GetEmbedChannelAsync(this IGuild guild)
        {
            var embedId = guild.EmbedChannelId;
            if (embedId.HasValue) 
                return await guild.GetChannelAsync(embedId.Value).ConfigureAwait(false) as IVoiceChannel;
            return null;
        }
        public static async Task<IGuildUser> GetOwnerAsync(this IGuild guild)
            => await guild.GetUserAsync(guild.OwnerId).ConfigureAwait(false);
    }
}
