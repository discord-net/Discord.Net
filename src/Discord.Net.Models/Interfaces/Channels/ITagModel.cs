namespace Discord.Models;

[ModelEquality]
public partial interface ITagModel : IEntityModel<ulong>
{
    string Name { get; }
    bool Moderated { get; }
    string? EmojiName { get; }
    ulong? EmojiId { get; }
}
