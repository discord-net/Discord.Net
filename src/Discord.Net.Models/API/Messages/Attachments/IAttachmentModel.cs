using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IAttachmentModel : IEntityModel<Snowflake>
{
    string Filename { get; }
    
    Optional<string> Title { get; }
    
    Optional<string> Description { get; }
    
    Optional<string> ContentType { get; }
    
    int Size { get; }
    
    string Url { get; }
    
    string ProxyUrl { get; }
    
    Optional<int?> Height { get; }
    
    Optional<int?> Width { get; }
    
    Optional<bool> Ephemeral { get; }
    
    Optional<float> DurationSecs { get; }
    
    Optional<string> Waveform { get; }
    
    Optional<AttachmentFlags> Flags { get; }
}