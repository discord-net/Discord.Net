namespace Discord.Models.Dispatch;

public interface IReadyPayload : IGatewayPayloadData
{
    int Version { get; }
    ISelfUserModel User { get; }
    IEnumerable<IUnavailableGuild> Guilds { get; }
    string SessionId { get; }
    string ResumeGatewayUrl { get; }
    int[]? Shard { get; }
}
