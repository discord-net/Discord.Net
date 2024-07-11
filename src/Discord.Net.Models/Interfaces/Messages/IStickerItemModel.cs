namespace Discord.Models;

[ModelEquality]
public partial interface IStickerItemModel : IEntityModel<ulong>
{
    string Name { get; }
    int FormatType { get; }
}
