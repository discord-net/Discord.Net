namespace Discord.Models;

public sealed class UploadAttachment
{
    public ulong Id { get; set; }
    public Optional<string> Filename { get; set; }
    public Optional<string> Description { get; set; }
}
