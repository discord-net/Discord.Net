namespace Discord.Models;

[ModelEquality]
public partial interface ITagModel : IEntityModel<ulong>
{
    string Name { get; }
    bool Moderated { get; }
    DiscordEmojiId? Emoji { get; }
}
