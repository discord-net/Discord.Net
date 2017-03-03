#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildParams
    {
        [JsonProperty("name")]
        public string Name { get; }
        [JsonProperty("region")]
        public string RegionId { get; }

        [JsonProperty("icon")]
        public Optional<Image?> Icon { get; set; }

        public CreateGuildParams(string name, string regionId)
        {
            Name = name;
            RegionId = regionId;
        }
    }
}
