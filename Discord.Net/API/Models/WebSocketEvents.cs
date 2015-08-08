//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;

namespace Discord.API.Models
{
	internal static class WebSocketEvents
	{
		internal sealed class Ready
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
		internal sealed class GuildCreate : ExtendedServerInfo { }
		internal sealed class GuildDelete : ExtendedServerInfo { }

		//Channels
		internal sealed class ChannelCreate : ChannelInfo { }
		internal sealed class ChannelDelete : ChannelInfo { }
		internal sealed class ChannelUpdate : ChannelInfo { }

		//Memberships
		internal abstract class GuildMemberEvent
		{
			[JsonProperty(PropertyName = "user")]
			public UserReference User;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}
		internal sealed class GuildMemberAdd : GuildMemberEvent
		{
			[JsonProperty(PropertyName = "joined_at")]
			public DateTime JoinedAt;
			[JsonProperty(PropertyName = "roles")]
			public object[] Roles;
		}
		internal sealed class GuildMemberUpdate : GuildMemberEvent
		{
			[JsonProperty(PropertyName = "roles")]
			public object[] Roles;
		}
		internal sealed class GuildMemberRemove : GuildMemberEvent { }

		//Roles
		internal abstract class GuildRoleEvent
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}
		internal sealed class GuildRoleCreateUpdate : GuildRoleEvent
		{
			[JsonProperty(PropertyName = "role")]
			public Role Role;
		}
		internal sealed class GuildRoleDelete : GuildRoleEvent
		{
			[JsonProperty(PropertyName = "role_id")]
			public string RoleId;
		}

		//Bans
		internal abstract class GuildBanEvent
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}
		internal sealed class GuildBanAddRemove : GuildBanEvent
		{
			[JsonProperty(PropertyName = "user")]
			public UserReference User;
		}
		internal sealed class GuildBanRemove : GuildBanEvent
		{
			[JsonProperty(PropertyName = "user_id")]
			public string UserId;
		}

		//User
		internal sealed class UserUpdate : SelfUserInfo { }
		internal sealed class PresenceUpdate : PresenceUserInfo { }
		internal sealed class VoiceStateUpdate
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
		internal sealed class MessageCreate : Message { }
		internal sealed class MessageUpdate : MessageReference
		{
			[JsonProperty(PropertyName = "embeds")]
			public object[] Embeds;
		}
		internal sealed class MessageDelete : MessageReference { }
		internal sealed class MessageAck : MessageReference { }
		internal sealed class TypingStart
		{
			[JsonProperty(PropertyName = "user_id")]
			public string UserId;
			[JsonProperty(PropertyName = "channel_id")]
			public string ChannelId;
			[JsonProperty(PropertyName = "timestamp")]
			public int Timestamp;
		}

		//Voice
		internal sealed class VoiceServerUpdate
		{
			[JsonProperty(PropertyName = "guild_id")]
			public string ServerId;
			[JsonProperty(PropertyName = "endpoint")]
			public string Endpoint;
		}
	}
}
