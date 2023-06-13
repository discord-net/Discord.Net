namespace Discord.Models
{
    public interface IReactionModel
    {
        int Count { get; }
        bool Me { get; }
        ulong? EmojiId { get; }
        string? EmojiName { get; }
    }
}
