using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class ModifyCurrentUserParams
    {
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }
        [JsonProperty("avatar"), Image]
        public Optional<Stream> Avatar { get; set; }
    }
}
