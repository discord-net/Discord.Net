//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;

namespace Discord.API
{
	//Auth
	internal sealed class RegisterRequest
	{
		[JsonProperty("fingerprint")]
		public string Fingerprint;
		[JsonProperty("username")]
		public string Username;
	}
	internal sealed class LoginRequest
	{
		[JsonProperty("email")]
		public string Email;
		[JsonProperty("password")]
		public string Password;
	}

	//Channels
	internal sealed class CreateChannelRequest
	{
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("type")]
		public string Type;
	}
	internal sealed class CreatePMChannelRequest
	{
		[JsonProperty("recipient_id")]
		public string RecipientId;
	}
	internal sealed class EditChannelRequest
	{
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name;
		[JsonProperty("topic", NullValueHandling = NullValueHandling.Ignore)]
		public string Topic;
	}
	internal sealed class ReorderChannelsRequest : IEnumerable<ReorderChannelsRequest.Channel>
	{
		public sealed class Channel
		{
			[JsonProperty("id")]
			public string Id;
			[JsonProperty("position")]
			public uint Position;
		}
		private IEnumerable<Channel> _channels;
		public ReorderChannelsRequest(IEnumerable<Channel> channels) { _channels = channels; }

		public IEnumerator<Channel> GetEnumerator() =>_channels.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _channels.GetEnumerator();
	}

	//Invites
	internal sealed class CreateInviteRequest
	{
		[JsonProperty("max_age")]
		public int MaxAge;
		[JsonProperty("max_uses")]
		public int MaxUses;
		[JsonProperty("temporary")]
		public bool IsTemporary;
		[JsonProperty("xkcdpass")]
		public bool WithXkcdPass;
	}

	//Members
	internal sealed class EditMemberRequest
	{
		[JsonProperty(PropertyName = "mute", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Mute;
		[JsonProperty(PropertyName = "deaf", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Deaf;
		[JsonProperty(PropertyName = "roles", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> Roles;
	}

	//Messages
	internal sealed class SendMessageRequest
	{
		[JsonProperty("content")]
		public string Content;
		[JsonProperty("mentions")]
		public IEnumerable<string> Mentions;
		[JsonProperty("nonce", NullValueHandling = NullValueHandling.Ignore)]
		public string Nonce;
		[JsonProperty("tts", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsTTS;
	}
	internal sealed class EditMessageRequest
	{
		[JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
		public string Content;
		[JsonProperty("mentions", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> Mentions;
	}

	//Permissions
	internal sealed class SetChannelPermissionsRequest //Both creates and modifies
	{
		[JsonProperty("id")]
		public string Id;
		[JsonProperty("type")]
		public string Type;
		[JsonProperty("allow")]
		public uint Allow;
		[JsonProperty("deny")]
		public uint Deny;
	}

	//Profile
	internal sealed class EditProfileRequest
	{
		[JsonProperty(PropertyName = "password")]
		public string CurrentPassword;
		[JsonProperty(PropertyName = "email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email;
		[JsonProperty(PropertyName = "new_password")]
		public string Password;
		[JsonProperty(PropertyName = "username", NullValueHandling = NullValueHandling.Ignore)]
		public string Username;
		[JsonProperty(PropertyName = "avatar", NullValueHandling = NullValueHandling.Ignore)]
		public string Avatar;
	}

	//Roles
	internal sealed class EditRoleRequest
	{
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name;
		[JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
		public uint? Permissions;
	}

	//Servers
	internal sealed class CreateServerRequest
	{
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("region")]
		public string Region;
	}
	internal sealed class EditServerRequest
	{
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name;
		[JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
		public string Region;
	}
}
