namespace Discord.Models;

public interface IMemberProfileTriggerMetadataModel : ITriggerMetadataModel
{
    string[] KeywordFilter { get; }
    string[] RegexPatterns { get; }
    string[] AllowList { get; }
}
