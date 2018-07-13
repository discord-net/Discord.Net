using System;
using System.Threading.Tasks;
using System.IO;

namespace Discord
{
    /// <summary> An extension class for various Discord user objects. </summary>
    public static class UserExtensions
    {
        /// <summary>
        ///     Sends a message via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchrnonous send operation. The task result contains the sent message.
        /// </returns>
        public static async Task<IUserMessage> SendMessageAsync(this IUser user,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null)
        {
            return await (await user.GetOrCreateDMChannelAsync().ConfigureAwait(false)).SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="stream">The <see cref="Stream"/> of the file to be sent.</param>
        /// <param name="filename">The name of the attachment.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <remarks>
        ///     If you wish to upload an image and have it embedded in a <see cref="EmbedType.Rich"/> embed, you may
        ///     upload the file and refer to the file with "attachment://filename.ext" in the 
        ///     <see cref="Discord.EmbedBuilder.ImageUrl"/>.
        /// </remarks>
        /// <returns>
        ///     A task that represents the asynchrnonous send operation. The task result contains the sent message.
        /// </returns>
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
        ///     Sends a file via DM with an optional caption.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="filePath">The file path of the file.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <remarks>
        ///     If you wish to upload an image and have it embedded in a <see cref="EmbedType.Rich"/> embed, you may
        ///     upload the file and refer to the file with "attachment://filename.ext" in the 
        ///     <see cref="Discord.EmbedBuilder.ImageUrl"/>.
        /// </remarks>
        /// <returns>
        ///     A task that represents the asynchrnonous send operation. The task result contains the sent message.
        /// </returns>
        public static async Task<IUserMessage> SendFileAsync(this IUser user,
            string filePath,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null)
        {
            return await (await user.GetOrCreateDMChannelAsync().ConfigureAwait(false)).SendFileAsync(filePath, text, isTTS, embed, options).ConfigureAwait(false);
        }

        /// <summary>
        ///     Bans the user from the guild and optionally prunes their recent messages.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="pruneDays">The number of days to remove messages from this <paramref name="user"/> for - must be between [0, 7]</param>
        /// <param name="reason">The reason of the ban to be written in the audit log.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentException"><paramref name="pruneDays" /> is not between 0 to 7.</exception>
        /// <returns>
        ///     A task that represents the asynchrnous operation for banning a user.
        /// </returns>
        public static Task BanAsync(this IGuildUser user, int pruneDays = 0, string reason = null, RequestOptions options = null)
            => user.Guild.AddBanAsync(user, pruneDays, reason, options);
    }
}
