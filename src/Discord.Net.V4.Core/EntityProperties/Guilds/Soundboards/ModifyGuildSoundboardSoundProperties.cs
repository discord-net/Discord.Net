using Discord.Models;

namespace Discord;

public sealed class ModifyGuildSoundboardSoundProperties : 
    IEntityProperties<ModifyGuildSoundboardSoundParams>
{
    public Optional<string> Name { get; set; }
    public Optional<double?> Volume { get; set; }
    public Optional<DiscordEmojiId> Emoji { get; set; }
    
    public ModifyGuildSoundboardSoundParams ToApiModel(ModifyGuildSoundboardSoundParams? existing = default)
    {
        return new ModifyGuildSoundboardSoundParams()
        {
            Name = Name,
            Volume = Volume,
            EmojiId = Emoji.Map(v => v.Id),
            EmojiName = Emoji.Map(v => v.Name)
        };
    }
}