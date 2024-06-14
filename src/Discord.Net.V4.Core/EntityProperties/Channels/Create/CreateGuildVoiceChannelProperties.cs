using Discord.Models.Json;

namespace Discord;

public class CreateGuildVoiceChannelProperties : CreateGuildChannelProperties
{
    public Optional<int?> Bitrate { get; set; }
    public Optional<int?> UserLimit { get; set; }
    public Optional<int?> Slowmode { get; set; }
    public Optional<EntityOrId<ulong, ICategoryChannel>?> Category { get; set; }
    public Optional<bool?> IsNsfw { get; set; }
    public Optional<EntityOrId<string, VoiceRegion>?> RtcRegion { get; set; }
    public Optional<VideoQualityMode?> VideoQualityMode { get; set; }

    public override CreateGuildChannelParams ToApiModel(CreateGuildChannelParams? existing = default)
    {
        existing ??= base.ToApiModel(existing);

        existing.Bitrate = Bitrate;
        existing.UserLimit = UserLimit;
        existing.RateLimitPerUser = Slowmode;
        existing.ParentId = Category.MapToNullableId();
        existing.IsNsfw = IsNsfw;
        existing.RtcRegion = RtcRegion.MapToId();
        existing.VideoQualityMode = VideoQualityMode.MapToInt();

        return existing;
    }
}
