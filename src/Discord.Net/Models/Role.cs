using Newtonsoft.Json;

namespace Discord
{
	public sealed class Role
	{
		private readonly DiscordClient _client;

		/// <summary> Returns the unique identifier for this role. </summary>
		public string Id { get; }
		/// <summary> Returns the name of this role. </summary>
		public string Name { get; internal set; }

		/// <summary> Returns the the permissions contained by this role. </summary>
		public PackedPermissions Permissions { get; }

		/// <summary> Returns the id of the server this role is a member of. </summary>
		public string ServerId { get; }
		/// <summary> Returns the server this role is a member of. </summary>
		[JsonIgnore]
		public Server Server => _client.GetServer(ServerId);

		internal Role(string id, string serverId, DiscordClient client)
		{
			Permissions = new PackedPermissions();
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
