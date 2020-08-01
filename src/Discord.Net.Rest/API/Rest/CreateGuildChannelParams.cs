#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildChannelParams
    {
        [JsonProperty("name")]
        public string Name { get; }
        [JsonProperty("type")]
        public ChannelType Type { get; }
        [JsonProperty("parent_id")]
        public Optional<ulong?> CategoryId { get; set; }
        [JsonProperty("position")]
        public Optional<int> Position { get; set; }
        [JsonProperty("permission_overwrites")]
        public Optional<Overwrite[]> Overwrites { get; set; }

        //Text channels
        [JsonProperty("topic")]
        public Optional<string> Topic { get; set; }
        [JsonProperty("nsfw")]
        public Optional<bool> IsNsfw { get; set; }
        [JsonProperty("rate_limit_per_user")]
        public Optional<int> SlowModeInterval { get; set; }

        //Voice channels
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public Optional<int?> UserLimit { get; set; }

        public CreateGuildChannelParams(string name, ChannelType type)
        {
            Name = name;
            Type = type;
        }
    }
}
