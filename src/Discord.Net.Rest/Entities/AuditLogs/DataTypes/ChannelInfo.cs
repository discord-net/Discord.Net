using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.AuditLogs.ChannelInfoAuditLogModel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents information for a channel.
    /// </summary>
    public struct ChannelInfo
    {
        internal ChannelInfo(Model model)
        {
            Name = model.Name;
            Topic = model.Topic;
            IsNsfw = model.IsNsfw;
            Bitrate = model.Bitrate;
            DefaultArchiveDuration = model.DefaultArchiveDuration;
            ChannelType = model.Type;
            SlowModeInterval = model.RateLimitPerUser;

            ForumTags = model.AvailableTags?.Select(
                x => new ForumTag(x.Id,
                    x.Name,
                    x.EmojiId.GetValueOrDefault(null),
                    x.EmojiName.GetValueOrDefault(null),
                    x.Moderated)).ToImmutableArray();

            if (model.DefaultEmoji is not null)
            {
                if (model.DefaultEmoji.EmojiId.HasValue && model.DefaultEmoji.EmojiId.Value != 0)
                    DefaultReactionEmoji = new Emote(model.DefaultEmoji.EmojiId.GetValueOrDefault(), null, false);
                else if (model.DefaultEmoji.EmojiName.IsSpecified)
                    DefaultReactionEmoji = new Emoji(model.DefaultEmoji.EmojiName.Value);
                else
                    DefaultReactionEmoji = null;
            }
            else
                DefaultReactionEmoji = null;
            AutoArchiveDuration = model.AutoArchiveDuration;
            DefaultSlowModeInterval = model.DefaultThreadRateLimitPerUser;

            VideoQualityMode = model.VideoQualityMode;
            RtcRegion = model.Region;
            Flags = model.Flags;
            UserLimit = model.UserLimit;
        }

        /// <summary>
        ///     Gets the name of this channel.
        /// </summary>
        /// <returns>
        ///     A string containing the name of this channel.
        /// </returns>
        public string Name { get; }
        /// <summary>
        ///     Gets the topic of this channel.
        /// </summary>
        /// <returns>
        ///     A string containing the topic of this channel, if any.
        /// </returns>
        public string Topic { get; }
        /// <summary>
        ///     Gets the current slow-mode delay of this channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the time in seconds required before the user can send another
        ///     message; <c>0</c> if disabled.
        ///     <see langword="null" /> if this is not mentioned in this entry.
        /// </returns>
        public int? SlowModeInterval { get; }
        /// <summary>
        ///     Gets the value that indicates whether this channel is NSFW.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if this channel has the NSFW flag enabled; otherwise <see langword="false" />.
        ///     <see langword="null" /> if this is not mentioned in this entry.
        /// </returns>
        public bool? IsNsfw { get; }
        /// <summary>
        ///     Gets the bit-rate of this channel if applicable.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the bit-rate set for the voice channel;
        ///     <see langword="null" /> if this is not mentioned in this entry.
        /// </returns>
        public int? Bitrate { get; }
        /// <summary>
        ///     Gets the type of this channel.
        /// </summary>
        /// <returns>
        ///     The channel type of this channel; <see langword="null" /> if not applicable.
        /// </returns>
        public ChannelType? ChannelType { get; }

        /// <inheritdoc cref="ITextChannel.DefaultArchiveDuration"/>
        /// <remarks>
        ///     <see langword="null" /> if the value was not updated in this entry.
        /// </remarks>
        public ThreadArchiveDuration? DefaultArchiveDuration { get; }

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

        /// <summary>
        ///     Gets the user limit configured in the created voice channel.
        /// </summary>
        public int? UserLimit { get; }

        /// <summary>
        ///     Gets the video quality mode configured in the created voice channel.
        /// </summary>
        public VideoQualityMode? VideoQualityMode { get; }

        /// <summary>
        ///     Gets the region configured in the created voice channel.
        /// </summary>
        public string RtcRegion { get; }

        /// <summary>
        ///     Gets channel flags configured for the created channel.
        /// </summary>
        public ChannelFlags? Flags { get; }

        /// <summary>
        ///     Gets the thread archive duration that was set in the created channel.
        /// </summary>
        public ThreadArchiveDuration? AutoArchiveDuration { get; }

        /// <summary>
        ///     Gets the default slow mode interval that will be set in child threads in the channel.
        /// </summary>
        public int? DefaultSlowModeInterval { get; }
    }
}
