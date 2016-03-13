using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateProfileRequest : IRestRequest<User>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"users/@me";
        object IRestRequest.Payload => this;

        [JsonProperty("password")]
        public string CurrentPassword { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("new_password")]
        public string Password { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("avatar")]
        public string AvatarBase64 { get; set; }
    }
}
