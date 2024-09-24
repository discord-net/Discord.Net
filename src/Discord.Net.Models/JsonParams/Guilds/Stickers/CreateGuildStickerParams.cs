namespace Discord.Models.Json;

public sealed class CreateGuildStickerParams : IMultipartParams
{
    public required string Name { get; set; }
    public Optional<string> Description { get; set; }
    public Optional<string> Tags { get; set; }
    public MultipartFile File { get; set; }
    
    public IDictionary<string, object?> GetKeys()
    {
        return new Dictionary<string, object?>()
        {
            {"name", Name},
            {"description", Description | string.Empty},
            {"tags", Tags | ""}
        };
    }

    public IDictionary<string, MultipartFile> GetFiles()
        => new Dictionary<string, MultipartFile> {{"file", File}};
}