using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IStickerItemModel : IEntityModel<Snowflake>
{
    string Name { get; }
    
    StickerFormatType FormatType { get; }
}