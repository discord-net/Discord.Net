namespace Discord.Models;

[ModelEquality]
public partial interface IPollMediaModel : IModel
{
    string? Text { get; }
    DiscordEmojiId? Emoji { get; }
}