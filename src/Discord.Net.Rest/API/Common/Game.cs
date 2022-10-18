using System.Text.Json.Serialization;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;

namespace Discord.API
{
    internal class Game
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("url")]
        public Optional<string> StreamUrl { get; set; }
        [JsonPropertyName("type")]
        public Optional<ActivityType?> Type { get; set; }
        [JsonPropertyName("details")]
        public Optional<string> Details { get; set; }
        [JsonPropertyName("state")]
        public Optional<string> State { get; set; }
        [JsonPropertyName("application_id")]
        public Optional<ulong> ApplicationId { get; set; }
        [JsonPropertyName("assets")]
        public Optional<API.GameAssets> Assets { get; set; }
        [JsonPropertyName("party")]
        public Optional<API.GameParty> Party { get; set; }
        [JsonPropertyName("secrets")]
        public Optional<API.GameSecrets> Secrets { get; set; }
        [JsonPropertyName("timestamps")]
        public Optional<API.GameTimestamps> Timestamps { get; set; }
        [JsonPropertyName("instance")]
        public Optional<bool> Instance { get; set; }
        [JsonPropertyName("sync_id")]
        public Optional<string> SyncId { get; set; }
        [JsonPropertyName("session_id")]
        public Optional<string> SessionId { get; set; }
        [JsonPropertyName("Flags")]
        public Optional<ActivityProperties> Flags { get; set; }
        [JsonPropertyName("id")]
        public Optional<string> Id { get; set; }
        [JsonPropertyName("emoji")]
        public Optional<Emoji> Emoji { get; set; }
        [JsonPropertyName("created_at")]
        public Optional<long> CreatedAt { get; set; }
        //[JsonPropertyName("buttons")]
        //public Optional<RichPresenceButton[]> Buttons { get; set; }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }
    }
}
