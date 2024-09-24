using Discord.Models;

namespace Discord;

public sealed class CreateGuildSoundboardSoundProperties :
    IEntityProperties<CreateGuildSoundboardSoundParams>
{
    public required string Name { get; set; }
    public required SoundData Sound { get; set; }
    public Optional<double?> Volume { get; set; }
    public Optional<DiscordEmojiId> Emoji { get; set; }
    
    public CreateGuildSoundboardSoundParams ToApiModel(CreateGuildSoundboardSoundParams? existing = default)
    {
        existing ??= new()
        {
            Name = Name,
            Sound = Sound.ToSoundData()
        };

        existing.Volume = Volume;

        if (Emoji.IsSpecified)
        {
            switch (Emoji.Value)
            {
                case {Id: not null}: 
                    existing.EmojiId = Emoji.Value.Id.Value;
                    break;
                case {Name: not null}:
                    existing.EmojiName = Emoji.Value.Name;
                    break;
            }
        }

        return existing;
    }
}