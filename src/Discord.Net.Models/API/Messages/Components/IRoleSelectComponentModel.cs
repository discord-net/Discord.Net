using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.RoleSelect)]
public interface IRoleSelectComponentModel : 
    IMessageComponentModel,
    IMessageComponentWithCustomId
{
    Optional<string> Placeholder { get; }
    
    Optional<IReadOnlyList<ISelectDefaultValueModel>> DefaultValues { get; }
    
    Optional<int> MinValues { get; }
    
    Optional<int> MaxValues { get; }
    
    Optional<bool> Disabled { get; }
}