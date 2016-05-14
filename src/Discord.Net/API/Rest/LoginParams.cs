using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class LoginParams
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
