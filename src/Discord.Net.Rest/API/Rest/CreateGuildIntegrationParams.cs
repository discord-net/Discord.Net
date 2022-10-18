using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildIntegrationParams
    {
        [JsonPropertyName("id")]
        public ulong Id { get; }
        [JsonPropertyName("type")]
        public string Type { get; }

        public CreateGuildIntegrationParams(ulong id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
