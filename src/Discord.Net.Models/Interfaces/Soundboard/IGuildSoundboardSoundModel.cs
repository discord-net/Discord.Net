namespace Discord.Models;

[ModelEquality]
public partial interface IGuildSoundboardSoundModel : ISoundboardSoundModel
{
    ulong GuildId { get; }
    ulong? UserId { get; }
}