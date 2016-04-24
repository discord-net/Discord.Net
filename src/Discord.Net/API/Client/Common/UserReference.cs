using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class UserReference
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
        public ulong Id { get; set; }
        [JsonProperty("discriminator")]
        public ushort? Discriminator { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("bot")]
        public bool? Bot { get; set; }
    }
}
