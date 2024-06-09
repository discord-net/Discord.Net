using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateChannelInviteParams
{
    [JsonPropertyName("max_age")]
    public Optional<int> MaxAge { get; set; }

    [JsonPropertyName("max_uses")]
    public Optional<int> MaxUses { get; set; }

    [JsonPropertyName("temporary")]
    public Optional<bool> IsTemporary { get; set; }

    [JsonPropertyName("unique")]
    public Optional<bool> IsUnique { get; set; }

    [JsonPropertyName("target_type")]
    public Optional<int> TargetType { get; set; }

    [JsonPropertyName("target_user_id")]
    public Optional<ulong> TargetUserId { get; set; }

    [JsonPropertyName("target_application_id")]
    public Optional<ulong> TargetApplicationId { get; set; }
}
