#pragma warning disable CS1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class Connection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("revoked")]
        public bool Revoked { get; set; }

        [JsonProperty("integrations")]
        public IReadOnlyCollection<ulong> Integrations { get; set; }
    }
}
