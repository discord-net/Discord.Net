using Newtonsoft.Json;

namespace Discord.Models
{
	public class ChatMessageReference
	{
		protected readonly DiscordClient _client;

		public string Id { get; }

		public string ChannelId { get; }
		[JsonIgnore]
		public Channel Channel { get { return _client.GetChannel(ChannelId); } }

		internal ChatMessageReference(string id, string channelId, DiscordClient client)
		{
			Id = id;
			ChannelId = channelId;
			_client = client;
		}
	}
}
