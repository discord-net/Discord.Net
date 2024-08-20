namespace Discord.Models;

[ModelEquality]
public partial interface IReactionModel : IEntityModel<DiscordEmojiId>
{
    int Total { get; }
    int BurstCount { get; }
    int NormalCount { get; }
    bool Me { get; }
    bool MeBurst { get; }
    string[] BurstColors { get; }
}
