namespace Discord.Models;

public interface IKeywordPresetTriggerMetadataModel : ITriggerMetadataModel
{
    int[] Presets { get; }
    string[] AllowList { get; }
}
