using Discord.Models.Json;

namespace Discord;

public class ModifyWebhookWithTokenProperties : IEntityProperties<ModifyWebhookWithTokenParams>
{
    public Optional<string> Name { get; set; }
    public Optional<Image?> Avatar { get; set; }

    public ModifyWebhookWithTokenParams ToApiModel(ModifyWebhookWithTokenParams? existing = default) =>
        existing ?? new ModifyWebhookWithTokenParams {Avatar = Avatar.Map(v => v?.ToImageData()), Name = Name};
}
