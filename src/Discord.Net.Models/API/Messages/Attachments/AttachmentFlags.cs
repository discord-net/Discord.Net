namespace Discord.Models;

[Flags]
public enum AttachmentFlags
{
    None = 0,
    IsRemix = 1 << 2
}