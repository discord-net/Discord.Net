using Discord.Models.Json;

namespace Discord.Models;

public class AttachmentUploadParams
{
    public virtual Optional<AttachmentParam[]> Attachments { get; set; }
}

public sealed class AttachmentParam
{
    public required Stream Stream { get; init; }
    public required UploadAttachment Attachment { get; init; }
}
