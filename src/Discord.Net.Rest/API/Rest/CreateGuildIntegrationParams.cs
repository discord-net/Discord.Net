#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildIntegrationParams
    {
        [JsonProperty("id")]
        public ulong Id { get; }
        [JsonProperty("type")]
        public string Type { get; }

        public CreateGuildIntegrationParams(ulong id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
