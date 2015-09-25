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
		public Server Server => _client.Servers[ServerId];

		internal Role(DiscordClient client, string id, string serverId)
		{
			_client = client;
			Id = id;
			ServerId = serverId;
			Permissions = new PackedPermissions(true);
		}

		internal void Update(API.RoleInfo model)
		{
			Name = model.Name;
			Permissions.RawValue = (uint)model.Permissions;
		}

		public override string ToString() => Name;
	}
}
