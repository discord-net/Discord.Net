using Newtonsoft.Json;

namespace Discord.API;

internal class InstallParams
{
    [JsonProperty("scopes")]
    public string[] Scopes { get; set; }

    [JsonProperty("permissions")]
    public GuildPermission Permission { get; set; }
}
