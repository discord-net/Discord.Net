using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Roles : AsyncCollection<Role>
	{
		private const string VirtualEveryoneId = "[Virtual]";
		public Role VirtualEveryone { get; private set; }

		public Roles(DiscordClient client, object writerLock)
			: base(client, writerLock, x => x.OnCached(), x => x.OnUncached()) { }

		internal Role CreateVirtualRole(string serverId, string name)
		{
			var role = new Role(_client, serverId, serverId);
			_dictionary[serverId] = role;
			role.Update(new API.RoleInfo
			{
				Id = serverId,
				Name = name,
				Permissions = ServerPermissions.None.RawValue
			});
			return role;
		}

		public Role GetOrAdd(string id, string serverId)
			=> GetOrAdd(id, () => new Role(_client, id, serverId));
	}

	public class RoleEventArgs : EventArgs
	{
		public Role Role { get; }
		public string RoleId => Role.Id;
		public Server Server => Role.Server;
		public string ServerId => Role.ServerId;

		internal RoleEventArgs(Role role) { Role = role; }
	}

	public partial class DiscordClient
	{
		public event EventHandler<RoleEventArgs> RoleCreated;
		private void RaiseRoleCreated(Role role)
		{
			if (RoleCreated != null)
				RaiseEvent(nameof(RoleCreated), () => RoleCreated(this, new RoleEventArgs(role)));
		}
		public event EventHandler<RoleEventArgs> RoleUpdated;
		private void RaiseRoleDeleted(Role role)
		{
			if (RoleDeleted != null)
				RaiseEvent(nameof(RoleDeleted), () => RoleDeleted(this, new RoleEventArgs(role)));
		}
		public event EventHandler<RoleEventArgs> RoleDeleted;
		private void RaiseRoleUpdated(Role role)
		{
			if (RoleUpdated != null)
				RaiseEvent(nameof(RoleUpdated), () => RoleUpdated(this, new RoleEventArgs(role)));
		}
		
		internal Roles Roles => _roles;
		private readonly Roles _roles;

		/// <summary> Returns the role with the specified id, or null if none was found. </summary>
		public Role GetRole(string id) => _roles[id];
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(Server server, string name) => FindRoles(server?.Id, name);
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(string serverId, string name)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (name == null) throw new ArgumentNullException(nameof(name));

			if (name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				return _roles.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				return _roles.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}
		}

		/// <summary> Note: due to current API limitations, the created role cannot be returned. </summary>
		public Task<Role> CreateRole(Server server, string name)
			=> CreateRole(server?.Id, name);
		/// <summary> Note: due to current API limitations, the created role cannot be returned. </summary>
		public async Task<Role> CreateRole(string serverId, string name)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			var response = await _api.CreateRole(serverId).ConfigureAwait(false);
			var role = _roles.GetOrAdd(response.Id, serverId);
			role.Update(response);

			await EditRole(role, name: name);

			return role;
		}

		public Task EditRole(string roleId, string name = null, ServerPermissions permissions = null, Color color = null, bool? hoist = null, int? position = null)
			=> EditRole(_roles[roleId], name: name, permissions: permissions, color: color, hoist: hoist, position: position);
		public async Task EditRole(Role role, string name = null, ServerPermissions permissions = null, Color color = null, bool? hoist = null, int? position = null)
		{
            CheckReady();
			if (role == null) throw new ArgumentNullException(nameof(role));

			//TODO: check this null workaround later, should be fixed on Discord's end soon
			var response = await _api.EditRole(role.ServerId, role.Id, 
				name: name ?? role.Name,
				permissions: permissions?.RawValue ?? role.Permissions.RawValue, 
				color: color?.RawValue, 
				hoist: hoist);

			if (position != null)
			{
				int oldPos = role.Position;
				int newPos = position.Value;
				int minPos;
				Role[] roles = role.Server.Roles.OrderBy(x => x.Position).ToArray();

				if (oldPos < newPos) //Moving Down
				{
					minPos = oldPos;
					for (int i = oldPos; i < newPos; i++)
						roles[i] = roles[i + 1];
					roles[newPos] = role;
				}
				else //(oldPos > newPos) Moving Up
				{
					minPos = newPos;
					for (int i = oldPos; i > newPos; i--)
						roles[i] = roles[i - 1];
					roles[newPos] = role;
				}
				await _api.ReorderRoles(role.ServerId, roles.Skip(minPos).Select(x => x.Id), minPos);
			}
		}

		public Task DeleteRole(Role role)
			=> DeleteRole(role?.ServerId, role?.Id);
		public Task DeleteRole(string serverId, string roleId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (roleId == null) throw new ArgumentNullException(nameof(roleId));

			return _api.DeleteRole(serverId, roleId);
		}

		public Task ReorderRoles(Server server, IEnumerable<object> roles, int startPos = 0)
			=> ReorderChannels(server.Id, roles, startPos);
		public Task ReorderRoles(string serverId, IEnumerable<object> roles, int startPos = 0)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (roles == null) throw new ArgumentNullException(nameof(roles));
			if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");

			var roleIds = roles.Select(x =>
			{
				if (x is string)
					return x as string;
				else if (x is Role)
					return (x as Role).Id;
				else
					throw new ArgumentException("Channels must be a collection of string or Role.", nameof(roles));
			});

			return _api.ReorderRoles(serverId, roleIds, startPos);
		}
	}
}