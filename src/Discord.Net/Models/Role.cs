using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Role
	{
		private readonly DiscordClient _client;

		/// <summary> Returns the unique identifier for this role. </summary>
		public string Id { get; }
		/// <summary> Returns the name of this role. </summary>
		public string Name { get; private set; }
		/// <summary> If true, this role is displayed isolated from other users. </summary>
		public bool Hoist { get; private set; }
		/// <summary> Returns the color of this role. </summary>
		public PackedColor Color { get; private set; }

		/// <summary> Returns the the permissions contained by this role. </summary>
		public PackedServerPermissions Permissions { get; }

		/// <summary> Returns the id of the server this role is a member of. </summary>
		public string ServerId { get; }
		/// <summary> Returns the server this role is a member of. </summary>
		[JsonIgnore]
		public Server Server => _client.Servers[ServerId];

		/// <summary> Returns true if this is the role representing all users in a server. </summary>
		public bool IsEveryone { get; }
		/// <summary> Returns a list of the ids of all members in this role. </summary>
		public IEnumerable<string> MemberIds => IsEveryone ? Server.UserIds : Server.Members.Where(x => x.RoleIds.Contains(Id)).Select(x => x.UserId);
		/// <summary> Returns a list of all members in this role. </summary>
		public IEnumerable<Member> Members => IsEveryone ? Server.Members : Server.Members.Where(x => x.RoleIds.Contains(Id));

		internal Role(DiscordClient client, string id, string serverId, bool isEveryone)
		{
			_client = client;
			Id = id;
			ServerId = serverId;
			IsEveryone = isEveryone;
			Permissions = new PackedServerPermissions(0);
			Permissions.Lock();
			Color = new PackedColor(0);
			Color.Lock();
        }

		internal void Update(API.RoleInfo model)
		{
			if (model.Name != null)
				Name = model.Name;
			if (model.Hoist != null)
				Hoist = model.Hoist.Value;
			if (model.Color != null)
				Color.SetRawValue(model.Color.Value);
			if (model.Permissions != null)
				Permissions.SetRawValue(model.Permissions.Value);

			foreach (var member in Members)
				member.UpdatePermissions();
		}

		public override string ToString() => Name;
	}
}
