using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildParams
    {
        [JsonPropertyName("name")]
        public string Name { get; }
        [JsonPropertyName("region")]
        public string RegionId { get; }

        [JsonPropertyName("icon")]
        public Optional<Image?> Icon { get; set; }

        public CreateGuildParams(string name, string regionId)
        {
            Name = name;
            RegionId = regionId;
        }
    }
}
