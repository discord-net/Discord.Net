//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.API.Models
{
	public class AuthFingerprintResponse
	{
		[JsonProperty(PropertyName = "fingerprint")]
		public string Fingerprint;
	}

	public class AuthRegisterRequest
	{
		[JsonProperty(PropertyName = "fingerprint")]
		public string Fingerprint;
		[JsonProperty(PropertyName = "username")]
		public string Username;
	}
	public class AuthRegisterResponse : AuthLoginResponse { }

	public class AuthLoginRequest
	{
		[JsonProperty(PropertyName = "email")]
		public string Email;
		[JsonProperty(PropertyName = "password")]
		public string Password;
	}
	public class AuthLoginResponse
	{
		[JsonProperty(PropertyName = "token")]
		public string Token;
	}

	public class CreateServerRequest
	{
		[JsonProperty(PropertyName = "name")]
		public string Name;
		[JsonProperty(PropertyName = "region")]
		public string Region;
	}

	public class GetInviteResponse
	{
		[JsonProperty(PropertyName = "inviter")]
		public UserInfo Inviter;
		[JsonProperty(PropertyName = "guild")]
		public ServerInfo Server;
		[JsonProperty(PropertyName = "channel")]
		public ChannelInfo Channel;
		[JsonProperty(PropertyName = "code")]
		public string Code;
		[JsonProperty(PropertyName = "xkcdpass")]
		public string XkcdPass;
	}

	public class SendMessageRequest
	{
		[JsonProperty(PropertyName = "content")]
		public string Content;
		[JsonProperty(PropertyName = "mentions")]
		public string[] Mentions;
	}
}
