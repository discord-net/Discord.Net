namespace Discord.Models;

public interface IStickerItemModel : IEntityModel<ulong>
{
    string Name { get; }
    int FormatType { get; }
}
