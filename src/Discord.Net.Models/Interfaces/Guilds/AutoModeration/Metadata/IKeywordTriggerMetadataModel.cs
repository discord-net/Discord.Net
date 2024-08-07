namespace Discord.Models;

public interface IKeywordTriggerMetadataModel : ITriggerMetadataModel
{
    string[] KeywordFilter { get; }
    string[] RegexPatterns { get; }
    string[] AllowList { get; }
}
