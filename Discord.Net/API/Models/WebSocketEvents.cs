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

		internal sealed class GuildCreate : ExtendedServerInfo { }
		internal sealed class GuildDelete : ExtendedServerInfo { }

		internal sealed class ChannelCreate : ChannelInfo { }
		internal sealed class ChannelDelete : ChannelInfo { }
		internal sealed class ChannelUpdate : ChannelInfo { }

		internal sealed class GuildMemberAdd : GuildMemberUpdate
		{
			[JsonProperty(PropertyName = "joined_at")]
			public DateTime JoinedAt;
		}
		internal class GuildMemberUpdate
		{
			[JsonProperty(PropertyName = "user")]
			public UserInfo User;
			[JsonProperty(PropertyName = "roles")]
			public object[] Roles;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}
		internal sealed class GuildMemberRemove
		{
			[JsonProperty(PropertyName = "user")]
			public UserInfo User;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}

		internal sealed class GuildRoleCreateUpdate
		{
			[JsonProperty(PropertyName = "role")]
			public Role Role;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}
		internal sealed class GuildRoleDelete
		{
			[JsonProperty(PropertyName = "role_id")]
			public string RoleId;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}

		internal sealed class GuildBanAddRemove
		{
			[JsonProperty(PropertyName = "user")]
			public UserInfo User;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}
		internal sealed class GuildBanRemove
		{
			[JsonProperty(PropertyName = "user_id")]
			public string UserId;
			[JsonProperty(PropertyName = "guild_id")]
			public string GuildId;
		}

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
		internal sealed class MessageCreate
		{
			[JsonProperty(PropertyName = "id")]
			public string Id;
			[JsonProperty(PropertyName = "channel_id")]
			public string ChannelId;
			[JsonProperty(PropertyName = "tts")]
			public bool IsTextToSpeech;
			[JsonProperty(PropertyName = "mention_everyone")]
			public bool IsMentioningEveryone;
			[JsonProperty(PropertyName = "timestamp")]
			public DateTime Timestamp;
			[JsonProperty(PropertyName = "mentions")]
			public UserInfo[] Mentions;
			[JsonProperty(PropertyName = "embeds")]
			public object[] Embeds;
			[JsonProperty(PropertyName = "attachments")]
			public object[] Attachments;
			[JsonProperty(PropertyName = "content")]
			public string Content;
			[JsonProperty(PropertyName = "author")]
			public UserInfo Author;
		}
		internal sealed class MessageUpdate
		{
			[JsonProperty(PropertyName = "id")]
			public string Id;
			[JsonProperty(PropertyName = "channel_id")]
			public string ChannelId;
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
	}
}
