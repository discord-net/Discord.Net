using Discord.API;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Role : CachedObject
	{
		private readonly string _serverId;
		
		/// <summary> Returns the name of this role. </summary>
		public string Name { get; private set; }
		/// <summary> If true, this role is displayed isolated from other users. </summary>
		public bool IsHoisted { get; private set; }
		/// <summary> Returns the position of this channel in the role list for this server. </summary>
		public int Position { get; private set; }
		/// <summary> Returns the color of this role. </summary>
		public Color Color { get; private set; }
		/// <summary> Returns whether this role is managed by server (e.g. for Twitch integration) </summary>
		public bool IsManaged { get; private set; }

		/// <summary> Returns the the permissions contained by this role. </summary>
		public ServerPermissions Permissions { get; }
		
		/// <summary> Returns the server this role is a member of. </summary>
		[JsonIgnore]
		public Server Server { get; private set; }

		/// <summary> Returns true if this is the role representing all users in a server. </summary>
		public bool IsEveryone => Id == _serverId;
		/// <summary> Returns a list of all members in this role. </summary>
		[JsonIgnore]
		public IEnumerable<User> Members => IsEveryone ? Server.Members : Server.Members.Where(x => x.HasRole(this));

		internal Role(DiscordClient client, string id, string serverId)
			: base(client, id)
		{
			_serverId = serverId;
			Permissions = new ServerPermissions(0);
			Permissions.Lock();
			Color = new Color(0);
			Color.Lock();

			if (IsEveryone)
				Position = int.MinValue;
		}
		internal override void OnCached()
		{
			var server = _client.Servers[_serverId];
			server.AddRole(this);
			Server = server;
		}
		internal override void OnUncached()
		{
			var server = Server;
			if (server != null)
				server.RemoveRole(this);
			Server = null;
        }

		internal void Update(RoleInfo model)
		{
			if (model.Name != null)
				Name = model.Name;
			if (model.Hoist != null)
				IsHoisted = model.Hoist.Value;
			if (model.Managed != null)
				IsManaged = model.Managed.Value;
			if (model.Position != null && !IsEveryone)
				Position = model.Position.Value;
			if (model.Color != null)
				Color.SetRawValue(model.Color.Value);
			if (model.Permissions != null)
				Permissions.SetRawValueInternal(model.Permissions.Value);

			foreach (var member in Members)
				member.UpdateServerPermissions();
		}

		public override string ToString() => Name;
	}
}
