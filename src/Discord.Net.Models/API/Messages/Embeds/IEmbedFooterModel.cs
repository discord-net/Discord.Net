using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IEmbedFooterModel : IModel
{
    string Text { get; }
    
    Optional<string> IconUrl { get; }
    
    Optional<string> ProxyIconUrl { get; }
}