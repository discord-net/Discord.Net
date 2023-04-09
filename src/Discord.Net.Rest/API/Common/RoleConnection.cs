using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API;

public class RoleConnection
{
    [JsonProperty("platform_name")]
    public Optional<string> PlatformName { get; set; }

    [JsonProperty("platform_username")]
    public Optional<string> PlatformUsername { get; set; }

    [JsonProperty("metadata")]
    public Optional<Dictionary<string, string>> Metadata { get; set; }
}
