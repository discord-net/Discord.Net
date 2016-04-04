using Discord.Net.JsonConverters;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyCurrentUserRequest : IRestRequest<User>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"users/@me";
        object IRestRequest.Payload => this;

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
