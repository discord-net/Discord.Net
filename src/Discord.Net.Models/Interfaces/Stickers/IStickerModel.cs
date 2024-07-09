namespace Discord.Models;

public interface IStickerModel : IEntityModel<ulong>
{
    ulong? PackId { get; }
    string Name { get; }
    string? Description { get; }
    string Tags { get; }
    int Type { get; }
    int FormatType { get; }
    int? SortValue { get; }
}
