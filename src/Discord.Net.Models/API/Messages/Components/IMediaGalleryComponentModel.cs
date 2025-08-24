using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.MediaGallery)]
public interface IMediaGalleryComponentModel : IContainerAtom
{
    IReadOnlyList<IMediaGalleryItemModel> Items { get; }
}

[APIModel]
public interface IMediaGalleryItemModel : IModel
{
    IUnfurledMediaItemModel Media { get; }
    
    Optional<string> Description { get; }
    
    Optional<bool> Spoiler { get; }
}