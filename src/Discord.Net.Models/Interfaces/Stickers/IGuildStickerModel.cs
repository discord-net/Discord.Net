namespace Discord.Models;

public interface IGuildStickerModel : IStickerModel
{
    bool? Available { get; }
    ulong GuildId { get; }
    ulong? AuthorId { get; }
}
