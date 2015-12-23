using System;
using System.Collections.Generic;
using System.Linq;
using APIRole = Discord.API.Client.Role;

namespace Discord
{
	public sealed class Role
    {
        private readonly DiscordClient _client;

        /// <summary> Gets the unique identifier for this role. </summary>
        public ulong Id { get; }
        /// <summary> Gets the server this role is a member of. </summary>
        public Server Server { get; }
        /// <summary> Gets the the permissions contained by this role. </summary>
        public ServerPermissions Permissions { get; }
        /// <summary> Gets the color of this role. </summary>
        public Color Color { get; }

        /// <summary> Gets the name of this role. </summary>
        public string Name { get; private set; }
		/// <summary> If true, this role is displayed isolated from other users. </summary>
		public bool IsHoisted { get; private set; }
        /// <summary> Gets the position of this channel relative to other channels in this server. </summary>
        public int Position { get; private set; }
        /// <summary> Gets whether this role is managed by server (e.g. for Twitch integration) </summary>
        public bool IsManaged { get; private set; }

        /// <summary> Gets true if this is the role representing all users in a server. </summary>
        public bool IsEveryone => Id == Server.Id;
        /// <summary> Gets a list of all members in this role. </summary>
        public IEnumerable<User> Members => IsEveryone ? Server.Users : Server.Users.Where(x => x.HasRole(this));

        /// <summary> Gets the string used to mention this role. </summary>
        public string Mention
        {
            get
            {
                if (IsEveryone)
                    return "@everyone";
                else
                    throw new InvalidOperationException("Roles may only be mentioned if IsEveryone is true");
            }
        }

		internal Role(ulong id, Server server)
		{
            Id = id;
            Server = server;
			Permissions = new ServerPermissions(0);
			Permissions.Lock();
			Color = new Color(0);
			Color.Lock();
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
		public override string ToString() => Name ?? Id.ToIdString();
	}
}
