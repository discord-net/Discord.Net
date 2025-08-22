namespace Discord.Models;

[Flags]
public enum ChannelFlags
{
    Pinned = 1 << 1,
    RequireTag = 1 << 4,
    HideMediaDownloadOptions = 1 << 15
}