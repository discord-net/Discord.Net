using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Presence : IPresenceModel
{
    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("status")]
    public Optional<string> Status { get; set; }

    [JsonPropertyName("client_status")]
    public Optional<ClientStatus> ClientStatus { get; set; }

    [JsonPropertyName("activities")]
    public Optional<Activity[]> Activities { get; set; }

    ulong IPresenceModel.UserId => User.Id;
    ulong? IPresenceModel.GuildId => ~GuildId;
    string? IPresenceModel.Status => ~Status;
    IEnumerable<IActivityModel>? IPresenceModel.Activities => Activities | [];
    IClientStatusModel? IPresenceModel.ClientStatus => ~ClientStatus;
}
