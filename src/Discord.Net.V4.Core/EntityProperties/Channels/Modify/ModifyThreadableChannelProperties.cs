using Discord.Models.Json;

namespace Discord;

public class ModifyThreadableChannelProperties : ModifyGuildChannelProperties
{
    public Optional<ThreadArchiveDuration?> DefaultAutoArchiveDuration { get; set; }
    public Optional<int?> DefaultThreadSlowmode { get; set; }

    public override ModifyGuildChannelParams ToApiModel(ModifyGuildChannelParams? existing = null)
    {
        existing ??= new();
        base.ToApiModel(existing);

        existing.DefaultThreadRateLimitPerUser = DefaultThreadSlowmode;
        existing.DefaultAutoArchiveDuration = DefaultAutoArchiveDuration.MapToInt();

        return existing;
    }
}
