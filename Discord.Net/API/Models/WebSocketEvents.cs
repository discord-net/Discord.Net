//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;

namespace Discord.API.Models
{
	internal static class WebSocketEvents
	{
		public sealed class Ready
		{
			[JsonProperty(PropertyName = "user")]
			public SelfUserInfo User;
			[JsonProperty(PropertyName = "session_id")]
			public string SessionId;
			[JsonProperty(PropertyName = "read_state")]
			public object[] ReadState;
			[JsonProperty(PropertyName = "guilds")]
			public ExtendedServerInfo[] Guilds;
			[JsonProperty(PropertyName = "private_channels")]
			public ChannelInfo[] PrivateChannels;
			[JsonProperty(PropertyName = "heartbeat_interval")]
			public int HeartbeatInterval;
		}

		//Servers
		public sealed class GuildCreate : ExtendedServerInfo { }
		public sealed class GuildUpdate : ExtendedServerInfo { }
		public sealed class GuildDelete : ExtendedServerInfo { }

		//Channels
		public sealed class ChannelCreate : ChannelInfo { }
		public sealed class ChannelDelete : ChannelInfo { }
		public sealed class ChannelUpdate : ChannelInfo { }

		//Memberships
		public abstract class GuildMemberEvent
		{
			[JsonProperty(PropertyName = "user")]
			public UserReference User;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}
		public sealed class GuildMemberAdd : GuildMemberEvent
		{
			[JsonProperty(PropertyName = "joined_at")]
			public DateTime JoinedAt;
			[JsonProperty(PropertyName = "roles")]
			public object[] Roles;
		}
		public sealed class GuildMemberUpdate : GuildMemberEvent
		{
			[JsonProperty(PropertyName = "roles")]
			public object[] Roles;
		}
		public sealed class GuildMemberRemove : GuildMemberEvent { }

		//Roles
		public abstract class GuildRoleEvent
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
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
			public string GuildId;
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
		public sealed class PresenceUpdate : PresenceUserInfo { }
		public sealed class VoiceStateUpdate
		{
			[JsonProperty(PropertyName = "user_id")]
			public string UserId;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
			[JsonProperty(PropertyName = "channel_id")]
			public string ChannelId;
			[JsonProperty(PropertyName = "suppress")]
			public bool IsSuppressed;
			[JsonProperty(PropertyName = "session_id")]
			public string SessionId;
			[JsonProperty(PropertyName = "self_mute")]
			public bool IsSelfMuted;
			[JsonProperty(PropertyName = "self_deaf")]
			public bool IsSelfDeafened;
			[JsonProperty(PropertyName = "mute")]
			public bool IsMuted;
			[JsonProperty(PropertyName = "deaf")]
			public bool IsDeafened;
		}

		//Chat
		public sealed class MessageCreate : Message { }
		public sealed class MessageUpdate : MessageReference
		{
			[JsonProperty(PropertyName = "embeds")]
			public object[] Embeds;
		}
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
		}
	}
}
