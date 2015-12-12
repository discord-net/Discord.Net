//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API
{
	//Common
	public class MemberReference
	{
		[JsonProperty("user_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long UserId; //Used in bans

		[JsonProperty("guild_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long GuildId;
        
		private UserReference _user;
        [JsonProperty("user")]
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
		[JsonConverter(typeof(LongStringArrayConverter))]
		public long[] Roles;
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
		public int? GameId;
		[JsonProperty("status")]
		public string Status;
		[JsonProperty("roles")] //TODO: Might be temporary
		[JsonConverter(typeof(LongStringArrayConverter))]
		public long[] Roles;
	}
	public class VoiceMemberInfo : MemberReference
	{
		[JsonProperty("channel_id")]
		[JsonConverter(typeof(NullableLongStringConverter))]
		public long? ChannelId;
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
        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(NullableLongStringConverter))]
        public long? ChannelId;
        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(LongStringEnumerableConverter))]
		public IEnumerable<long> Roles;
	}

	public class PruneUsersResponse
	{
		[JsonProperty("pruned")]
		public int? Pruned;
	}

	//Events
	internal sealed class MemberAddEvent : MemberInfo { }
	internal sealed class MemberUpdateEvent : MemberInfo { }
	internal sealed class MemberRemoveEvent : MemberInfo { }
    internal sealed class MemberVoiceStateUpdateEvent : VoiceMemberInfo { }
	internal sealed class MembersChunkEvent
	{
		[JsonProperty("members")]
		public MemberInfo[] Members;
	}
}
