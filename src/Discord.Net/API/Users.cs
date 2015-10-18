//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.API
{
	//Common
	public class UserReference
	{
		[JsonProperty("username")]
		public string Username;
		[JsonProperty("id")]
		public string Id;
		[JsonProperty("discriminator")]
		public string Discriminator;
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
	public sealed class EditUserResponse : UserInfo { }

	//Events
	internal sealed class UserUpdateEvent : UserInfo { }
}
