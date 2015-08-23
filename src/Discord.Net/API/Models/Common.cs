//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Discord.API.Models
{
	internal class WebSocketMessage
	{
		[JsonProperty(PropertyName = "op")]
		public int Operation;
		[JsonProperty(PropertyName = "t")]
		public string Type;
		[JsonProperty(PropertyName = "d")]
		public object Payload;
	}
	internal abstract class WebSocketMessage<T> : WebSocketMessage
		where T : new()
	{
		public WebSocketMessage() { Payload = new T(); }
		public WebSocketMessage(int op) { Operation = op; Payload = new T(); }
		public WebSocketMessage(int op, T payload) { Operation = op; Payload = payload; }

		[JsonIgnore]
		public new T Payload
		{
			get { if (base.Payload is JToken) { base.Payload = (base.Payload as JToken).ToObject<T>(); } return (T)base.Payload; }
			set { base.Payload = value; }
		}
	}

	//Users
	internal class UserReference
	{
		[JsonProperty(PropertyName = "username")]
		public string Username;
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "discriminator")]
		public string Discriminator;
		[JsonProperty(PropertyName = "avatar")]
		public string Avatar;
	}
	internal class SelfUserInfo : UserReference
	{
		[JsonProperty(PropertyName = "email")]
		public string Email;
		[JsonProperty(PropertyName = "verified")]
		public bool IsVerified;
	}
	internal class MemberInfo
	{
		[JsonProperty(PropertyName = "user_id")]
		public string UserId;
		[JsonProperty(PropertyName = "user")]
		public UserReference User;
		[JsonProperty(PropertyName = "guild_id")]
		public string ServerId;
	}
	internal class InitialMemberInfo : RoleMemberInfo
	{
		[JsonProperty(PropertyName = "mute")]
		public bool IsMuted;
		[JsonProperty(PropertyName = "deaf")]
		public bool IsDeafened;
	}
	internal class PresenceMemberInfo : MemberInfo
	{
		[JsonProperty(PropertyName = "game_id")]
		public string GameId;
		[JsonProperty(PropertyName = "status")]
		public string Status;
	}
	internal class VoiceMemberInfo : MemberInfo
	{
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
		[JsonProperty(PropertyName = "token")]
		public string Token;
	}
	internal class RoleMemberInfo : MemberInfo
	{
		[JsonProperty(PropertyName = "joined_at")]
		public DateTime? JoinedAt;
		[JsonProperty(PropertyName = "roles")]
		public string[] Roles;
	}

	//Channels
	internal class ChannelReference
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "guild_id")]
		public string GuildId;
		[JsonProperty(PropertyName = "name")]
		public string Name;
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
	internal class ChannelInfo : ChannelReference
	{
		public sealed class PermissionOverwrite
		{
			[JsonProperty(PropertyName = "type")]
			public string Type;
			[JsonProperty(PropertyName = "id")]
			public string Id;
			[JsonProperty(PropertyName = "deny")]
			public uint Deny;
			[JsonProperty(PropertyName = "allow")]
			public uint Allow;
		}

		[JsonProperty(PropertyName = "last_message_id")]
		public string LastMessageId;
		[JsonProperty(PropertyName = "is_private")]
		public bool IsPrivate;
		[JsonProperty(PropertyName = "position")]
		public int Position;
		[JsonProperty(PropertyName = "permission_overwrites")]
		public PermissionOverwrite[] PermissionOverwrites;
		[JsonProperty(PropertyName = "recipient")]
		public UserReference Recipient;
	}

	//Servers
	internal class ServerReference
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "name")]
		public string Name;
	}
	internal class ServerInfo : ServerReference
	{
		[JsonProperty(PropertyName = "afk_channel_id")]
		public string AFKChannelId;
		[JsonProperty(PropertyName = "afk_timeout")]
		public int AFKTimeout;
		[JsonProperty(PropertyName = "embed_channel_id")]
		public string EmbedChannelId;
		[JsonProperty(PropertyName = "embed_enabled")]
		public bool EmbedEnabled;
		[JsonProperty(PropertyName = "icon")]
		public string Icon;
		[JsonProperty(PropertyName = "joined_at")]
		public DateTime? JoinedAt;
		[JsonProperty(PropertyName = "owner_id")]
		public string OwnerId;
		[JsonProperty(PropertyName = "region")]
		public string Region;
		[JsonProperty(PropertyName = "roles")]
		public Role[] Roles;
	}
	internal class ExtendedServerInfo : ServerInfo
	{
		[JsonProperty(PropertyName = "channels")]
		public ChannelInfo[] Channels;
		[JsonProperty(PropertyName = "members")]
		public InitialMemberInfo[] Members;
		[JsonProperty(PropertyName = "presences")]
		public PresenceMemberInfo[] Presences;
		[JsonProperty(PropertyName = "voice_states")]
		public VoiceMemberInfo[] VoiceStates;
	}

	//Messages
	internal class MessageReference
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "channel_id")]
		public string ChannelId;
		[JsonProperty(PropertyName = "message_id")]
		public string MessageId { get { return Id; } set { Id = value; } }
	}
	internal class Message : MessageReference
	{
		public sealed class Attachment
		{
			[JsonProperty(PropertyName = "id")]
			public string Id;
			[JsonProperty(PropertyName = "url")]
			public string Url;
			[JsonProperty(PropertyName = "proxy_url")]
			public string ProxyUrl;
			[JsonProperty(PropertyName = "size")]
			public int Size;
			[JsonProperty(PropertyName = "filename")]
			public string Filename;
			[JsonProperty(PropertyName = "width")]
			public int Width;
			[JsonProperty(PropertyName = "height")]
			public int Height;
		}
		public sealed class Embed
		{
			public sealed class Reference
			{
				[JsonProperty(PropertyName = "url")]
				public string Url;
				[JsonProperty(PropertyName = "name")]
				public string Name;
			}
			public sealed class ThumbnailInfo
			{
				[JsonProperty(PropertyName = "url")]
				public string Url;
				[JsonProperty(PropertyName = "proxy_url")]
				public string ProxyUrl;
				[JsonProperty(PropertyName = "width")]
				public int Width;
				[JsonProperty(PropertyName = "height")]
				public int Height;
			}

			[JsonProperty(PropertyName = "url")]
			public string Url;
			[JsonProperty(PropertyName = "type")]
			public string Type;
			[JsonProperty(PropertyName = "title")]
			public string Title;
			[JsonProperty(PropertyName = "description")]
			public string Description;
			[JsonProperty(PropertyName = "author")]
			public Reference Author;
			[JsonProperty(PropertyName = "provider")]
			public Reference Provider;
			[JsonProperty(PropertyName = "thumbnail")]
			public ThumbnailInfo Thumbnail;
		}

		[JsonProperty(PropertyName = "tts")]
		public bool IsTextToSpeech;
		[JsonProperty(PropertyName = "mention_everyone")]
		public bool IsMentioningEveryone;
		[JsonProperty(PropertyName = "timestamp")]
		public DateTime Timestamp;
		[JsonProperty(PropertyName = "edited_timestamp")]
		public DateTime? EditedTimestamp;
		[JsonProperty(PropertyName = "mentions")]
		public UserReference[] Mentions;
		[JsonProperty(PropertyName = "embeds")]
		public Embed[] Embeds; //TODO: Parse this
		[JsonProperty(PropertyName = "attachments")]
		public Attachment[] Attachments;
		[JsonProperty(PropertyName = "content")]
		public string Content;
		[JsonProperty(PropertyName = "author")]
		public UserReference Author;
		[JsonProperty(PropertyName = "nonce")]
		public string Nonce;
	}

	//Roles
    internal class Role
	{
		[JsonProperty(PropertyName = "permissions")]
		public int Permissions;
		[JsonProperty(PropertyName = "name")]
		public string Name;
		[JsonProperty(PropertyName = "id")]
		public string Id;
	}
}
