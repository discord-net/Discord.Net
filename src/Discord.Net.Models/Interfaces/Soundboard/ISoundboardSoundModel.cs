namespace Discord.Models;

[ModelEquality]
public partial interface ISoundboardSoundModel : IEntityModel<ulong>
{
    string Name { get; }
    double Volume { get; }
    ulong? EmojiId { get; }
    string? EmojiName { get; }
    bool IsAvailable { get; }
}