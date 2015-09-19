//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.Net.API
{
	internal static class Requests
	{
		//Auth
		public sealed class AuthRegister
		{
			[JsonProperty(PropertyName = "fingerprint")]
			public string Fingerprint;
			[JsonProperty(PropertyName = "username")]
			public string Username;
		}
		public sealed class AuthLogin
		{
			[JsonProperty(PropertyName = "email")]
			public string Email;
			[JsonProperty(PropertyName = "password")]
			public string Password;
		}

		//Servers
		public sealed class CreateServer
		{
			[JsonProperty(PropertyName = "name")]
			public string Name;
			[JsonProperty(PropertyName = "region")]
			public string Region;
		}

		//Channels
		public sealed class CreateChannel
		{
			[JsonProperty(PropertyName = "name")]
			public string Name;
			[JsonProperty(PropertyName = "type")]
			public string Type;
		}
		public sealed class CreatePMChannel
		{
			[JsonProperty(PropertyName = "recipient_id")]
			public string RecipientId;
		}

		//Invites
		public sealed class CreateInvite
		{
			[JsonProperty(PropertyName = "max_age")]
			public int MaxAge;
			[JsonProperty(PropertyName = "max_uses")]
			public int MaxUses;
			[JsonProperty(PropertyName = "temporary")]
			public bool IsTemporary;
			[JsonProperty(PropertyName = "xkcdpass")]
			public bool WithXkcdPass;
		}

		//Messages
		public sealed class SendMessage
		{
			[JsonProperty(PropertyName = "content")]
			public string Content;
			[JsonProperty(PropertyName = "mentions")]
			public string[] Mentions;
			[JsonProperty(PropertyName = "nonce")]
			public string Nonce;
		}
		public sealed class EditMessage
		{
			[JsonProperty(PropertyName = "content")]
			public string Content;
			[JsonProperty(PropertyName = "mentions")]
			public string[] Mentions;
		}

		//Members
		public sealed class SetMemberMute
		{
			[JsonProperty(PropertyName = "mute")]
			public bool Value;
		}
		public sealed class SetMemberDeaf
		{
			[JsonProperty(PropertyName = "deaf")]
			public bool Value;
		}
		public sealed class ModifyMember
		{
			[JsonProperty(PropertyName = "roles")]
			public string[] Roles;
		}
		
		//Profile
		public sealed class ChangeUsername
		{
			[JsonProperty(PropertyName = "email")]
			public string CurrentEmail;
			[JsonProperty(PropertyName = "password")]
			public string CurrentPassword;
			[JsonProperty(PropertyName = "username")]
			public string Username;
		}
		public sealed class ChangeEmail
		{
			[JsonProperty(PropertyName = "email")]
			public string NewEmail;
			[JsonProperty(PropertyName = "password")]
			public string CurrentPassword;
		}
		public sealed class ChangePassword
		{
			[JsonProperty(PropertyName = "email")]
			public string CurrentEmail;
			[JsonProperty(PropertyName = "password")]
			public string CurrentPassword;
			[JsonProperty(PropertyName = "new_password")]
			public string NewPassword;
		}
		public sealed class ChangeAvatar
		{
			[JsonProperty(PropertyName = "email")]
			public string CurrentEmail;
			[JsonProperty(PropertyName = "password")]
			public string CurrentPassword;
			[JsonProperty(PropertyName = "avatar")]
			public string Avatar;
		}

		//Roles
		public sealed class ModifyRole
		{
			[JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
			public string Name;
			[JsonProperty(PropertyName = "permissions", NullValueHandling = NullValueHandling.Ignore)]
			public uint Permissions;
		}

		//Permissions
		public sealed class SetChannelPermissions
		{
			[JsonProperty(PropertyName = "id")]
			public string Id;
			[JsonProperty(PropertyName = "type")]
			public string Type;
			[JsonProperty(PropertyName = "allow")]
			public uint Allow;
			[JsonProperty(PropertyName = "deny")]
			public uint Deny;
		}
	}
}
