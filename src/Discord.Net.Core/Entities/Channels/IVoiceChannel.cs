using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic voice channel in a guild.
    /// </summary>
    public interface IVoiceChannel : IMessageChannel, INestedChannel, IAudioChannel, IMentionable
    {
        /// <summary>
        ///     Gets the bit-rate that the clients in this voice channel are requested to use.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the bit-rate (bps) that this voice channel defines and requests the
        ///     client(s) to use.
        /// </returns>
        int Bitrate { get; }
        /// <summary>
        ///     Gets the max number of users allowed to be connected to this channel at once.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the maximum number of users that are allowed to be connected to this
        ///     channel at once; <c>null</c> if a limit is not set.
        /// </returns>
        int? UserLimit { get; }

        /// <summary>
        ///     Gets the video quality mode for this channel.
        /// </summary>
        VideoQualityMode VideoQualityMode { get; }

        /// <summary>
        ///     Bulk-deletes multiple messages.
        /// </summary>
        /// <example>
        ///     <para>The following example gets 250 messages from the channel and deletes them.</para>
        ///     <code language="cs">
        ///     var messages = await voiceChannel.GetMessagesAsync(250).FlattenAsync();
        ///     await voiceChannel.DeleteMessagesAsync(messages);
        ///     </code>
        /// </example>
        /// <remarks>
        ///     This method attempts to remove the messages specified in bulk.
        ///     <note type="important">
        ///         Due to the limitation set by Discord, this method can only remove messages that are posted within 14 days!
        ///     </note>
        /// </remarks>
        /// <param name="messages">The messages to be bulk-deleted.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous bulk-removal operation.
        /// </returns>
        Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null);
        /// <summary>
        ///     Bulk-deletes multiple messages.
        /// </summary>
        /// <remarks>
        ///     This method attempts to remove the messages specified in bulk.
        ///     <note type="important">
        ///         Due to the limitation set by Discord, this method can only remove messages that are posted within 14 days!
        ///     </note>
        /// </remarks>
        /// <param name="messageIds">The snowflake identifier of the messages to be bulk-deleted.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous bulk-removal operation.
        /// </returns>
        Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null);

        /// <summary>
        ///     Modifies this voice channel.
        /// </summary>
        /// <param name="func">The properties to modify the channel with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        /// <seealso cref="VoiceChannelProperties"/>
        Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null);
    }
}
