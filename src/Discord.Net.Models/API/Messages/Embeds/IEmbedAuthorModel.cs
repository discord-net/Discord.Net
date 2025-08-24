using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IEmbedAuthorModel : IModel
{
    string Name { get; }
    
    Optional<string> Url { get; }
    
    Optional<string> IconUrl { get; }
    
    Optional<string> ProxyIconUrl { get; }
}