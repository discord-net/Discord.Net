using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API;

public class RoleConnectionMetadata
{
    [JsonProperty("type")]
    public RoleConnectionMetadataType Type { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("name_localizations")]
    public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

    [JsonProperty("description_localizations")]
    public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }
}
