using System.Threading.Tasks;

namespace Discord.Extensions
{
    public static class GuildExtensions
    {
        public static async Task<ITextChannel> GetTextChannelAsync(this IGuild guild, ulong id)
            => await guild.GetChannelAsync(id).ConfigureAwait(false) as ITextChannel;
        public static async Task<IVoiceChannel> GetVoiceChannelAsync(this IGuild guild, ulong id)
            => await guild.GetChannelAsync(id).ConfigureAwait(false) as IVoiceChannel;
    }
}
