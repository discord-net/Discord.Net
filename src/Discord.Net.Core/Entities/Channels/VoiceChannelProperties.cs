using System;

namespace Discord;

/// <summary>
///     Provides properties that are used to modify an <see cref="IVoiceChannel" /> with the specified changes.
/// </summary>
public class VoiceChannelProperties : GuildChannelProperties
{
    /// <summary>
    ///     Gets or sets the bitrate of the voice connections in this channel. Must be greater than 8000.
    /// </summary>
    public Optional<int> Bitrate { get; set; }
    /// <summary>
    ///     Gets or sets the maximum number of users that can be present in a channel, or <c>null</c> if none.
    /// </summary>
    public Optional<int?> UserLimit { get; set; }
    /// <summary>
    ///     Gets or sets the channel voice region id, automatic when set to <see langword="null"/>.
    /// </summary>
    public Optional<string> RTCRegion { get; set; }

    /// <summary>
    ///     Get or sets the video quality mode for this channel.
    /// </summary>
    public Optional<VideoQualityMode> VideoQualityMode { get; set; }
    /// <summary>
    ///     Gets or sets whether this channel should be flagged as NSFW.
    /// </summary>
    /// <remarks>
    ///     Setting this value to <c>true</c> will mark the channel as NSFW (Not Safe For Work) and will prompt the
    ///     user about its possibly mature nature before they may view the channel; setting this value to <c>false</c> will
    ///     remove the NSFW indicator.
    /// </remarks>
    public Optional<bool> IsNsfw { get; set; }
    /// <summary>
    ///     Gets or sets the slow-mode ratelimit in seconds for this channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to anything above zero will require each user to wait X seconds before
    ///     sending another message; setting this value to <c>0</c> will disable slow-mode for this channel.
    ///     <note>
    ///         Users with <see cref="Discord.ChannelPermission.ManageMessages"/> or 
    ///         <see cref="ChannelPermission.ManageChannels"/> will be exempt from slow-mode.
    ///     </note>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value does not fall within [0, 21600].</exception>
    public Optional<int> SlowModeInterval { get; set; }
}
