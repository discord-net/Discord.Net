using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using APIRole = Discord.API.Client.Role;

namespace Discord
{
	public sealed class Role : CachedObject<ulong>
	{
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
		public Server Server => _server.Value;
		[JsonProperty]
		private ulong? ServerId { get { return _server.Id; } set { _server.Id = value; } }
		private readonly Reference<Server> _server;

		/// <summary> Returns true if this is the role representing all users in a server. </summary>
		public bool IsEveryone => _server.Id == null || Id == _server.Id;

		/// <summary> Returns a list of all members in this role. </summary>
		[JsonIgnore]
		public IEnumerable<User> Members => _server.Id != null ? (IsEveryone ? Server.Members : Server.Members.Where(x => x.HasRole(this))) : new User[0];
		[JsonProperty]
		private IEnumerable<ulong> MemberIds => Members.Select(x => x.Id);
		//TODO: Add local members cache

		/// <summary> Returns the string used to mention this role. </summary>
		public string Mention { get { if (IsEveryone) return "@everyone"; else throw new InvalidOperationException("Discord currently only supports mentioning the everyone role"); } }

		internal Role(DiscordClient client, ulong id, ulong serverId)
			: base(client, id)
		{
			_server = new Reference<Server>(serverId, x => _client.Servers[x], x => x.AddRole(this), x => x.RemoveRole(this));
			Permissions = new ServerPermissions(0);
			Permissions.Lock();
			Color = new Color(0);
			Color.Lock();
		}
		internal override bool LoadReferences()
		{
			return _server.Load();
		}
		internal override void UnloadReferences()
		{
			_server.Unload();
        }

		internal void Update(APIRole model)
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
				Server.UpdatePermissions(member);
		}

		public override bool Equals(object obj) => obj is Role && (obj as Role).Id == Id;
		public override int GetHashCode() => unchecked(Id.GetHashCode() + 6653);
		public override string ToString() => Name ?? IdConvert.ToString(Id);
	}
}
