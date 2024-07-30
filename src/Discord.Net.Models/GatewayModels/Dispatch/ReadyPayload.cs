using Discord.Models.Dispatch;
using Discord.Models.Json;
using System.Text.Json.Serialization;

namespace Discord.Models.GatewayModels.Dispatch;

public sealed class ReadyPayload : IReadyPayload
{
    [JsonPropertyName("v")]
    public int Version { get; set; }

    [JsonPropertyName("user")]
    public required SelfUser User { get; set; }

    [JsonPropertyName("guild")]
    public required UnavailableGuild[] Guilds { get; set; }

    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }

    [JsonPropertyName("resume_gateway_url")]
    public required string ResumeGatewayUrl { get; set; }

    [JsonPropertyName("shard")]
    public int[]? Shard { get; set; }

    ISelfUserModel IReadyPayload.User => User;
    IEnumerable<IUnavailableGuild> IReadyPayload.Guilds => Guilds;
}
