using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.Separator)]
public interface ISeparatorComponentItem : IContainerAtom
{
    Optional<bool> Divider { get; }
    
    Optional<SeparatorSpacing> Spacing { get; }
}

public enum SeparatorSpacing
{
    Small = 1,
    Large = 2
}