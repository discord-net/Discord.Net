using System.Threading.Tasks;

namespace Discord.Extensions
{
    public static class GuildExtensions
    {
        public static async Task<ITextChannel> GetTextChannel(this IGuild guild, ulong id)
            => await guild.GetChannel(id).ConfigureAwait(false) as ITextChannel;
        public static async Task<IVoiceChannel> GetVoiceChannel(this IGuild guild, ulong id)
            => await guild.GetChannel(id).ConfigureAwait(false) as IVoiceChannel;
    }
}
