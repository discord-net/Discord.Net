using Discord.Rest;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Model = Discord.API.AuditLogs.ChannelInfoAuditLogModel;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a channel deletion.
/// </summary>
public class SocketChannelDeleteAuditLogData : ISocketAuditLogData
{
    private SocketChannelDeleteAuditLogData(Model model, EntryModel entry)
    {
        ChannelId = entry.TargetId!.Value;
        ChannelType = model.Type;
        ChannelName = model.Name;

        Topic = model.Topic;
        IsNsfw = model.IsNsfw;
        Bitrate = model.Bitrate;
        DefaultArchiveDuration = model.DefaultArchiveDuration;
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

        Overwrites = model.Overwrites?.Select(x
            => new Overwrite(x.TargetId,
                x.TargetType,
                new OverwritePermissions(x.Allow, x.Deny))).ToImmutableArray();
    }

    internal static SocketChannelDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<Model>(changes, discord);

        return new SocketChannelDeleteAuditLogData(data, entry);
    }

    /// <summary>
    ///     Gets the snowflake ID of the deleted channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the deleted channel.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the name of the deleted channel.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the deleted channel.
    /// </returns>
    public string ChannelName { get; }

    /// <summary>
    ///     Gets the type of the deleted channel.
    /// </summary>
    /// <returns>
    ///     The type of channel that was deleted.
    /// </returns>
    public ChannelType? ChannelType { get; }

    /// <summary>
    ///     Gets the slow-mode delay of the deleted channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    ///     <c>null</c> if this is not mentioned in this entry.
    /// </returns>
    public int? SlowModeInterval { get; }

    /// <summary>
    ///     Gets the value that indicates whether the deleted channel was NSFW.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if this channel had the NSFW flag enabled; otherwise <c>false</c>.
    ///     <c>null</c> if this is not mentioned in this entry.
    /// </returns>
    public bool? IsNsfw { get; }

    /// <summary>
    ///     Gets the bit-rate of this channel if applicable.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the bit-rate set of the voice channel.
    ///     <c>null</c> if this is not mentioned in this entry.
    /// </returns>
    public int? Bitrate { get; }

    /// <summary>
    ///     Gets a collection of permission overwrites that was assigned to the deleted channel.
    /// </summary>
    /// <returns>
    ///     A collection of permission <see cref="Overwrite"/>.
    /// </returns>
    public IReadOnlyCollection<Overwrite> Overwrites { get; }

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
    ///     Gets the thread archive duration that was configured for the created channel.
    /// </summary>
    public ThreadArchiveDuration? AutoArchiveDuration { get; }

    /// <summary>
    ///     Gets the default slow mode interval that was configured for the channel.
    /// </summary>
    public int? DefaultSlowModeInterval { get; }

    /// <inheritdoc cref="ITextChannel.DefaultArchiveDuration"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not specified in this entry..
    /// </remarks>
    public ThreadArchiveDuration? DefaultArchiveDuration { get; }

    /// <inheritdoc cref="IForumChannel.Tags"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not specified in this entry..
    /// </remarks>
    public IReadOnlyCollection<ForumTag> ForumTags { get; }

    /// <inheritdoc cref="ITextChannel.Topic"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not specified in this entry..
    /// </remarks>
    public string Topic { get; }

    /// <inheritdoc cref="IForumChannel.DefaultReactionEmoji"/>
    /// <remarks>
    ///     <see langword="null" /> if the value was not specified in this entry..
    /// </remarks>
    public IEmote DefaultReactionEmoji { get; }
}
