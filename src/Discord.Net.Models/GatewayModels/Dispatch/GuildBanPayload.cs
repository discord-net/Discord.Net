using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildBanPayload : IGuildBanPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    public required User User { get; set; }

    IUserModel IGuildBanPayloadData.User => User;
}
