using Discord.Models;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.ListDefaultSoundboardSounds))]
public partial interface ISoundboardSound : 
    ISnowflakeEntity<ISoundboardSoundModel>,
    ISoundboardSoundActor
{
    string Name { get; }
    double Volume { get; }
    DiscordEmojiId? Emoji { get; }
    bool IsAvailable { get; }
}