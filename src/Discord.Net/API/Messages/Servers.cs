//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API
{
	//Common
	public class GuildReference
	{
		[JsonProperty("id")]
		[JsonConverter(typeof(LongStringConverter))]
		public ulong Id;
		[JsonProperty("name")]
		public string Name;
	}
	public class GuildInfo : GuildReference
	{
		[JsonProperty("afk_channel_id")]
		[JsonConverter(typeof(NullableLongStringConverter))]
		public ulong? AFKChannelId;
		[JsonProperty("afk_timeout")]
		public int? AFKTimeout;
		[JsonProperty("embed_channel_id")]
		[JsonConverter(typeof(NullableLongStringConverter))]
		public ulong? EmbedChannelId;
		[JsonProperty("embed_enabled")]
		public bool EmbedEnabled;
		[JsonProperty("icon")]
		public string Icon;
		[JsonProperty("joined_at")]
		public DateTime? JoinedAt;
		[JsonProperty("owner_id")]
		[JsonConverter(typeof(NullableLongStringConverter))]
		public ulong? OwnerId;
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
		public bool? Unavailable;
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
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("region")]
		public string Region;
		[JsonProperty("icon")]
		public string Icon;
		[JsonProperty("afk_channel_id")]
		[JsonConverter(typeof(NullableLongStringConverter))]
		public ulong? AFKChannelId;
		[JsonProperty("afk_timeout")]
		public int AFKTimeout;
	}
	public sealed class EditServerResponse : GuildInfo { }

	//Delete
	public sealed class DeleteServerResponse : GuildInfo { }

	//GetRegions
	public class GetRegionsResponse : List<GetRegionsResponse.RegionData>
	{
		public sealed class RegionData
		{
			[JsonProperty("sample_hostname")]
			public string Hostname;
			[JsonProperty("sample_port")]
			public int Port;
			[JsonProperty("id")]
			public string Id;
			[JsonProperty("name")]
			public string Name;
		}
	}
	//Events
	internal sealed class GuildCreateEvent : ExtendedGuildInfo { }
	internal sealed class GuildUpdateEvent : GuildInfo { }
	internal sealed class GuildDeleteEvent : ExtendedGuildInfo { }
}
