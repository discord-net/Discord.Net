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

	public class UserInfo
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
	public class SelfUserInfo : UserInfo
	{
		[JsonProperty(PropertyName = "email")]
		public string Email;
		[JsonProperty(PropertyName = "verified")]
		public bool IsVerified;
	}
	public class PresenceUserInfo : UserInfo
	{
		[JsonProperty(PropertyName = "game_id")]
		public string GameId;
		[JsonProperty(PropertyName = "status")]
		public string Status;
	}

	public class MembershipInfo
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
		public UserInfo User;
	}

	public class ChannelInfo
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "name")]
		public string Name;
		[JsonProperty(PropertyName = "last_message_id")]
		public string LastMessageId;
		[JsonProperty(PropertyName = "is_private")]
		public bool IsPrivate;
		[JsonProperty(PropertyName = "type")]
		public string Type;
		[JsonProperty(PropertyName = "permission_overwrites")]
		public object[] PermissionOverwrites;
		[JsonProperty(PropertyName = "recipient")]
		public UserInfo Recipient;
	}

	public class ServerInfo
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "name")]
		public string Name;
	}
	public class ExtendedServerInfo : ServerInfo
	{
		[JsonProperty(PropertyName = "afk_channel_id")]
		public string AFKChannelId;
		[JsonProperty(PropertyName = "afk_timeout")]
		public int AFKTimeout;
		[JsonProperty(PropertyName = "channels")]
		public ChannelInfo[] Channels;
		[JsonProperty(PropertyName = "joined_at")]
		public DateTime JoinedAt;
		[JsonProperty(PropertyName = "members")]
		public MembershipInfo[] Members;
		[JsonProperty(PropertyName = "owner_id")]
		public string OwnerId;
		[JsonProperty(PropertyName = "presence")]
		public object[] Presence;
		[JsonProperty(PropertyName = "region")]
		public string Region;
		[JsonProperty(PropertyName = "roles")]
		public object[] Roles;
		[JsonProperty(PropertyName = "voice_states")]
		public object[] VoiceStates;
	}

	internal class MessageReference
	{
		[JsonProperty(PropertyName = "message_id")]
		public string MessageId;
		[JsonProperty(PropertyName = "channel_id")]
		public string ChannelId;
	}
}
