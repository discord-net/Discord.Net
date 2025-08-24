using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IEmbedVideoModel : IModel
{
    Optional<string> Url { get; }
    
    Optional<string> ProxyUrl { get; }
    
    Optional<int> Height { get; }
    
    Optional<int> Width { get; }
}