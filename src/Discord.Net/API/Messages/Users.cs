//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API
{
	//Common
	public class UserReference
	{
		[JsonProperty("username")]
		public string Username;
		[JsonProperty("id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long Id;
		[JsonProperty("discriminator")]
		public short? Discriminator;
		[JsonProperty("avatar")]
		public string Avatar;
	}
	public class UserInfo : UserReference
	{
		[JsonProperty("email")]
		public string Email;
		[JsonProperty("verified")]
		public bool? IsVerified;
	}

	//Edit
	internal sealed class EditUserRequest
	{
		[JsonProperty("password")]
		public string CurrentPassword;
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email;
		[JsonProperty("new_password")]
		public string Password;
		[JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
		public string Username;
		[JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
		public string Avatar;
	}
	public sealed class EditUserResponse : UserInfo { }

	//Events
	internal sealed class UserUpdateEvent : UserInfo { }
	internal sealed class PresenceUpdateEvent : PresenceInfo { }
	internal sealed class TypingStartEvent
	{
		[JsonProperty("user_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long UserId;
		[JsonProperty("channel_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long ChannelId;
		[JsonProperty("timestamp")]
		public int Timestamp;
	}
	internal sealed class BanAddEvent : MemberReference { }
	internal sealed class BanRemoveEvent : MemberReference { }
}
