//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;

namespace Discord.API
{
	//Common
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
		public PresenceInfo[] Presences;
		[JsonProperty("voice_states")]
		public VoiceMemberInfo[] VoiceStates;
		[JsonProperty("unavailable")]
		public bool Unavailable;
	}

	//Create
	internal sealed class CreateServerRequest
	{
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("region")]
		public string Region;
	}
	public sealed class CreateServerResponse : GuildInfo { }

	//Edit
	internal sealed class EditServerRequest
	{
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name;
		[JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
		public string Region;
		[JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
		public string Icon;
	}
	public sealed class EditServerResponse : GuildInfo { }

	//Delete
	public sealed class DeleteServerResponse : GuildInfo { }

	//Events
	internal sealed class GuildCreateEvent : ExtendedGuildInfo { }
	internal sealed class GuildUpdateEvent : GuildInfo { }
	internal sealed class GuildDeleteEvent : ExtendedGuildInfo { }
}
