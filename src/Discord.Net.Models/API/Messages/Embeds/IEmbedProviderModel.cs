using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IEmbedProviderModel : IModel
{
    Optional<string> Name { get; }
    
    Optional<string> Url { get; }
}