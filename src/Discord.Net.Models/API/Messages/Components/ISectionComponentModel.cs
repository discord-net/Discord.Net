using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.Section)]
public interface ISectionComponentModel : IContainerAtom
{
    IReadOnlyList<ISectionComponentAtom> Components { get; }
    
    ISectionComponentAccessory Accessory { get; }
}

[APIModel]
public interface ISectionComponentAtom : IMessageComponentModel;

[APIModel]
public interface ISectionComponentAccessory : IMessageComponentModel;