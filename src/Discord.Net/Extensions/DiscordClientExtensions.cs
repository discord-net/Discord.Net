using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public static class DiscordClientExtensions
    {
        public static async Task<IPrivateChannel> GetPrivateChannelAsync(this IDiscordClient client, ulong id)
            => await client.GetChannelAsync(id).ConfigureAwait(false) as IPrivateChannel;

        public static async Task<IDMChannel> GetDMChannelAsync(this IDiscordClient client, ulong id)
            => await client.GetPrivateChannelAsync(id).ConfigureAwait(false) as IDMChannel;
        public static async Task<IEnumerable<IDMChannel>> GetDMChannelsAsync(this IDiscordClient client)
            => (await client.GetPrivateChannelsAsync().ConfigureAwait(false)).Select(x => x as IDMChannel).Where(x => x != null);

        public static async Task<IGroupChannel> GetGroupChannelAsync(this IDiscordClient client, ulong id)
            => await client.GetPrivateChannelAsync(id).ConfigureAwait(false) as IGroupChannel;
        public static async Task<IEnumerable<IGroupChannel>> GetGroupChannelsAsync(this IDiscordClient client)
            => (await client.GetPrivateChannelsAsync().ConfigureAwait(false)).Select(x => x as IGroupChannel).Where(x => x != null);

        public static async Task<IVoiceRegion> GetOptimalVoiceRegionAsync(this IDiscordClient discord)
        {
            var regions = await discord.GetVoiceRegionsAsync().ConfigureAwait(false);
            return regions.FirstOrDefault(x => x.IsOptimal);
        }
    }
}
