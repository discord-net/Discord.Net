using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
