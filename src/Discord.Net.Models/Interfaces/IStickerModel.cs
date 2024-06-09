namespace Discord.Models;

public interface IStickerModel : IEntityModel<ulong>
{
    ulong? PackId { get; }
    string Name { get; }
    string? Description { get; }
    string Tags { get; }
    int Type { get; }
    int FormatType { get; }
    bool? Available { get; }
    ulong? GuildId { get; }
    ulong? AuthorId { get; }
    int? SortValue { get; }
}
