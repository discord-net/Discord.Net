namespace Discord.Models
{
	public class ChatMessageReference
	{
		protected readonly DiscordClient _client;

		public string Id { get; }
		public string ChannelId { get; internal set; }
		public Channel Channel { get { return _client.GetChannel(ChannelId); } }

		internal ChatMessageReference(string id, DiscordClient client)
		{
			Id = id;
			_client = client;
		}
	}
}
