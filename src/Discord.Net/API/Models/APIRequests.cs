//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.API.Models
{
	internal static class APIRequests
	{
		public class AuthRegisterRequest
		{
			[JsonProperty(PropertyName = "fingerprint")]
			public string Fingerprint;
			[JsonProperty(PropertyName = "username")]
			public string Username;
		}
		public class AuthLogin
		{
			[JsonProperty(PropertyName = "email")]
			public string Email;
			[JsonProperty(PropertyName = "password")]
			public string Password;
		}

		public class CreateServer
		{
			[JsonProperty(PropertyName = "name")]
			public string Name;
			[JsonProperty(PropertyName = "region")]
			public string Region;
		}

		public class CreateChannel
		{
			[JsonProperty(PropertyName = "name")]
			public string Name;
			[JsonProperty(PropertyName = "type")]
			public string Type;
		}
		public class CreatePMChannel
		{
			[JsonProperty(PropertyName = "recipient_id")]
			public string RecipientId;
		}

		public class CreateInvite
		{
			[JsonProperty(PropertyName = "max_age")]
			public int MaxAge;
			[JsonProperty(PropertyName = "max_uses")]
			public int MaxUses;
			[JsonProperty(PropertyName = "temporary")]
			public bool IsTemporary;
			[JsonProperty(PropertyName = "xkcdpass")]
			public bool HasXkcdPass;
		}

		public class SendMessage
		{
			[JsonProperty(PropertyName = "content")]
			public string Content;
			[JsonProperty(PropertyName = "mentions")]
			public string[] Mentions;
		}
		public class EditMessage : SendMessage { }

		public class SetMemberMute
		{
			[JsonProperty(PropertyName = "mute")]
			public bool Mute;
		}
		public class SetMemberDeaf
		{
			[JsonProperty(PropertyName = "deaf")]
			public bool Deaf;
		}

		public class ChangeUsername
		{
			[JsonProperty(PropertyName = "username")]
			public string Username;
			[JsonProperty(PropertyName = "email")]
			public string CurrentEmail;
			[JsonProperty(PropertyName = "password")]
			public string CurrentPassword;
		}
		public class ChangeEmail
		{
			[JsonProperty(PropertyName = "email")]
			public string Email;
			[JsonProperty(PropertyName = "password")]
			public string CurrentPassword;
		}
		public class ChangePassword
		{
			[JsonProperty(PropertyName = "new_password")]
			public string NewPassword;
			[JsonProperty(PropertyName = "email")]
			public string CurrentEmail;
			[JsonProperty(PropertyName = "password")]
			public string CurrentPassword;
		}
	}
}
