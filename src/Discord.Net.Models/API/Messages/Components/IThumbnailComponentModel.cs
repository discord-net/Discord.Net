using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.Thumbnail)]
public interface IThumbnailComponentModel :
    ISectionComponentAccessory
{
    IUnfurledMediaItemModel Media { get; }
    
    Optional<string> Description { get; }
    
    Optional<bool> Spoiler { get; }
}