#pragma warning disable CS1591
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;

namespace Discord.API
{
    internal class Activity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public Optional<string> StreamUrl { get; set; }
        [JsonProperty("type")]
        public Optional<ActivityType?> Type { get; set; }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }
    }
}
