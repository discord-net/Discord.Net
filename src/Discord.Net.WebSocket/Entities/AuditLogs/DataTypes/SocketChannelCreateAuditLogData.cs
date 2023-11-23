using Discord.Rest;
using Discord.API.AuditLogs;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a channel creation.
/// </summary>
public class SocketChannelCreateAuditLogData : ISocketAuditLogData
{
    private SocketChannelCreateAuditLogData(ChannelInfoAuditLogModel model, EntryModel entry)
    {
        ChannelId = entry.TargetId!.Value;
        ChannelName = model.Name;
        ChannelType = model.Type!.Value; 
        SlowModeInterval = model.RateLimitPerUser;
        IsNsfw = model.IsNsfw;
        Bitrate = model.Bitrate;
        Topic = model.Topic;
        AutoArchiveDuration = model.AutoArchiveDuration;
        DefaultSlowModeInterval = model.DefaultThreadRateLimitPerUser;
        DefaultAutoArchiveDuration = model.DefaultArchiveDuration;

        AvailableTags = model.AvailableTags?.Select(x => new ForumTag(x.Id,
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

        VideoQualityMode = model.VideoQualityMode;
        RtcRegion = model.Region;
        Flags = model.Flags;
        UserLimit = model.UserLimit;
    }

    internal static SocketChannelCreateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<ChannelInfoAuditLogModel>(changes, discord);

        return new SocketChannelCreateAuditLogData(data, entry);
    }

    /// <summary>
    ///     Gets the snowflake ID of the created channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the created channel.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the name of the created channel.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the created channel.
    /// </returns>
    public string ChannelName { get; }

    /// <summary>
    ///     Gets the type of the created channel.
    /// </summary>
    /// <returns>
    ///     The type of channel that was created.
    /// </returns>
    public ChannelType ChannelType { get; }

    /// <summary>
    ///     Gets the current slow-mode delay of the created channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public int? SlowModeInterval { get; }

    /// <summary>
    ///     Gets the value that indicates whether the created channel is NSFW.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the created channel has the NSFW flag enabled; otherwise <see langword="false" />.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public bool? IsNsfw { get; }

    /// <summary>
    ///     Gets the bit-rate that the clients in the created voice channel are requested to use.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the bit-rate (bps) that the created voice channel defines and requests the
    ///     client(s) to use.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public int? Bitrate { get; }

    /// <summary>
    ///     Gets the thread archive duration that was set in the created channel.
    /// </summary>
    public ThreadArchiveDuration? AutoArchiveDuration { get; }

    /// <summary>
    ///     Gets the default thread archive duration that was set in the created channel.
    /// </summary>
    public ThreadArchiveDuration? DefaultAutoArchiveDuration { get; }

    /// <summary>
    ///     Gets the default slow mode interval that will be set in child threads in the channel.
    /// </summary>
    public int? DefaultSlowModeInterval { get; }

    /// <summary>
    ///     Gets the topic that was set in the created channel.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    ///     Gets tags available in the created forum channel.
    /// </summary>
    public IReadOnlyCollection<ForumTag> AvailableTags { get; }

    /// <summary>
    ///     Gets the default reaction added to posts in the created forum channel.
    /// </summary>
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
}
