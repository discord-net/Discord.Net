using Discord.Net.Converters;
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class ModifyCurrentUserParams
    {
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }
        [JsonProperty("email")]
        public Optional<string> Email { get; set; }
        [JsonProperty("password")]
        public Optional<string> Password { get; set; }
        [JsonProperty("new_password")]
        public Optional<string> NewPassword { get; set; }
        [JsonProperty("avatar"), Image]
        public Optional<Stream> Avatar { get; set; }
    }
}
