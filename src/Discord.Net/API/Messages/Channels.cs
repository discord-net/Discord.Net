//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Discord.API
{
	//Common
	public class ChannelReference
	{
		[JsonProperty("id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long Id;
		[JsonProperty("guild_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long GuildId;
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
			[JsonConverter(typeof(LongStringConverter))]
			public long Id;
			[JsonProperty("deny")]
			public uint Deny;
			[JsonProperty("allow")]
			public uint Allow;
		}

		[JsonProperty("last_message_id")]
		[JsonConverter(typeof(NullableLongStringConverter))]
		public long? LastMessageId;
		[JsonProperty("is_private")]
		public bool IsPrivate;
		[JsonProperty("position")]
		public int? Position;
		[JsonProperty("topic")]
		public string Topic;
		[JsonProperty("permission_overwrites")]
		public PermissionOverwrite[] PermissionOverwrites;
		[JsonProperty("recipient")]
		public UserReference Recipient;
	}

	//Create
	public class CreateChannelRequest
	{
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("type")]
		public string Type;
	}
	public class CreatePMChannelRequest
	{
		[JsonProperty("recipient_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long RecipientId;
	}
	public class CreateChannelResponse : ChannelInfo { }

	//Edit
	public class EditChannelRequest
	{
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name;
		[JsonProperty("topic", NullValueHandling = NullValueHandling.Ignore)]
		public string Topic;
	}
	public class EditChannelResponse : ChannelInfo { }

	//Destroy
	public class DestroyChannelResponse : ChannelInfo { }

	//Reorder
	public class ReorderChannelsRequest : IEnumerable<ReorderChannelsRequest.Channel>
	{
		public sealed class Channel
		{
			[JsonProperty("id")]
			[JsonConverter(typeof(LongStringConverter))]
			public long Id;
			[JsonProperty("position")]
			public uint Position;
		}
		private IEnumerable<Channel> _channels;
		public ReorderChannelsRequest(IEnumerable<Channel> channels) { _channels = channels; }

		public IEnumerator<Channel> GetEnumerator() => _channels.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _channels.GetEnumerator();
	}

	//Events
	internal sealed class ChannelCreateEvent : ChannelInfo { }
	internal sealed class ChannelDeleteEvent : ChannelInfo { }
	internal sealed class ChannelUpdateEvent : ChannelInfo { }
}
