using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class GameParty
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("size")]
        public long[] Size { get; set; }
    }
}
