namespace Discord.Models;

public interface IBlockMessageMetadataModel : IAutoModerationActionMetadataModel
{
    string? CustomMessage { get; }
}
