using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IUnfurledMediaItemModel : IModel
{
    string Url { get; }
    
    Optional<string> ProxyUrl { get; }
    
    Optional<int?> Height { get; }
    
    Optional<int?> Width { get; }
    
    Optional<string> ContentType { get; }
    
    Optional<Snowflake> AttachmentId { get; }
}