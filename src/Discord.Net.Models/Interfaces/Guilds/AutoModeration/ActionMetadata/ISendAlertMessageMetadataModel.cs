namespace Discord.Models;

public interface ISendAlertMessageMetadataModel : IAutoModerationActionMetadataModel
{
    ulong ChannelId { get; }
}
