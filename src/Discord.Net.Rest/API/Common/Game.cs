#pragma warning disable CS1591
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;

namespace Discord.API
{
    internal class Game
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public Optional<string> StreamUrl { get; set; }
        [JsonProperty("type")]
        public Optional<ActivityType?> Type { get; set; }
        [JsonProperty("details")]
        public Optional<string> Details { get; set; }
        [JsonProperty("state")]
        public Optional<string> State { get; set; }
        [JsonProperty("application_id")]
        public Optional<ulong> ApplicationId { get; set; }
        [JsonProperty("assets")]
        public Optional<API.GameAssets> Assets { get; set; }
        [JsonProperty("party")]
        public Optional<API.GameParty> Party { get; set; }
        [JsonProperty("secrets")]
        public Optional<API.GameSecrets> Secrets { get; set; }
        [JsonProperty("timestamps")]
        public Optional<API.GameTimestamps> Timestamps { get; set; }
        [JsonProperty("instance")]
        public Optional<bool> Instance { get; set; }
        [JsonProperty("sync_id")]
        public Optional<string> SyncId { get; set; }
        [JsonProperty("session_id")]
        public Optional<string> SessionId { get; set; }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }
    }
}
