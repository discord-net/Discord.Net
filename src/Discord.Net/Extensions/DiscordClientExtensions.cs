using Discord.Rest;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public static class DiscordClientExtensions
    {
        public static async Task<IVoiceRegion> GetOptimalVoiceRegionAsync(this DiscordRestClient discord)
        {
            var regions = await discord.GetVoiceRegionsAsync().ConfigureAwait(false);
            return regions.FirstOrDefault(x => x.IsOptimal);
        }
    }
}
