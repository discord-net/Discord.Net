using Newtonsoft.Json;
using System;

namespace Discord.Models
{
	public sealed class ChatMessage : ChatMessageReference
	{
		public bool IsMentioningEveryone { get; internal set; }
		public bool IsTTS { get; internal set; }
		public string Text { get; internal set; }
		public DateTime Timestamp { get; internal set; }
		
		public string UserId { get; internal set; }
		[JsonIgnore]
		public User User { get { return _client.GetUser(UserId); } }
		
		//Not Implemented
		public object[] Attachments { get; internal set; }
		public object[] Embeds { get; internal set; }

		internal ChatMessage(string id, string channelId, DiscordClient client)
			: base(id, channelId, client)
		{
		}

		public override string ToString()
		{
			return User.ToString() + ": " + Text;
		}
	}
}
