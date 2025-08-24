using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.Button)]
public interface IButtonComponentModel : 
    IMessageComponentWithCustomId,
    ISectionComponentAccessory
{
    ButtonStyle Style { get; }
    
    Optional<string> Label { get; }
    
    Optional<EmojiId> Emoji { get; }
    
    Optional<Snowflake> SkuId { get; }
    
    Optional<string> Url { get; }
    
    Optional<bool> Disabled { get; }
}