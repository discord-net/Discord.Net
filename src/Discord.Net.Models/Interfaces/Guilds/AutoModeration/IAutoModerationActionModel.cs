namespace Discord.Models;

public interface IAutoModerationActionModel
{
    int Type { get; }
    IAutoModerationActionMetadataModel? Metadata { get; }
}
