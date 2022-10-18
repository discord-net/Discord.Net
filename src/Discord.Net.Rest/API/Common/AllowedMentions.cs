using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class AllowedMentions
    {
        [JsonPropertyName("parse")]
        public Optional<string[]> Parse { get; set; }
        // Roles and Users have a max size of 100
        [JsonPropertyName("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonPropertyName("users")]
        public Optional<ulong[]> Users { get; set; }
        [JsonPropertyName("replied_user")]
        public Optional<bool> RepliedUser { get; set; }
    }
}
