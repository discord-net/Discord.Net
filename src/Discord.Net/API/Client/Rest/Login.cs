using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class LoginRequest : IRestRequest<LoginResponse>
	{
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"auth/login";
        object IRestRequest.Payload => this;
        bool IRestRequest.IsPrivate => false;

        [JsonProperty("email")]
		public string Email { get; set; }
		[JsonProperty("password")]
		public string Password { get; set; }
    }

	public sealed class LoginResponse
	{
		[JsonProperty("token")]
		public string Token { get; set; }
    }
}
