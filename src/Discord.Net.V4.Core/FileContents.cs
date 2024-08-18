using Discord.Models;

namespace Discord;

public readonly struct FileContents(string filename, string? contentType, Stream stream)
    : IEntityProperties<MultipartFile>
{
    public readonly string Filename = filename;
    public readonly string? ContentType = contentType;
    public readonly Stream Stream = stream;

    public MultipartFile ToApiModel(MultipartFile existing = default)
    {
        return new MultipartFile(Filename, ContentType, Stream);
    }
}