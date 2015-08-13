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
	internal class PresenceUserInfo : UserReference
	{
		[JsonProperty(PropertyName = "game_id")]
		public string GameId;
		[JsonProperty(PropertyName = "status")]
		public string Status;
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
		[JsonProperty(PropertyName = "last_message_id")]
		public string LastMessageId;
		[JsonProperty(PropertyName = "is_private")]
		public bool IsPrivate;
		[JsonProperty(PropertyName = "permission_overwrites")]
		public object[] PermissionOverwrites;
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
		public class Membership
		{
			[JsonProperty(PropertyName = "roles")]
			public object[] Roles;
			[JsonProperty(PropertyName = "mute")]
			public bool IsMuted;
			[JsonProperty(PropertyName = "deaf")]
			public bool IsDeaf;
			[JsonProperty(PropertyName = "joined_at")]
			public DateTime JoinedAt;
			[JsonProperty(PropertyName = "user")]
			public UserReference User;
		}

		[JsonProperty(PropertyName = "channels")]
		public ChannelInfo[] Channels;
		[JsonProperty(PropertyName = "members")]
		public Membership[] Members;
		[JsonProperty(PropertyName = "presence")]
		public object[] Presence;
		[JsonProperty(PropertyName = "voice_states")]
		public object[] VoiceStates;
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
		[JsonProperty(PropertyName = "tts")]
		public bool IsTextToSpeech;
		[JsonProperty(PropertyName = "mention_everyone")]
		public bool IsMentioningEveryone;
		[JsonProperty(PropertyName = "timestamp")]
		public DateTime Timestamp;
		[JsonProperty(PropertyName = "mentions")]
		public UserReference[] Mentions;
		[JsonProperty(PropertyName = "embeds")]
		public object[] Embeds;
		[JsonProperty(PropertyName = "attachments")]
		public object[] Attachments;
		[JsonProperty(PropertyName = "content")]
		public string Content;
		[JsonProperty(PropertyName = "author")]
		public UserReference Author;
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
