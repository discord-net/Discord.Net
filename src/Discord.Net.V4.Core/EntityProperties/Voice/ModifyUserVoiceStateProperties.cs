using Discord.Models.Json;

namespace Discord;

public class ModifyUserVoiceStateProperties : IEntityProperties<ModifyUserVoiceStateParams>
{
    public Optional<EntityOrId<ulong, IVoiceChannel>> Channel { get; set; }
    public Optional<bool> IsSuppressed { get; set; }

    public ModifyUserVoiceStateParams ToApiModel(ModifyUserVoiceStateParams? existing = default)
    {
        existing ??= new ModifyUserVoiceStateParams();

        existing.Suppress = IsSuppressed;
        existing.ChannelId = Channel.MapToId();

        return existing;
    }
}
