using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.File)]
public interface IFileComponentModel : IContainerAtom
{
    IUnfurledMediaItemModel File { get; }
    
    Optional<bool> Spoiler { get; }
    
    string Name { get; }
    
    int Size { get; }
}