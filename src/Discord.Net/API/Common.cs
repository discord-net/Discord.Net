//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;

namespace Discord.API
{
	//User
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
	public class SelfUserInfo : UserReference
	{
		[JsonProperty("email")]
		public string Email;
		[JsonProperty("verified")]
		public bool IsVerified;
	}

	//Members
	public class MemberReference
	{
		[JsonProperty("user_id")]
		public string UserId;
		[JsonProperty("user")]
		public UserReference User;
		[JsonProperty("guild_id")]
		public string GuildId;
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
		public bool IsMuted;
		[JsonProperty("deaf")]
		public bool IsDeafened;
	}
	public class PresenceMemberInfo : MemberReference
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
		[JsonProperty("suppress")]
		public bool? IsSuppressed;
		[JsonProperty("session_id")]
		public string SessionId;
		[JsonProperty("self_mute")]
		public bool? IsSelfMuted;
		[JsonProperty("self_deaf")]
		public bool? IsSelfDeafened;
		[JsonProperty("mute")]
		public bool IsMuted;
		[JsonProperty("deaf")]
		public bool IsDeafened;
		[JsonProperty("token")]
		public string Token;
	}

	//Channels
	public class ChannelReference
	{
		[JsonProperty("id")]
		public string Id;
		[JsonProperty("guild_id")]
		public string GuildId;
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("type")]
		public string Type;
	}
	public class ChannelInfo : ChannelReference
	{
		public sealed class PermissionOverwrite
		{
			[JsonProperty("type")]
			public string Type;
			[JsonProperty("id")]
			public string Id;
			[JsonProperty("deny")]
			public uint Deny;
			[JsonProperty("allow")]
			public uint Allow;
		}

		[JsonProperty("last_message_id")]
		public string LastMessageId;
		[JsonProperty("is_private")]
		public bool IsPrivate;
		[JsonProperty("position")]
		public int Position;
		[JsonProperty(PropertyName = "topic")]
		public string Topic;
		[JsonProperty("permission_overwrites")]
		public PermissionOverwrite[] PermissionOverwrites;
		[JsonProperty("recipient")]
		public UserReference Recipient;
	}

	//Guilds (Servers)
	public class GuildReference
	{
		[JsonProperty("id")]
		public string Id;
		[JsonProperty("name")]
		public string Name;
	}
	public class GuildInfo : GuildReference
	{
		[JsonProperty("afk_channel_id")]
		public string AFKChannelId;
		[JsonProperty("afk_timeout")]
		public int AFKTimeout;
		[JsonProperty("embed_channel_id")]
		public string EmbedChannelId;
		[JsonProperty("embed_enabled")]
		public bool EmbedEnabled;
		[JsonProperty("icon")]
		public string Icon;
		[JsonProperty("joined_at")]
		public DateTime? JoinedAt;
		[JsonProperty("owner_id")]
		public string OwnerId;
		[JsonProperty("region")]
		public string Region;
		[JsonProperty("roles")]
		public RoleInfo[] Roles;
	}
	public class ExtendedGuildInfo : GuildInfo
	{
		[JsonProperty("channels")]
		public ChannelInfo[] Channels;
		[JsonProperty("members")]
		public ExtendedMemberInfo[] Members;
		[JsonProperty("presences")]
		public PresenceMemberInfo[] Presences;
		[JsonProperty("voice_states")]
		public VoiceMemberInfo[] VoiceStates;
		[JsonProperty("unavailable")]
		public bool Unavailable;
	}

	//Messages
	public class MessageReference
	{
		[JsonProperty("id")]
		public string Id;
		[JsonProperty("channel_id")]
		public string ChannelId;
		[JsonProperty("message_id")]
		public string MessageId { get { return Id; } set { Id = value; } }
	}
	public class Message : MessageReference
	{
		public sealed class Attachment
		{
			[JsonProperty("id")]
			public string Id;
			[JsonProperty("url")]
			public string Url;
			[JsonProperty("proxy_url")]
			public string ProxyUrl;
			[JsonProperty("size")]
			public int Size;
			[JsonProperty("filename")]
			public string Filename;
			[JsonProperty("width")]
			public int Width;
			[JsonProperty("height")]
			public int Height;
		}
		public sealed class Embed
		{
			public sealed class Reference
			{
				[JsonProperty("url")]
				public string Url;
				[JsonProperty("name")]
				public string Name;
			}
			public sealed class ThumbnailInfo
			{
				[JsonProperty("url")]
				public string Url;
				[JsonProperty("proxy_url")]
				public string ProxyUrl;
				[JsonProperty("width")]
				public int Width;
				[JsonProperty("height")]
				public int Height;
			}

			[JsonProperty("url")]
			public string Url;
			[JsonProperty("type")]
			public string Type;
			[JsonProperty("title")]
			public string Title;
			[JsonProperty("description")]
			public string Description;
			[JsonProperty("author")]
			public Reference Author;
			[JsonProperty("provider")]
			public Reference Provider;
			[JsonProperty("thumbnail")]
			public ThumbnailInfo Thumbnail;
		}

		[JsonProperty("tts")]
		public bool IsTextToSpeech;
		[JsonProperty("mention_everyone")]
		public bool IsMentioningEveryone;
		[JsonProperty("timestamp")]
		public DateTime Timestamp;
		[JsonProperty("edited_timestamp")]
		public DateTime? EditedTimestamp;
		[JsonProperty("mentions")]
		public UserReference[] Mentions;
		[JsonProperty("embeds")]
		public Embed[] Embeds; //TODO: Parse this
		[JsonProperty("attachments")]
		public Attachment[] Attachments;
		[JsonProperty("content")]
		public string Content;
		[JsonProperty("author")]
		public UserReference Author;
		[JsonProperty("nonce")]
		public string Nonce;
	}

	//Roles
	public class RoleReference
	{
		[JsonProperty("guild_id")]
		public string GuildId;
		[JsonProperty("role_id")]
		public string RoleId;
	}
	public class RoleInfo
	{
		[JsonProperty("permissions")]
		public uint Permissions;
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("id")]
		public string Id;
	}

	//Invites
	public class Invite
	{
		[JsonProperty("inviter")]
		public UserReference Inviter;
		[JsonProperty("guild")]
		public GuildReference Guild;
		[JsonProperty("channel")]
		public ChannelReference Channel;
		[JsonProperty("code")]
		public string Code;
		[JsonProperty("xkcdpass")]
		public string XkcdPass;
	}
	public class ExtendedInvite : Invite
	{
		[JsonProperty("max_age")]
		public int MaxAge;
		[JsonProperty("max_uses")]
		public int MaxUses;
		[JsonProperty("revoked")]
		public bool IsRevoked;
		[JsonProperty("temporary")]
		public bool IsTemporary;
		[JsonProperty("uses")]
		public int Uses;
		[JsonProperty("created_at")]
		public DateTime CreatedAt;
	}
}
