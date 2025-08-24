using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.StringSelect)]
public interface IStringSelectComponentModel : 
    IMessageComponentModel,
    IMessageComponentWithCustomId
{
    [Max(Constants.MAX_SELECT_OPTIONS_LENGTH)]
    IReadOnlyList<ISelectOptionModel> Options { get; }
    
    Optional<string> Placeholder { get; }
    
    Optional<int> MinValues { get; }
    
    Optional<int> MaxValues { get; }
    
    Optional<bool> Disabled { get; }
}