using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class PresenceUpdated : IPresenceUpdatedPayloadData
{
    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("activities")]
    public required Activity[] Activities { get; set; }

    [JsonPropertyName("client_status")]
    public required ClientStatus ClientStatus { get; set; }

    IUserModel IPresenceUpdatedPayloadData.User => User;
    IClientStatusModel IPresenceUpdatedPayloadData.ClientStatus => ClientStatus;
    IEnumerable<IActivityModel> IPresenceUpdatedPayloadData.Activities => Activities;
}
