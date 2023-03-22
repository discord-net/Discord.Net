using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.AuditLogs.ChannelInfoAuditLogModel;

namespace Discord.WebSocket;

/// <summary>
///     Represents information for a channel.
/// </summary>
public struct SocketChannelInfo
{
    internal SocketChannelInfo(Model model)
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
        RTCRegion = model.Region;
        Flags = model.Flags;
        UserLimit = model.UserLimit;
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

    /// <inheritdoc cref="IVoiceChannel.UserLimit"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public int? UserLimit { get; }

    /// <inheritdoc cref="IVoiceChannel.VideoQualityMode"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public VideoQualityMode? VideoQualityMode { get; }

    /// <inheritdoc cref="IAudioChannel.RTCRegion"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public string RTCRegion { get; }

    /// <inheritdoc cref="IGuildChannel.Flags"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ChannelFlags? Flags { get; }

    /// <inheritdoc cref="IThreadChannel.AutoArchiveDuration"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public ThreadArchiveDuration? AutoArchiveDuration { get; }

    /// <inheritdoc cref="IForumChannel.DefaultSlowModeInterval"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not updated in this entry.
    /// </remarks>
    public int? DefaultSlowModeInterval { get; }

}
