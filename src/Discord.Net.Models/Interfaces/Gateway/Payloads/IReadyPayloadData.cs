namespace Discord.Models.Dispatch;

public interface IReadyPayloadData : IGatewayPayloadData
{
    int Version { get; }
    ISelfUserModel User { get; }
    IEnumerable<IUnavailableGuild> Guilds { get; }
    string SessionId { get; }
    string ResumeGatewayUrl { get; }
    int[]? Shard { get; }
}
