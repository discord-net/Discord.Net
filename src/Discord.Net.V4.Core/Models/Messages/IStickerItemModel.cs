namespace Discord.Models;

public interface IStickerItemModel
{
    ulong Id { get; }
    string Name { get; }
    StickerFormatType FormatType { get; }
}
