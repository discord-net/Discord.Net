using Discord.Models;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.ListGuildSoundboardSounds))]
[Fetchable(nameof(Routes.GetGuildSoundboardSound))]
public partial interface IGuildSoundboardSound :
    ISoundboardSound,
    ISnowflakeEntity<IGuildSoundboardSoundModel>,
    IGuildSoundboardSoundActor
{
    IUserActor? Creator { get; }
    
    [SourceOfTruth]
    new IGuildSoundboardSoundModel GetModel();
}