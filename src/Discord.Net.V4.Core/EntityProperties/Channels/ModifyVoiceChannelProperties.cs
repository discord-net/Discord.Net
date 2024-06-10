using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Provides properties that are used to modify an <see cref="IVoiceChannel" /> with the specified changes.
/// </summary>
public class ModifyVoiceChannelProperties : ModifyGuildChannelProperties
{
    /// <summary>
    ///     Gets or sets the bitrate of the voice connections in this channel. Must be greater than 8000.
    /// </summary>
    public Optional<int?> Bitrate { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of users that can be present in a channel, or <see langword="null" /> if none.
    /// </summary>
    public Optional<int?> UserLimit { get; set; }

    /// <summary>
    ///     Gets or sets the channel voice region id, automatic when set to <see langword="null" />.
    /// </summary>
    public Optional<string?> RTCRegion { get; set; }

    /// <summary>
    ///     Get or sets the video quality mode for this channel.
    /// </summary>
    public Optional<VideoQualityMode?> VideoQualityMode { get; set; }

    public override ModifyGuildChannelParams ToApiModel(ModifyGuildChannelParams? existing = null)
    {
        existing ??= new();
        base.ToApiModel(existing);

        existing.Bitrate = Bitrate;
        existing.UserLimit = UserLimit;
        existing.RtcRegion = RTCRegion;
        existing.VideoQualityMode = VideoQualityMode.Map(v => (int?)v);

        return existing;
    }
}
