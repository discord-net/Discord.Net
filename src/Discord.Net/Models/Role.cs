using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using APIRole = Discord.API.Client.Role;

namespace Discord
{
    public class Role : IMentionable
    {
        private readonly static Action<Role, Role> _cloner = DynamicIL.CreateCopyMethod<Role>();

        public DiscordClient Client => Server.Client;

        /// <summary> Gets the unique identifier for this role. </summary>
        public ulong Id { get; }
        /// <summary> Gets the server this role is a member of. </summary>
        public Server Server { get; }

        /// <summary> Gets the name of this role. </summary>
        public string Name { get; private set; }
		/// <summary> If true, this role is displayed isolated from other users. </summary>
		public bool IsHoisted { get; private set; }
        /// <summary> Gets the position of this channel relative to other channels in this server. </summary>
        public int Position { get; private set; }
        /// <summary> Gets whether this role is managed by server (e.g. for Twitch integration) </summary>
        public bool IsManaged { get; private set; }
        /// <summary> Gets whether this role is mentionable by anyone. </summary>
        public bool IsMentionable { get; private set; }
        /// <summary> Gets the the permissions given to this role. </summary>
        public ServerPermissions Permissions { get; private set; }
        /// <summary> Gets the color of this role. </summary>
        public Color Color { get; private set; }

        /// <summary> Gets the path to this object. </summary>
        internal string Path => $"{Server?.Name ?? "[Private]"}/{Name}";
        /// <summary> Gets true if this is the role representing all users in a server. </summary>
        public bool IsEveryone => Id == Server.Id;
        /// <summary> Gets a list of all members in this role. </summary>
        public IEnumerable<User> Members => IsEveryone ? Server.Users : Server.Users.Where(x => x.HasRole(this));

        /// <summary> Gets the string used to mention this role. </summary>
        public string Mention => IsEveryone ? "@everyone" : IsMentionable ? $"<@&{Id}>" : "";

		internal Role(ulong id, Server server)
		{
            Id = id;
            Server = server;

			Permissions = new ServerPermissions(0);
			Color = new Color(0);
		}

		internal void Update(APIRole model, bool updatePermissions)
		{
            if (model.Name != null)
				Name = model.Name;
			if (model.Hoist != null)
				IsHoisted = model.Hoist.Value;
			if (model.Managed != null)
				IsManaged = model.Managed.Value;
			if (model.Mentionable != null)
				IsMentionable = model.Mentionable.Value;
			if (model.Position != null && !IsEveryone)
				Position = model.Position.Value;
			if (model.Color != null)
				Color = new Color(model.Color.Value);
            if (model.Permissions != null)
            {
                Permissions = new ServerPermissions(model.Permissions.Value);
                if (updatePermissions) //Dont update these during READY
                {                    
                    foreach (var member in Members)
                        Server.UpdatePermissions(member);
                }
            }
		}
        
        public async Task Edit(string name = null, ServerPermissions? permissions = null, Color color = null, bool? isHoisted = null, int? position = null, bool? isMentionable = null)
        {
            var updateRequest = new UpdateRoleRequest(Server.Id, Id)
            {
                Name = name ?? Name,
                Permissions = (permissions ?? Permissions).RawValue,
                Color = (color ?? Color).RawValue,
                IsHoisted = isHoisted ?? IsHoisted,
                IsMentionable = isMentionable ?? IsMentionable
            };

            var updateResponse = await Client.ClientAPI.Send(updateRequest).ConfigureAwait(false);

            if (position != null)
            {
                int oldPos = Position;
                int newPos = position.Value;
                int minPos;
                Role[] roles = Server.Roles.OrderBy(x => x.Position).ToArray();

                if (oldPos < newPos) //Moving Down
                {
                    minPos = oldPos;
                    for (int i = oldPos; i < newPos; i++)
                        roles[i] = roles[i + 1];
                    roles[newPos] = this;
                }
                else //(oldPos > newPos) Moving Up
                {
                    minPos = newPos;
                    for (int i = oldPos; i > newPos; i--)
                        roles[i] = roles[i - 1];
                    roles[newPos] = this;
                }

                var reorderRequest = new ReorderRolesRequest(Server.Id)
                {
                    RoleIds = roles.Skip(minPos).Select(x => x.Id).ToArray(),
                    StartPos = minPos
                };
                await Client.ClientAPI.Send(reorderRequest).ConfigureAwait(false);
            }
        }

        public async Task Delete()
        {
            try { await Client.ClientAPI.Send(new DeleteRoleRequest(Server.Id, Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        internal Role Clone()
        {
            var result = new Role();
            _cloner(this, result);
            return result;
        }
        private Role() { } //Used for cloning

        public override string ToString() => Name ?? Id.ToIdString();
    }
}
