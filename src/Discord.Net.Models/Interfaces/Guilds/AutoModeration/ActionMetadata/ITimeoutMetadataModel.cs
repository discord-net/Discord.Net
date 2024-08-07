namespace Discord.Models;

public interface ITimeoutMetadataModel : IAutoModerationActionMetadataModel
{
    int TimeoutDuration { get; }
}
