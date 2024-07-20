using Newtonsoft.Json;

namespace Discord.API;

internal class Emoji
{
    [JsonProperty("id")]
    public ulong? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("animated")]
    public Optional<bool> Animated { get; set; }

    [JsonProperty("roles")]
    public Optional<ulong[]> Roles { get; set; }

    [JsonProperty("require_colons")]
    public Optional<bool> RequireColons { get; set; }

    [JsonProperty("managed")]
    public Optional<bool> Managed { get; set; }

    [JsonProperty("user")]
    public Optional<User> User { get; set; }

    [JsonProperty("available")]
    public Optional<bool> IsAvailable { get; set; }
}
