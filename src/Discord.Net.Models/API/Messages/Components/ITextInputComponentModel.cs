using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.TextInput)]
public interface ITextInputComponentModel : 
    IMessageComponentModel,
    IMessageComponentWithCustomId
{
    TextInputStyle Style { get; }
    
    string Label { get; }
    
    Optional<int> MinLength { get; }
    
    Optional<int> MaxLength { get; }
    
    Optional<bool> Required { get; }
    
    Optional<string> Value { get; }
    
    Optional<string> Placeholder { get; }
}