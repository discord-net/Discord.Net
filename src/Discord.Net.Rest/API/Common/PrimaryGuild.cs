using Newtonsoft.Json;

namespace Discord.API;

internal class PrimaryGuild
{
    [JsonProperty("identity_guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("identity_enabled")]
    public bool? IdentityEnabled { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("badge")]
    public string BadgeHash { get; set; }
}
