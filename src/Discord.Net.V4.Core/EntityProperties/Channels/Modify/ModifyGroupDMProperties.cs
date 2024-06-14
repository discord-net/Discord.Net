using Discord.Models.Json;

namespace Discord;

public sealed class ModifyGroupDMProperties : ModifyChannelBaseProperties, IEntityProperties<ModifyGroupDmParams>
{
    public Optional<Image?> Icon { get; set; }

    public ModifyGroupDmParams ToApiModel(ModifyGroupDmParams? existing = default)
    {
        existing ??= new();
        base.ToApiModel(existing);

        existing.Icon = Icon.Map(v => v?.ToImageData());

        return existing;
    }
}
