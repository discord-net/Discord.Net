using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Message
	{
		public struct Attachment
		{
			public string Id;
			public string Url;
			public string ProxyUrl;
			public int Size;
			public string Filename;
		}

		private readonly DiscordClient _client;

		/// <summary> Returns the unique identifier for this message. </summary>
		public string Id { get; }

		/// <summary> Returns true if the logged-in user was mentioned. </summary>
		/// <remarks> This is not set to true if the user was mentioned with @everyone (see IsMentioningEverone). </remarks>
		public bool IsMentioningMe { get; internal set; }
		/// <summary> Returns true if @everyone was mentioned by someone with permissions to do so. </summary>
		public bool IsMentioningEveryone { get; internal set; }
		/// <summary> Returns true if the message was sent as text-to-speech by someone with permissions to do so. </summary>
		public bool IsTTS { get; internal set; }
		/// <summary> Returns the content of this message. </summary>
		public string Text { get; internal set; }
		/// <summary> Returns the timestamp of this message. </summary>
		public DateTime Timestamp { get; internal set; }
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
