using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class User : UserReference
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("verified")]
        public bool? IsVerified { get; set; }
    }
}
