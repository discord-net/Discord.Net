using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IForumChannel : IGuildChannel, IMentionable
    {
        /// <summary>
        ///     Gets a value that indicates whether the channel is NSFW.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the channel has the NSFW flag enabled; otherwise <c>false</c>.
        /// </returns>
        bool IsNsfw { get; }

        /// <summary>
        ///     Gets the current topic for this text channel.
        /// </summary>
        /// <returns>
        ///     A string representing the topic set in the channel; <c>null</c> if none is set.
        /// </returns>
        string Topic { get; }

        /// <summary>
        ///     Gets the default archive duration for a newly created post.
        /// </summary>
        ThreadArchiveDuration DefaultAutoArchiveDuration { get; }

        /// <summary>
        ///     Gets a collection of tags inside of this forum channel.
        /// </summary>
        IReadOnlyCollection<ForumTag> Tags { get; }

        /// <summary>
        ///     Creates a new post (thread) within the forum.
        /// </summary>
        /// <param name="title">The title of the post.</param>
        /// <param name="archiveDuration">The archive duration of the post.</param>
        /// <param name="slowmode">The slowmode for the posts thread.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the message.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> is permitted.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation.
        /// </returns>
        Task<IThreadChannel> CreatePostAsync(string title, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null,
            string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None);

        /// <summary>
        ///     Creates a new post (thread) within the forum.
        /// </summary>
        /// <param name="title">The title of the post.</param>
        /// <param name="archiveDuration">The archive duration of the post.</param>
        /// <param name="slowmode">The slowmode for the posts thread.</param>
        /// <param name="filePath">The file path of the file.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich" /> <see cref="Embed" /> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="isSpoiler">Whether the message attachment should be hidden as a spoiler.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the file.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> is permitted.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation.
        /// </returns>
        Task<IThreadChannel> CreatePostWithFileAsync(string title, string filePath, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, bool isSpoiler = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None);

        /// <summary>
        ///     Creates a new post (thread) within the forum.
        /// </summary>
        /// <param name="title">The title of the post.</param>
        /// <param name="stream">The <see cref="Stream" /> of the file to be sent.</param>
        /// <param name="filename">The name of the attachment.</param>
        /// <param name="archiveDuration">The archive duration of the post.</param>
        /// <param name="slowmode">The slowmode for the posts thread.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="isSpoiler">Whether the message attachment should be hidden as a spoiler.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the file.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> is permitted.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation.
        /// </returns>
        public Task<IThreadChannel> CreatePostWithFileAsync(string title, Stream stream, string filename, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, bool isSpoiler = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null,MessageFlags flags = MessageFlags.None);

        /// <summary>
        ///     Creates a new post (thread) within the forum.
        /// </summary>
        /// <param name="title">The title of the post.</param>
        /// <param name="attachment">The attachment containing the file and description.</param>
        /// <param name="archiveDuration">The archive duration of the post.</param>
        /// <param name="slowmode">The slowmode for the posts thread.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the file.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> is permitted.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation.
        /// </returns>
        public Task<IThreadChannel> CreatePostWithFileAsync(string title, FileAttachment attachment, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None);

        /// <summary>
        ///     Creates a new post (thread) within the forum.
        /// </summary>
        /// <param name="title">The title of the post.</param>
        /// <param name="attachments">A collection of attachments to upload.</param>
        /// <param name="archiveDuration">The archive duration of the post.</param>
        /// <param name="slowmode">The slowmode for the posts thread.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the file.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> is permitted.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation.
        /// </returns>
        public Task<IThreadChannel> CreatePostWithFilesAsync(string title, IEnumerable<FileAttachment> attachments, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None);

        /// <summary>
        ///     Gets a collection of active threads within this forum channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of active threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of publicly archived threads within this forum channel.
        /// </summary>
        /// <param name="limit">The optional limit of how many to get.</param>
        /// <param name="before">The optional date to return threads created before this timestamp.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of publicly archived threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetPublicArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of privately archived threads within this forum channel.
        /// </summary>
        /// <remarks>
        ///     The bot requires the <see cref="GuildPermission.ManageThreads"/> permission in order to execute this request.
        /// </remarks>
        /// <param name="limit">The optional limit of how many to get.</param>
        /// <param name="before">The optional date to return threads created before this timestamp.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of privately archived threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of privately archived threads that the current bot has joined within this forum channel.
        /// </summary>
        /// <param name="limit">The optional limit of how many to get.</param>
        /// <param name="before">The optional date to return threads created before this timestamp.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of privately archived threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetJoinedPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null);
    }
}
