namespace Discord.Models;

public readonly struct MultipartFile(string filename, string? contentType, Stream stream)
{
    public readonly string Filename = filename;
    public readonly string? ContentType = contentType;
    public readonly Stream Stream = stream;
}