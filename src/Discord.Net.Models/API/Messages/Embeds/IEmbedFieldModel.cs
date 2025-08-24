using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IEmbedFieldModel : IModel
{
    string Name { get; }
    
    string Value { get; }
    
    Optional<bool> Inline { get; }
}