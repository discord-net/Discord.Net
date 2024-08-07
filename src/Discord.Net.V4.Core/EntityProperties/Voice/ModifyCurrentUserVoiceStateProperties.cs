using Discord.Models.Json;

namespace Discord;

public sealed class ModifyCurrentUserVoiceStateProperties :
    ModifyUserVoiceStateProperties,
    IEntityProperties<ModifyCurrentUserVoiceStateParams>
{
    public Optional<DateTimeOffset?> RequestToSpeakTimestamp { get; set; }

    public ModifyCurrentUserVoiceStateParams ToApiModel(ModifyCurrentUserVoiceStateParams? existing = default)
    {
        existing ??= new ModifyCurrentUserVoiceStateParams();

        base.ToApiModel(existing);

        existing.RequestToSpeakTimestamp = RequestToSpeakTimestamp;

        return existing;
    }
}
