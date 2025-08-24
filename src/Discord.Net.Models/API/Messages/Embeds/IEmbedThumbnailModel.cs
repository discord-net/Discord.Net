using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IEmbedThumbnailModel : IModel
{
    string Url { get; }
    
    Optional<string> ProxyUrl { get; }
    
    Optional<int> Height { get; }
    
    Optional<int> Width { get; }
}