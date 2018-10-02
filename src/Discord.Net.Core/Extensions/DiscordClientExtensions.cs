using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> An extension class for the Discord client. </summary>
    public static class DiscordClientExtensions
    {
        /// <summary> Gets the private channel with the provided ID. </summary>
        public static async Task<IPrivateChannel> GetPrivateChannelAsync(this IDiscordClient client, ulong id)
            => await client.GetChannelAsync(id).ConfigureAwait(false) as IPrivateChannel;

        /// <summary> Gets the DM channel with the provided ID. </summary>
        public static async Task<IDMChannel> GetDMChannelAsync(this IDiscordClient client, ulong id)
            => await client.GetPrivateChannelAsync(id).ConfigureAwait(false) as IDMChannel;
        /// <summary> Gets all available DM channels for the client. </summary>
        public static async Task<IEnumerable<IDMChannel>> GetDMChannelsAsync(this IDiscordClient client)
            => (await client.GetPrivateChannelsAsync().ConfigureAwait(false)).OfType<IDMChannel>();

        /// <summary> Gets the group channel with the provided ID. </summary>
        public static async Task<IGroupChannel> GetGroupChannelAsync(this IDiscordClient client, ulong id)
            => await client.GetPrivateChannelAsync(id).ConfigureAwait(false) as IGroupChannel;
        /// <summary> Gets all available group channels for the client. </summary>
        public static async Task<IEnumerable<IGroupChannel>> GetGroupChannelsAsync(this IDiscordClient client)
            => (await client.GetPrivateChannelsAsync().ConfigureAwait(false)).OfType<IGroupChannel>();

        /// <summary> Gets the most optimal voice region for the client. </summary>
        public static async Task<IVoiceRegion> GetOptimalVoiceRegionAsync(this IDiscordClient discord)
        {
            var regions = await discord.GetVoiceRegionsAsync().ConfigureAwait(false);
            return regions.FirstOrDefault(x => x.IsOptimal);
        }
    }
}
