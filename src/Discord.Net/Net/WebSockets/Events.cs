//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.Net.API;
using Newtonsoft.Json;

namespace Discord.Net.WebSockets
{
	internal static class Events
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
			public ExtendedGuildInfo[] Guilds;
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
		public sealed class GuildCreate : ExtendedGuildInfo { }
		public sealed class GuildUpdate : GuildInfo { }
		public sealed class GuildDelete : ExtendedGuildInfo { }

		//Channels
		public sealed class ChannelCreate : ChannelInfo { }
		public sealed class ChannelDelete : ChannelInfo { }
		public sealed class ChannelUpdate : ChannelInfo { }

		//Memberships
		public sealed class GuildMemberAdd : MemberInfo { }
		public sealed class GuildMemberUpdate : MemberInfo { }
		public sealed class GuildMemberRemove : MemberInfo { }

		//Roles
		public sealed class GuildRoleCreate
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
			[JsonProperty(PropertyName = "role")]
			public RoleInfo Data;
		}
		public sealed class GuildRoleUpdate
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
			[JsonProperty(PropertyName = "role")]
			public RoleInfo Data;
		}
		public sealed class GuildRoleDelete : RoleReference { }

		//Bans
		public sealed class GuildBanAdd : MemberReference { }
		public sealed class GuildBanRemove : MemberReference { }

		//User
		public sealed class UserUpdate : SelfUserInfo { }
		public sealed class PresenceUpdate : PresenceMemberInfo { }
		public sealed class VoiceStateUpdate : VoiceMemberInfo { }

		//Chat
		public sealed class MessageCreate : API.Message { }
		public sealed class MessageUpdate : API.Message { }
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
			public string GuildId;
			[JsonProperty(PropertyName = "endpoint")]
			public string Endpoint;
			[JsonProperty(PropertyName = "token")]
			public string Token;
		}
	}
}
