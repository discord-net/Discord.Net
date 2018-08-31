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
        ///     The following example sends a message with the current system time in RFC 1123 format to the channel and
        ///     deletes itself after 5 seconds.
        ///     <code language="cs">
        ///     var message = await channel.SendMessageAsync(DateTimeOffset.UtcNow.ToString("R"));
        ///     await Task.Delay(TimeSpan.FromSeconds(5))
        ///         .ContinueWith(x => message.DeleteAsync());
        ///     </code>
        /// </example>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        Task<IUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);
        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
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
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        Task<IUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);
        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <example>
        ///     The following example uploads a streamed image that will be called <c>b1nzy.jpg</c> embedded inside a
        ///     rich embed to the channel.
        ///     <code language="cs">
        ///     await channel.SendFileAsync(b1nzyStream, "b1nzy.jpg",
        ///         embed: new EmbedBuilder {ImageUrl = "attachment://b1nzy.jpg"}.Build());
        ///     </code>
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
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        Task<IUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);

        /// <summary>
        ///     Gets a message from this message channel.
        /// </summary>
        /// <param name="id">The snowflake identifier of the message.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
        ///     the retrieved message; <c>null</c> if no message is found with the specified identifier.
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
        ///     The following example downloads 300 messages and gets messages that belong to the user 
        ///     <c>53905483156684800</c>.
        ///     <code lang="cs">
        ///     var messages = await messageChannel.GetMessagesAsync(300).FlattenAsync();
        ///     var userMessages = messages.Where(x =&gt; x.Author.Id == 53905483156684800);
        ///     </code>
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
        ///     The following example gets 5 message prior to the message identifier <c>442012544660537354</c>.
        ///     <code lang="cs">
        ///     var messages = await channel.GetMessagesAsync(442012544660537354, Direction.Before, 5).FlattenAsync();
        ///     </code>
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
        ///     The following example gets 5 message prior to a specific message, <c>oldMessage</c>.
        ///     <code lang="cs">
        ///     var messages = await channel.GetMessagesAsync(oldMessage, Direction.Before, 5).FlattenAsync();
        ///     </code>
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
        ///     The following example keeps the client in the typing state until <c>LongRunningAsync</c> has finished.
        ///     <code lang="cs">
        ///     using (messageChannel.EnterTypingState())
        ///     {
        ///         await LongRunningAsync();
        ///     }
        ///     </code>
        /// </example>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A disposable object that, upon its disposal, will stop the client from broadcasting its typing state in
        ///     this channel.
        /// </returns>
        IDisposable EnterTypingState(RequestOptions options = null);
    }
}
