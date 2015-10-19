//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API
{
	//Common
	public class MemberReference
	{
		[JsonProperty("user_id")]
		public string UserId;
		[JsonProperty("guild_id")]
		public string GuildId;

		[JsonProperty("user")]
		private UserReference _user;
		public UserReference User
		{
			get { return _user; }
			set
			{
				_user = value;
				UserId = User.Id;
			}
		}
	}
	public class MemberInfo : MemberReference
	{
		[JsonProperty("joined_at")]
		public DateTime? JoinedAt;
		[JsonProperty("roles")]
		public string[] Roles;
	}
	public class ExtendedMemberInfo : MemberInfo
	{
		[JsonProperty("mute")]
		public bool? IsServerMuted;
		[JsonProperty("deaf")]
		public bool? IsServerDeafened;
	}
	public class PresenceInfo : MemberReference
	{
		[JsonProperty("game_id")]
		public string GameId;
		[JsonProperty("status")]
		public string Status;
	}
	public class VoiceMemberInfo : MemberReference
	{
		[JsonProperty("channel_id")]
		public string ChannelId;
		[JsonProperty("session_id")]
		public string SessionId;
		[JsonProperty("token")]
		public string Token;

		[JsonProperty("self_mute")]
		public bool? IsSelfMuted;
		[JsonProperty("self_deaf")]
		public bool? IsSelfDeafened;
		[JsonProperty("mute")]
		public bool? IsServerMuted;
		[JsonProperty("deaf")]
		public bool? IsServerDeafened;
		[JsonProperty("suppress")]
		public bool? IsServerSuppressed;
	}

	public class EditMemberRequest
	{
		[JsonProperty("mute", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Mute;
		[JsonProperty("deaf", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Deaf;
		[JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> Roles;
	}

	//Events
	internal sealed class MemberAddEvent : MemberInfo { }
	internal sealed class MemberUpdateEvent : MemberInfo { }
	internal sealed class MemberRemoveEvent : MemberInfo { }
	internal sealed class MemberVoiceStateUpdateEvent : VoiceMemberInfo { }
}
