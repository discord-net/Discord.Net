using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic channel that can send and receive messages.
    /// </summary>
    public interface IMessageChannel : IChannel
    {
        /// <summary>
        ///     Sends a message to this message channel.
        /// </summary>
        /// <example>
        ///     <para>The following example sends a message with the current system time in RFC 1123 format to the channel and
        ///     deletes itself after 5 seconds.</para>
        ///     <code language="cs" region="SendMessageAsync"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        /// </example>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <see langword="null" />, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="messageReference">The message references to be included. Used to reply to specific messages.</param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the message.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/>
        /// and <see cref="MessageFlags.SuppressNotification"/> is permitted.</param>
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        Task<IUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null,
            AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null);

        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <example>
        ///     <para>The following example uploads a local file called <c>wumpus.txt</c> along with the text
        ///     <c>good discord boi</c> to the channel.</para>
        ///     <code language="cs" region="SendFileAsync.FilePath"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        ///     <para>The following example uploads a local image called <c>b1nzy.jpg</c> embedded inside a rich embed to the
        ///     channel.</para>
        ///     <code language="cs" region="SendFileAsync.FilePath.EmbeddedImage"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        /// </example>
        /// <remarks>
        ///     This method sends a file as if you are uploading an attachment directly from your Discord client.
        ///     <note>
        ///         If you wish to upload an image and have it embedded in a <see cref="Discord.EmbedType.Rich"/> embed,
        ///         you may upload the file and refer to the file with "attachment://filename.ext" in the
        ///         <see cref="Discord.EmbedBuilder.ImageUrl"/>. See the example section for its usage.
        ///     </note>
        /// </remarks>
        /// <param name="filePath">The file path of the file.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich" /> <see cref="Embed" /> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="isSpoiler">Whether the message attachment should be hidden as a spoiler.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <see langword="null" />, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="messageReference">The message references to be included. Used to reply to specific messages.</param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the file.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.SuppressNotification"/> is permitted.</param>
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        Task<IUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null,
            bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null);

        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <example>
        ///     <para>The following example uploads a streamed image that will be called <c>b1nzy.jpg</c> embedded inside a
        ///     rich embed to the channel.</para>
        ///     <code language="cs" region="SendFileAsync.FileStream.EmbeddedImage"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        /// </example>
        /// <remarks>
        ///     This method sends a file as if you are uploading an attachment directly from your Discord client.
        ///     <note>
        ///         If you wish to upload an image and have it embedded in a <see cref="Discord.EmbedType.Rich"/> embed,
        ///         you may upload the file and refer to the file with "attachment://filename.ext" in the
        ///         <see cref="Discord.EmbedBuilder.ImageUrl"/>. See the example section for its usage.
        ///     </note>
        /// </remarks>
        /// <param name="stream">The <see cref="Stream" /> of the file to be sent.</param>
        /// <param name="filename">The name of the attachment.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="isSpoiler">Whether the message attachment should be hidden as a spoiler.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <see langword="null" />, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="messageReference">The message references to be included. Used to reply to specific messages.</param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the file.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.SuppressNotification"/> is permitted.</param>
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        Task<IUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null,
            bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null);

        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <remarks>
        ///     This method sends a file as if you are uploading an attachment directly from your Discord client.
        ///     <note>
        ///         If you wish to upload an image and have it embedded in a <see cref="Discord.EmbedType.Rich"/> embed,
        ///         you may upload the file and refer to the file with "attachment://filename.ext" in the
        ///         <see cref="Discord.EmbedBuilder.ImageUrl"/>. See the example section for its usage.
        ///     </note>
        /// </remarks>
        /// <param name="attachment">The attachment containing the file and description.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <see langword="null" />, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="messageReference">The message references to be included. Used to reply to specific messages.</param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the file.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.SuppressNotification"/> is permitted.</param>
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        Task<IUserMessage> SendFileAsync(FileAttachment attachment, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null,
            AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null);
        /// <summary>
        ///     Sends a collection of files to this message channel.
        /// </summary>
        /// <remarks>
        ///     This method sends files as if you are uploading attachments directly from your Discord client.
        ///     <note>
        ///         If you wish to upload an image and have it embedded in a <see cref="Discord.EmbedType.Rich"/> embed,
        ///         you may upload the file and refer to the file with "attachment://filename.ext" in the
        ///         <see cref="Discord.EmbedBuilder.ImageUrl"/>. See the example section for its usage.
        ///     </note>
        /// </remarks>
        /// <param name="attachments">A collection of attachments to upload.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <see langword="null" />, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="messageReference">The message references to be included. Used to reply to specific messages.</param>
        /// <param name="components">The message components to be included with this message. Used for interactions.</param>
        /// <param name="stickers">A collection of stickers to send with the file.</param>
        /// <param name="embeds">A array of <see cref="Embed"/>s to send with this response. Max 10.</param>
        /// <param name="flags">A message flag to be applied to the sent message, only <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.SuppressNotification"/> is permitted.</param>
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        Task<IUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, bool isTTS = false, Embed embed = null,
            RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null);

        /// <summary>
        ///     Gets a message from this message channel.
        /// </summary>
        /// <param name="id">The snowflake identifier of the message.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
        ///     the retrieved message; <see langword="null" /> if no message is found with the specified identifier.
        /// </returns>
        Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Gets the last N messages from this message channel.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
        ///         collection.
        ///     </note>
        ///     <note type="warning">
        ///         Do not fetch too many messages at once! This may cause unwanted preemptive rate limit or even actual
        ///         rate limit, causing your bot to freeze!
        ///     </note>
        ///     This method will attempt to fetch the number of messages specified under <paramref name="limit"/>. The
        ///     library will attempt to split up the requests according to your <paramref name="limit"/> and
        ///     <see cref="DiscordConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
        ///     and the <see cref="Discord.DiscordConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
        ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
        ///     of flattening.
        /// </remarks>
        /// <example>
        ///     <para>The following example downloads 300 messages and gets messages that belong to the user
        ///     <c>53905483156684800</c>.</para>
        ///     <code language="cs" region="GetMessagesAsync.FromLimit.Standard"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        /// </example>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from
        /// cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch,
            CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of messages in this channel.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
        ///         collection.
        ///     </note>
        ///     <note type="warning">
        ///         Do not fetch too many messages at once! This may cause unwanted preemptive rate limit or even actual
        ///         rate limit, causing your bot to freeze!
        ///     </note>
        ///     This method will attempt to fetch the number of messages specified under <paramref name="limit"/> around
        ///     the message <paramref name="fromMessageId"/> depending on the <paramref name="dir"/>. The library will
        ///     attempt to split up the requests according to your <paramref name="limit"/> and
        ///     <see cref="DiscordConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
        ///     and the <see cref="Discord.DiscordConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
        ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
        ///     of flattening.
        /// </remarks>
        /// <example>
        ///     <para>The following example gets 5 message prior to the message identifier <c>442012544660537354</c>.</para>
        ///     <code language="cs" region="GetMessagesAsync.FromId.FromMessage"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        ///     <para>The following example attempts to retrieve <c>messageCount</c> number of messages from the
        ///     beginning of the channel and prints them to the console.</para>
        ///     <code language="cs" region="GetMessagesAsync.FromId.BeginningMessages"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        /// </example>
        /// <param name="fromMessageId">The ID of the starting message to get the messages from.</param>
        /// <param name="dir">The direction of the messages to be gotten from.</param>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from
        /// cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch,
            CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of messages in this channel.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
        ///         collection.
        ///     </note>
        ///     <note type="warning">
        ///         Do not fetch too many messages at once! This may cause unwanted preemptive rate limit or even actual
        ///         rate limit, causing your bot to freeze!
        ///     </note>
        ///     This method will attempt to fetch the number of messages specified under <paramref name="limit"/> around
        ///     the message <paramref name="fromMessage"/> depending on the <paramref name="dir"/>. The library will
        ///     attempt to split up the requests according to your <paramref name="limit"/> and
        ///     <see cref="DiscordConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
        ///     and the <see cref="Discord.DiscordConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
        ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
        ///     of flattening.
        /// </remarks>
        /// <example>
        ///     <para>The following example gets 5 message prior to a specific message, <c>oldMessage</c>.</para>
        ///     <code language="cs" region="GetMessagesAsync.FromMessage"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        /// </example>
        /// <param name="fromMessage">The starting message to get the messages from.</param>
        /// <param name="dir">The direction of the messages to be gotten from.</param>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from
        /// cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch,
            CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of pinned messages in this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation for retrieving pinned messages in this channel.
        ///     The task result contains a collection of messages found in the pinned messages.
        /// </returns>
        Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions options = null);

        /// <summary>
        ///     Deletes a message.
        /// </summary>
        /// <param name="messageId">The snowflake identifier of the message that would be removed.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation.
        /// </returns>
        Task DeleteMessageAsync(ulong messageId, RequestOptions options = null);
        /// <summary> Deletes a message based on the provided message in this channel. </summary>
        /// <param name="message">The message that would be removed.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation.
        /// </returns>
        Task DeleteMessageAsync(IMessage message, RequestOptions options = null);

        /// <summary>
        ///     Modifies a message.
        /// </summary>
        /// <remarks>
        ///     This method modifies this message with the specified properties. To see an example of this
        ///     method and what properties are available, please refer to <see cref="MessageProperties"/>.
        /// </remarks>
        /// <param name="messageId">The snowflake identifier of the message that would be changed.</param>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task<IUserMessage> ModifyMessageAsync(ulong messageId, Action<MessageProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Broadcasts the "user is typing" message to all users in this channel, lasting 10 seconds.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation that triggers the broadcast.
        /// </returns>
        Task TriggerTypingAsync(RequestOptions options = null);
        /// <summary>
        ///     Continuously broadcasts the "user is typing" message to all users in this channel until the returned
        ///     object is disposed.
        /// </summary>
        /// <example>
        ///     <para>The following example keeps the client in the typing state until <c>LongRunningAsync</c> has finished.</para>
        ///     <code language="cs" region="EnterTypingState"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Channels/IMessageChannel.Examples.cs" />
        /// </example>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A disposable object that, upon its disposal, will stop the client from broadcasting its typing state in
        ///     this channel.
        /// </returns>
        IDisposable EnterTypingState(RequestOptions options = null);
    }
}
