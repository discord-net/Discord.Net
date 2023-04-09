using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> An extension class for various Discord user objects. </summary>
    public static class UserExtensions
    {
        /// <summary>
        ///     Sends a message via DM.
        /// </summary>
        /// <remarks>
        ///     This method attempts to send a direct-message to the user.
        ///     <note type="warning">
        ///         <para>
        ///         Please note that this method <strong>will</strong> throw an <see cref="Discord.Net.HttpException"/>
        ///         if the user cannot receive DMs due to privacy reasons or if the user has the sender blocked.
        ///         </para>
        ///         <para>
        ///         You may want to consider catching for <see cref="Discord.Net.HttpException.DiscordCode"/> 
        ///         <c>50007</c> when using this method.
        ///         </para>
        ///     </note>
        /// </remarks>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <returns>
        ///     A task that represents the asynchronous send operation. The task result contains the sent message.
        /// </returns>
        public static async Task<IUserMessage> SendMessageAsync(this IUser user,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed[] embeds = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendMessageAsync(text, isTTS, embed, options, allowedMentions, components: components, embeds: embeds).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <example>
        ///     <para>The following example uploads a streamed image that will be called <c>b1nzy.jpg</c> embedded inside a
        ///     rich embed to the channel.</para>
        ///     <code language="cs">
        ///     await channel.SendFileAsync(b1nzyStream, "b1nzy.jpg",
        ///         embed: new EmbedBuilder {ImageUrl = "attachment://b1nzy.jpg"}.Build());
        ///     </code>
        /// </example>
        /// <remarks>
        ///     This method attempts to send an attachment as a direct-message to the user.
        ///     <note type="warning">
        ///         <para>
        ///         Please note that this method <strong>will</strong> throw an <see cref="Discord.Net.HttpException"/>
        ///         if the user cannot receive DMs due to privacy reasons or if the user has the sender blocked.
        ///         </para>
        ///         <para>
        ///         You may want to consider catching for <see cref="Discord.Net.HttpException.DiscordCode"/> 
        ///         <c>50007</c> when using this method.
        ///         </para>
        ///     </note>
        ///     <note>
        ///         If you wish to upload an image and have it embedded in a <see cref="Discord.EmbedType.Rich"/> embed,
        ///         you may upload the file and refer to the file with "attachment://filename.ext" in the
        ///         <see cref="Discord.EmbedBuilder.ImageUrl"/>. See the example section for its usage.
        ///     </note>
        /// </remarks>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="stream">The <see cref="Stream"/> of the file to be sent.</param>
        /// <param name="filename">The name of the attachment.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="components">The message component to be included with this message. Used for interactions.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public static async Task<IUserMessage> SendFileAsync(this IUser user,
            Stream stream,
            string filename,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null,
            MessageComponent components = null,
            Embed[] embeds = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFileAsync(stream, filename, text, isTTS, embed, options, components: components, embeds: embeds).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a file via DM with an optional caption.
        /// </summary>
        /// <example>
        ///     The following example uploads a local file called <c>wumpus.txt</c> along with the text 
        ///     <c>good discord boi</c> to the channel.
        ///     <code language="cs">
        ///     await channel.SendFileAsync("wumpus.txt", "good discord boi");
        ///     </code>
        /// 
        ///     The following example uploads a local image called <c>b1nzy.jpg</c> embedded inside a rich embed to the
        ///     channel.
        ///     <code language="cs">
        ///     await channel.SendFileAsync("b1nzy.jpg",
        ///         embed: new EmbedBuilder {ImageUrl = "attachment://b1nzy.jpg"}.Build());
        ///     </code>
        /// </example>
        /// <remarks>
        ///     This method attempts to send an attachment as a direct-message to the user.
        ///     <note type="warning">
        ///         <para>
        ///         Please note that this method <strong>will</strong> throw an <see cref="Discord.Net.HttpException"/>
        ///         if the user cannot receive DMs due to privacy reasons or if the user has the sender blocked.
        ///         </para>
        ///         <para>
        ///         You may want to consider catching for <see cref="Discord.Net.HttpException.DiscordCode"/> 
        ///         <c>50007</c> when using this method.
        ///         </para>
        ///     </note>
        ///     <note>
        ///         If you wish to upload an image and have it embedded in a <see cref="Discord.EmbedType.Rich"/> embed,
        ///         you may upload the file and refer to the file with "attachment://filename.ext" in the
        ///         <see cref="Discord.EmbedBuilder.ImageUrl"/>. See the example section for its usage.
        ///     </note>
        /// </remarks>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="filePath">The file path of the file.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="components">The message component to be included with this message. Used for interactions.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public static async Task<IUserMessage> SendFileAsync(this IUser user,
            string filePath,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null,
            MessageComponent components = null,
            Embed[] embeds = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFileAsync(filePath, text, isTTS, embed, options, components: components, embeds: embeds).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a file via DM with an optional caption.
        /// </summary>
        /// <remarks>
        ///     This method attempts to send an attachment as a direct-message to the user.
        ///     <note type="warning">
        ///         <para>
        ///         Please note that this method <strong>will</strong> throw an <see cref="Discord.Net.HttpException"/>
        ///         if the user cannot receive DMs due to privacy reasons or if the user has the sender blocked.
        ///         </para>
        ///         <para>
        ///         You may want to consider catching for <see cref="Discord.Net.HttpException.DiscordCode"/>
        ///         <c>50007</c> when using this method.
        ///         </para>
        ///     </note>
        ///     <note>
        ///         If you wish to upload an image and have it embedded in a <see cref="Discord.EmbedType.Rich"/> embed,
        ///         you may upload the file and refer to the file with "attachment://filename.ext" in the
        ///         <see cref="Discord.EmbedBuilder.ImageUrl"/>. See the example section for its usage.
        ///     </note>
        /// </remarks>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="attachment">The attachment containing the file and description.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="components">The message component to be included with this message. Used for interactions.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public static async Task<IUserMessage> SendFileAsync(this IUser user,
            FileAttachment attachment,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null,
            MessageComponent components = null,
            Embed[] embeds = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFileAsync(attachment, text, isTTS, embed, options, components: components, embeds: embeds).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a collection of files via DM.
        /// </summary>
        /// <remarks>
        ///     This method attempts to send an attachments as a direct-message to the user.
        ///     <note type="warning">
        ///         <para>
        ///         Please note that this method <strong>will</strong> throw an <see cref="Discord.Net.HttpException"/>
        ///         if the user cannot receive DMs due to privacy reasons or if the user has the sender blocked.
        ///         </para>
        ///         <para>
        ///         You may want to consider catching for <see cref="Discord.Net.HttpException.DiscordCode"/>
        ///         <c>50007</c> when using this method.
        ///         </para>
        ///     </note>
        ///     <note>
        ///         If you wish to upload an image and have it embedded in a <see cref="Discord.EmbedType.Rich"/> embed,
        ///         you may upload the file and refer to the file with "attachment://filename.ext" in the
        ///         <see cref="Discord.EmbedBuilder.ImageUrl"/>. See the example section for its usage.
        ///     </note>
        /// </remarks>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="attachments">A collection of attachments to upload.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="components">The message component to be included with this message. Used for interactions.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public static async Task<IUserMessage> SendFilesAsync(this IUser user,
            IEnumerable<FileAttachment> attachments,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null,
            MessageComponent components = null,
            Embed[] embeds = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFilesAsync(attachments, text, isTTS, embed, options, components: components, embeds: embeds).ConfigureAwait(false);
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
        ///     A task that represents the asynchronous operation for banning a user.
        /// </returns>
        public static Task BanAsync(this IGuildUser user, int pruneDays = 0, string reason = null, RequestOptions options = null)
            => user.Guild.AddBanAsync(user, pruneDays, reason, options);
    }
}
