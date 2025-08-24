using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.Container)]
public interface IContainerComponentModel : IMessageComponentModel
{
    IReadOnlyList<IContainerAtom> Components { get; }
    
    Optional<Color?> AccentColor { get; }
    
    Optional<bool> Spoiler { get; }
}

[APIModel]
public interface IContainerAtom : IMessageComponentModel;