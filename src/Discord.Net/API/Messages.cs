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
	public class MessageReference
	{
		[JsonProperty("id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long Id;
		[JsonProperty("channel_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long ChannelId;
		[JsonProperty("message_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long MessageId { get { return Id; } set { Id = value; } }
	}
	public class MessageInfo : MessageReference
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
		public bool? IsTextToSpeech;
		[JsonProperty("mention_everyone")]
		public bool? IsMentioningEveryone;
		[JsonProperty("timestamp")]
		public DateTime? Timestamp;
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

	//Create
	internal sealed class SendMessageRequest
	{
		[JsonProperty("content")]
		public string Content;
		[JsonProperty("mentions")]
		[JsonConverter(typeof(EnumerableLongStringConverter))]
		public IEnumerable<long> Mentions;
		[JsonProperty("nonce", NullValueHandling = NullValueHandling.Ignore)]
		public string Nonce;
		[JsonProperty("tts", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsTTS;
	}
	public sealed class SendMessageResponse : MessageInfo { }

	//Edit
	internal sealed class EditMessageRequest
	{
		[JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
		public string Content;
		[JsonProperty("mentions", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EnumerableLongStringConverter))]
		public IEnumerable<long> Mentions;
	}
	public sealed class EditMessageResponse : MessageInfo { }

	//Get
	public sealed class GetMessagesResponse : List<MessageInfo> { }

	//Commands
	internal sealed class GetUsersCommand : WebSocketMessage<GetUsersCommand.Data>
	{
		public GetUsersCommand() : base(8) { }
		public class Data
		{
			[JsonProperty("guild_id")]
			[JsonConverter(typeof(LongStringConverter))]
			public long ServerId;
			[JsonProperty("query")]
			public string Query;
			[JsonProperty("limit")]
			public int Limit;
		}
	}

	//Events
	internal sealed class MessageCreateEvent : MessageInfo { }
	internal sealed class MessageUpdateEvent : MessageInfo { }
	internal sealed class MessageDeleteEvent : MessageReference { }
	internal sealed class MessageAckEvent : MessageReference { }
}
