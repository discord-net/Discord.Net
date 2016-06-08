using System.Linq;
using System.Threading.Tasks;

namespace Discord.Extensions
{
    public static class DiscordClientExtensions
    {
        public static async Task<IVoiceRegion> GetOptimalVoiceRegion(this DiscordClient discord)
        {
            var regions = await discord.GetVoiceRegions().ConfigureAwait(false);
            return regions.FirstOrDefault(x => x.IsOptimal);
        }
    }
}
