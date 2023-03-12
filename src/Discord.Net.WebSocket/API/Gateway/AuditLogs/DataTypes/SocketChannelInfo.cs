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
        RtcRegion = model.Region;
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
    public ChannelFlags Flags { get; }

    /// <summary>
    ///     Gets the thread archive duration that was set in the created channel.
    /// </summary>
    public ThreadArchiveDuration? AutoArchiveDuration { get; }

    /// <summary>
    ///     Gets the default slow mode interval that will be set in child threads in the channel.
    /// </summary>
    public int? DefaultSlowModeInterval { get; }

}
