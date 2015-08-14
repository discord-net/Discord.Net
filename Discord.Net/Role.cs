using Newtonsoft.Json;

namespace Discord
{
	public sealed class Role
	{
		private readonly DiscordClient _client;

		public string Id { get; }
		public string Name { get; internal set; }

		public int Permissions { get; internal set; }

		public string ServerId { get; }
		[JsonIgnore]
		public Server Server { get { return _client.GetServer(ServerId); } }

		internal Role(string id, string serverId, DiscordClient client)
		{
			Id = id;
			ServerId = serverId;
			_client = client;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
