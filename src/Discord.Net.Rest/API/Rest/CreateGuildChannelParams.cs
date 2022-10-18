using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildChannelParams
    {
        [JsonPropertyName("name")]
        public string Name { get; }
        [JsonPropertyName("type")]
        public ChannelType Type { get; }
        [JsonPropertyName("parent_id")]
        public Optional<ulong?> CategoryId { get; set; }
        [JsonPropertyName("position")]
        public Optional<int> Position { get; set; }
        [JsonPropertyName("permission_overwrites")]
        public Optional<Overwrite[]> Overwrites { get; set; }

        //Text channels
        [JsonPropertyName("topic")]
        public Optional<string> Topic { get; set; }
        [JsonPropertyName("nsfw")]
        public Optional<bool> IsNsfw { get; set; }
        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int> SlowModeInterval { get; set; }

        //Voice channels
        [JsonPropertyName("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonPropertyName("user_limit")]
        public Optional<int?> UserLimit { get; set; }

        public CreateGuildChannelParams(string name, ChannelType type)
        {
            Name = name;
            Type = type;
        }
    }
}
