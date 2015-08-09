using Newtonsoft.Json;
using System;

namespace Discord
{
	public sealed class ChatMessage
	{
		private readonly DiscordClient _client;

		public string Id { get; }

		public bool IsMentioningEveryone { get; internal set; }
		public bool IsTTS { get; internal set; }
		public string Text { get; internal set; }
		public DateTime Timestamp { get; internal set; }

		public string ChannelId { get; internal set; }
		[JsonIgnore]
		public Channel Channel { get { return _client.GetChannel(ChannelId); } }

		public string UserId { get; internal set; }
		[JsonIgnore]
		public User User { get { return _client.GetUser(UserId); } }
		
		//Not Implemented
		public object[] Attachments { get; internal set; }
		public object[] Embeds { get; internal set; }

		internal ChatMessage(string id, string channelId, DiscordClient client)
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
