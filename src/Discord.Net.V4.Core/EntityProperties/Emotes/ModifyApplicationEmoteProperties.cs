using Discord.Models.Json;

namespace Discord;

public class ModifyApplicationEmoteProperties : IEntityProperties<ModifyApplicationEmojiParams>
{
    public Optional<string> Name { get; set; }
    
    public ModifyApplicationEmojiParams ToApiModel(ModifyApplicationEmojiParams? existing = default)
    {
        return new ModifyApplicationEmojiParams()
        {
            Name = Name.Value
        };
    }
}