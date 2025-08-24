namespace Discord.Models;

[Flags]
public enum MessageFlags
{
    None = 0,
    
    Crossposted = 1 << 0,
    IsCrosspost = 1 << 1,
    SuppressEmbeds = 1 << 2,
    SourceMessageDeleted = 1 << 3,
    Urgent = 1 << 4,
    HasThread = 1 << 5,
    Ephemeral = 1 << 6,
    Loading = 1 << 7,
    FailedToMentionSomeRolesInThread = 1 << 8,
    SuppressNotifications = 1 << 12,
    IsVoiceMessage = 1 << 13,
    HasSnapshot = 1 << 14,
    IsComponentsV2 = 1 << 15
}