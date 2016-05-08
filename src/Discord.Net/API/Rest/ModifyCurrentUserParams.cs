using Discord.Net.Converters;
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class ModifyCurrentUserParams
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
        [JsonProperty("avatar"), JsonConverter(typeof(ImageConverter))]
        public Stream Avatar { get; set; }
    }
}
