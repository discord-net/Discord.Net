namespace Discord.Models;

[ModelEquality]
public partial interface IGuildStickerModel : IStickerModel
{
    bool? Available { get; }
    ulong GuildId { get; }
    ulong? AuthorId { get; }
}
