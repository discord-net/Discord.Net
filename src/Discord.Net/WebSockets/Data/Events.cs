//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API;
using Newtonsoft.Json;

namespace Discord.WebSockets.Data
{
	internal sealed class ReadyEvent
	{
		public sealed class ReadStateInfo
		{
			[JsonProperty("id")]
			public string ChannelId;
			[JsonProperty("mention_count")]
			public int MentionCount;
			[JsonProperty("last_message_id")]
			public string LastMessageId;
		}

		[JsonProperty("v")]
		public int Version;
		[JsonProperty("user")]
		public SelfUserInfo User;
		[JsonProperty("session_id")]
		public string SessionId;
		[JsonProperty("read_state")]
		public ReadStateInfo[] ReadState;
		[JsonProperty("guilds")]
		public ExtendedGuildInfo[] Guilds;
		[JsonProperty("private_channels")]
		public ChannelInfo[] PrivateChannels;
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval;
	}
	internal sealed class ResumedEvent
	{
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval;
	}

	internal sealed class RedirectEvent
	{
		[JsonProperty("url")]
		public string Url;
	}

	//Servers
	internal sealed class GuildCreateEvent : ExtendedGuildInfo { }
	internal sealed class GuildUpdateEvent : GuildInfo { }
	internal sealed class GuildDeleteEvent : ExtendedGuildInfo { }

	//Channels
	internal sealed class ChannelCreateEvent : ChannelInfo { }
	internal sealed class ChannelDeleteEvent : ChannelInfo { }
	internal sealed class ChannelUpdateEvent : ChannelInfo { }

	//Memberships
	internal sealed class GuildMemberAddEvent : MemberInfo { }
	internal sealed class GuildMemberUpdateEvent : MemberInfo { }
	internal sealed class GuildMemberRemoveEvent : MemberInfo { }

	//Roles
	internal sealed class GuildRoleCreateEvent
	{
		[JsonProperty("guild_id")]
		public string GuildId;
		[JsonProperty("role")]
		public RoleInfo Data;
	}
	internal sealed class GuildRoleUpdateEvent
	{
		[JsonProperty("guild_id")]
		public string GuildId;
		[JsonProperty("role")]
		public RoleInfo Data;
	}
	internal sealed class GuildRoleDeleteEvent : RoleReference { }

	//Bans
	internal sealed class GuildBanAddEvent : MemberReference { }
	internal sealed class GuildBanRemoveEvent : MemberReference { }

	//User
	internal sealed class UserUpdateEvent : SelfUserInfo { }
	internal sealed class PresenceUpdateEvent : PresenceMemberInfo { }
	internal sealed class VoiceStateUpdateEvent : VoiceMemberInfo { }

	//Chat
	internal sealed class MessageCreateEvent : API.Message { }
	internal sealed class MessageUpdateEvent : API.Message { }
	internal sealed class MessageDeleteEvent : MessageReference { }
	internal sealed class MessageAckEvent : MessageReference { }
	internal sealed class TypingStartEvent
	{
		[JsonProperty("user_id")]
		public string UserId;
		[JsonProperty("channel_id")]
		public string ChannelId;
		[JsonProperty("timestamp")]
		public int Timestamp;
	}

	//Voice
	internal sealed class VoiceServerUpdateEvent
	{
		[JsonProperty("guild_id")]
		public string GuildId;
		[JsonProperty("endpoint")]
		public string Endpoint;
		[JsonProperty("token")]
		public string Token;
	}
}
