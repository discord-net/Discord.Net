//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.API
{
	//Gateway
	public class GatewayResponse
	{
		[JsonProperty("url")]
		public string Url;
	}
	
	//Login
	public sealed class LoginRequest
	{
		[JsonProperty("email")]
		public string Email;
		[JsonProperty("password")]
		public string Password;
	}
	public sealed class LoginResponse
	{
		[JsonProperty("token")]
		public string Token;
	}
}
