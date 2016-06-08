using Newtonsoft.Json;

namespace Discord.API
{
    public class User
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("verified")]
        public bool IsVerified { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("bot")]
        public bool Bot { get; set; }
    }
}
