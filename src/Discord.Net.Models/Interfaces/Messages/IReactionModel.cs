namespace Discord.Models;

[ModelEquality]
public partial interface IReactionModel : IEntityModel
{
    int Total { get; }
    int BurstCount { get; }
    int NormalCount { get; }
    bool Me { get; }
    bool MeBurst { get; }
    ulong? EmojiId { get; }
    string? EmojiName { get; }
    string[] BurstColors { get; }
}
