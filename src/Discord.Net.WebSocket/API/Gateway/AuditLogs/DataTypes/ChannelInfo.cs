using Discord.API;
using System.Collections.Generic;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents information for a channel.
    /// </summary>
    public struct ChannelInfo
    {
        internal ChannelInfo(string name, string topic, int? rateLimit, bool? nsfw, int? bitrate, ChannelType? type, int? defaultArchiveDuration, ForumTag[] forumTags, IEmote defaultReactionEmoji)
        {
            Name = name;
            Topic = topic;
            SlowModeInterval = rateLimit;
            IsNsfw = nsfw;
            Bitrate = bitrate;
            ChannelType = type;
            DefaultArchiveDuration = defaultArchiveDuration;
            ForumTags = forumTags;
            DefaultReactionEmoji = defaultReactionEmoji;
        }

        /// <inheritdoc cref="IChannel.Name"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public string Name { get; }
        /// <inheritdoc cref="ITextChannel.Topic"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public string Topic { get; }
        /// <inheritdoc cref="ITextChannel.SlowModeInterval"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public int? SlowModeInterval { get; }
        /// <inheritdoc cref="ITextChannel.IsNsfw"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public bool? IsNsfw { get; }
        /// <inheritdoc cref="IVoiceChannel.Bitrate"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public int? Bitrate { get; }
        /// <summary>
        ///     Gets the type of this channel.
        /// </summary>
        /// <returns>
        ///     The channel type of this channel; <c>null</c> if not applicable.
        /// </returns>
        public ChannelType? ChannelType { get; }
        /// <inheritdoc cref="ITextChannel.DefaultArchiveDuration"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public int? DefaultArchiveDuration { get; }
        /// <inheritdoc cref="IForumChannel.Tags"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public IReadOnlyCollection<ForumTag> ForumTags { get; }
        /// <inheritdoc cref="IForumChannel.DefaultReactionEmoji"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public IEmote DefaultReactionEmoji { get; }


    }
}
