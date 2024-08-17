using Discord.Models.Dispatch;
using Discord.Models.Json;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ReadyPayloadData : IReadyPayloadData
{
    [JsonPropertyName("v")]
    public int Version { get; set; }

    [JsonPropertyName("user")]
    public required SelfUser User { get; set; }

    [JsonPropertyName("guilds")]
    public required UnavailableGuild[] Guilds { get; set; }

    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }

    [JsonPropertyName("resume_gateway_url")]
    public required string ResumeGatewayUrl { get; set; }

    [JsonPropertyName("shard")]
    public int[]? Shard { get; set; }
    
    [JsonPropertyName("application")]
    public required PartialApplication Application { get; set; }
    
    IPartialApplicationModel IReadyPayloadData.Application => Application;
    ISelfUserModel IReadyPayloadData.User => User;
    IEnumerable<IUnavailableGuild> IReadyPayloadData.Guilds => Guilds;
}
