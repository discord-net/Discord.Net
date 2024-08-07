namespace Discord.Models;

public interface IUpdateVoiceStatePayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    ulong? ChannelId { get; }
    bool SelfMute { get; }
    bool SelfDeafen { get; }
}
