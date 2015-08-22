using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Message
	{
		public class Attachment
		{
			/// <summary> Unique identifier for this file. </summary>
			public string Id { get; internal set; }
			/// <summary> Download url for this file. </summary>
			public string Url { get; internal set; }
			/// <summary> Preview url for this file. </summary>
			public string ProxyUrl { get; internal set; }
			/// <summary> Width of the this file, if it is an image. </summary>
			public int? Width { get; internal set; }
			/// <summary> Height of this file, if it is an image. </summary>
			public int? Height { get; internal set; }
			/// <summary> Size, in bytes, of this file file. </summary>
			public int Size { get; internal set; }
			/// <summary> Filename of this file. </summary>
			public string Filename { get; internal set; }
		}

		private readonly DiscordClient _client;
		private string _cleanText;
		
		/// <summary> Returns the unique identifier for this message. </summary>
		public string Id { get; }

		/// <summary> Returns true if the logged-in user was mentioned. </summary>
		/// <remarks> This is not set to true if the user was mentioned with @everyone (see IsMentioningEverone). </remarks>
		public bool IsMentioningMe { get; internal set; }
		/// <summary> Returns true if @everyone was mentioned by someone with permissions to do so. </summary>
		public bool IsMentioningEveryone { get; internal set; }
		/// <summary> Returns true if the message was sent as text-to-speech by someone with permissions to do so. </summary>
		public bool IsTTS { get; internal set; }
		/// <summary> Returns the raw content of this message as it was received from the server.. </summary>
		public string RawText { get; internal set; }
		/// <summary> Returns the content of this message with any special references such as mentions converted. </summary>
		/// <remarks> This value is lazy loaded and only processed on first request. Each subsequent request will pull from cache. </remarks>
		public string Text => _cleanText != null ? _cleanText : (_cleanText = _client.CleanMessageText(RawText));
		/// <summary> Returns the timestamp for when this message was sent. </summary>
		public DateTime Timestamp { get; internal set; }
		/// <summary> Returns the timestamp for when this message was last edited. </summary>
		public DateTime? EditedTimestamp { get; internal set; }
		/// <summary> Returns the attachments included in this message. </summary>
		public Attachment[] Attachments { get; internal set; }

		/// <summary> Returns a collection of all user ids mentioned in this message. </summary>
		public string[] MentionIds { get; internal set; }
		/// <summary> Returns a collection of all users mentioned in this message. </summary>
		[JsonIgnore]
		public IEnumerable<User> Mentions => MentionIds.Select(x => _client.GetUser(x)).Where(x => x != null);

		/// <summary> Returns the id of the channel this message was sent in. </summary>
		public string ChannelId { get; }
		/// <summary> Returns the the channel this message was sent in. </summary>
		[JsonIgnore]
		public Channel Channel => _client.GetChannel(ChannelId);

		/// <summary> Returns the id of the author of this message. </summary>
		public string UserId { get; internal set; }
		/// <summary> Returns the author of this message. </summary>
		[JsonIgnore]
		public User User => _client.GetUser(UserId);

		//TODO: Not Implemented
		/// <summary> Not implemented, stored for reference. </summary>
		public object[] Embeds { get; internal set; }

		internal Message(string id, string channelId, DiscordClient client)
		{
			Id = id;
			ChannelId = channelId;
			_client = client;
        }

		public override string ToString()
		{
			return User.ToString() + ": " + Text;
		}
    }
}
