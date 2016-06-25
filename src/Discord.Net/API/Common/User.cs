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
        [JsonProperty("bot")]
        public bool Bot { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        //CurrentUser
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("mfa_enabled")]
        public bool MfaEnabled { get; set; }
    }
}
