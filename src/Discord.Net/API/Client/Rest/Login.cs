using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoginRequest : IRestRequest<LoginResponse>
	{
        string IRestRequest.Method => Email != null ? "POST" : "GET";
        string IRestRequest.Endpoint => $"auth/login";
        object IRestRequest.Payload => this;

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email { get; set; }
		[JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
		public string Password { get; set; }
    }

	public class LoginResponse
	{
		[JsonProperty("token")]
		public string Token { get; set; }
    }
}
