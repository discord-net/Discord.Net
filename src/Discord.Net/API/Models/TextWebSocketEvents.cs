//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.API.Models
{
	internal static class TextWebSocketEvents
	{
		public sealed class Ready
		{
			public sealed class ReadStateInfo
			{
				[JsonProperty(PropertyName = "id")]
				public string ChannelId;
				[JsonProperty(PropertyName = "mention_count")]
				public int MentionCount;
				[JsonProperty(PropertyName = "last_message_id")]
				public string LastMessageId;
			}

			[JsonProperty(PropertyName = "v")]
			public int Version;
			[JsonProperty(PropertyName = "user")]
			public SelfUserInfo User;
			[JsonProperty(PropertyName = "session_id")]
			public string SessionId;
			[JsonProperty(PropertyName = "read_state")]
			public ReadStateInfo[] ReadState;
			[JsonProperty(PropertyName = "guilds")]
			public ExtendedServerInfo[] Guilds;
			[JsonProperty(PropertyName = "private_channels")]
			public ChannelInfo[] PrivateChannels;
			[JsonProperty(PropertyName = "heartbeat_interval")]
			public int HeartbeatInterval;
		}

		public sealed class Redirect
		{
			[JsonProperty(PropertyName = "url")]
			public string Url;
		}

		//Servers
		public sealed class GuildCreate : ExtendedServerInfo { }
		public sealed class GuildUpdate : ServerInfo { }
		public sealed class GuildDelete : ExtendedServerInfo { }

		//Channels
		public sealed class ChannelCreate : ChannelInfo { }
		public sealed class ChannelDelete : ChannelInfo { }
		public sealed class ChannelUpdate : ChannelInfo { }

		//Memberships
		public sealed class GuildMemberAdd : RoleMemberInfo { }
		public sealed class GuildMemberUpdate : RoleMemberInfo { }
		public sealed class GuildMemberRemove : MemberInfo { }

		//Roles
		public abstract class GuildRoleEvent
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string ServerId;
		}
		public sealed class GuildRoleCreateUpdate : GuildRoleEvent
		{
			[JsonProperty(PropertyName = "role")]
			public Role Role;
		}
		public sealed class GuildRoleDelete : GuildRoleEvent
		{
			[JsonProperty(PropertyName = "role_id")]
			public string RoleId;
		}

		//Bans
		public abstract class GuildBanEvent
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string ServerId;
		}
		public sealed class GuildBanAddRemove : GuildBanEvent
		{
			[JsonProperty(PropertyName = "user")]
			public UserReference User;
		}
		public sealed class GuildBanRemove : GuildBanEvent
		{
			[JsonProperty(PropertyName = "user_id")]
			public string UserId;
		}

		//User
		public sealed class UserUpdate : SelfUserInfo { }
		public sealed class PresenceUpdate : PresenceMemberInfo { }
		public sealed class VoiceStateUpdate : VoiceMemberInfo { }

		//Chat
		public sealed class MessageCreate : Message { }
		public sealed class MessageUpdate : Message { }
		public sealed class MessageDelete : MessageReference { }
		public sealed class MessageAck : MessageReference { }
		public sealed class TypingStart
		{
			[JsonProperty(PropertyName = "user_id")]
			public string UserId;
			[JsonProperty(PropertyName = "channel_id")]
			public string ChannelId;
			[JsonProperty(PropertyName = "timestamp")]
			public int Timestamp;
		}

		//Voice
		public sealed class VoiceServerUpdate
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string ServerId;
			[JsonProperty(PropertyName = "endpoint")]
			public string Endpoint;
			[JsonProperty(PropertyName = "token")]
			public string Token;
		}
	}
}
