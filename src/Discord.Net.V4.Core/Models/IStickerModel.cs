namespace Discord.Models
{
    public interface IStickerModel : IEntityModel<ulong>
    {
        ulong? PackId { get; }
        string Name { get; }
        string? Description { get; }
        string Tags { get; }
        StickerType Type { get; }
        StickerFormatType FormatType { get; }
        bool? Available { get; }
        ulong? GuildId { get; }
        ulong? AuthorId { get; }
        int? SortValue { get; }
    }
}
