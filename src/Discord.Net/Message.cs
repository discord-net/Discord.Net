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

		public string Id { get; }

		public bool IsMentioningMe { get; internal set; }
		public bool IsMentioningEveryone { get; internal set; }
		public bool IsTTS { get; internal set; }
		public string Text { get; internal set; }
		public DateTime Timestamp { get; internal set; }
		public Attachment[] Attachments { get; internal set; }

		public string[] MentionIds { get; internal set; }
		[JsonIgnore]
		public IEnumerable<User> Mentions => MentionIds.Select(x => _client.GetUser(x)).Where(x => x != null);

		public string ChannelId { get; }
		[JsonIgnore]
		public Channel Channel => _client.GetChannel(ChannelId);

		public string UserId { get; internal set; }
		[JsonIgnore]
		public User User => _client.GetUser(UserId);
		
		//Not Implemented
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
