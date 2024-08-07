namespace Discord.Models;

public interface IMentionSpamTriggerMetadataModel : ITriggerMetadataModel
{
    int MentionTotalLimit { get; }
    bool MentionRaidProtectionEnabled { get; }
}
