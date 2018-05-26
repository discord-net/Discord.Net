using System.Threading.Tasks;
using System.IO;

namespace Discord
{
    public static class UserExtensions
    {
        /// <summary>
        /// Sends a message to the user via DM.
        /// </summary>
        public static async Task<IUserMessage> SendMessageAsync(this IUser user,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null)
        {
            return await (await user.GetOrCreateDMChannelAsync().ConfigureAwait(false)).SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a file to the user via DM.
        /// </summary>
        public static async Task<IUserMessage> SendFileAsync(this IUser user,
            Stream stream,
            string filename,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null
            )
        {
            return await (await user.GetOrCreateDMChannelAsync().ConfigureAwait(false)).SendFileAsync(stream, filename, text, isTTS, embed, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a file to the user via DM.
        /// </summary>
        public static async Task<IUserMessage> SendFileAsync(this IUser user,
            string filePath,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null)
        {
            return await (await user.GetOrCreateDMChannelAsync().ConfigureAwait(false)).SendFileAsync(filePath, text, isTTS, embed, options).ConfigureAwait(false);
        }

        public static Task BanAsync(this IGuildUser user, int pruneDays = 0, string reason = null, RequestOptions options = null)
            => user.Guild.AddBanAsync(user, pruneDays, reason, options);
    }
}
