using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildMemberRemoved : IGuildMemberRemovedPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    IUserModel IGuildMemberRemovedPayloadData.User => User;
}
