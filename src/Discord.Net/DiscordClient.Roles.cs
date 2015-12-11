using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Roles : AsyncCollection<long, Role>
	{
		public Roles(DiscordClient client, object writerLock)
			: base(client, writerLock) { }
		
		public Role GetOrAdd(long id, long serverId)
			=> GetOrAdd(id, () => new Role(_client, id, serverId));
	}

	public class RoleEventArgs : EventArgs
	{
		public Role Role { get; }
		public Server Server => Role.Server;

		public RoleEventArgs(Role role) { Role = role; }
	}

	public partial class DiscordClient
	{
		public event EventHandler<RoleEventArgs> RoleCreated;
		private void RaiseRoleCreated(Role role)
		{
			if (RoleCreated != null)
				EventHelper.Raise(_logger, nameof(RoleCreated), () => RoleCreated(this, new RoleEventArgs(role)));
		}
		public event EventHandler<RoleEventArgs> RoleUpdated;
		private void RaiseRoleDeleted(Role role)
		{
			if (RoleDeleted != null)
				EventHelper.Raise(_logger, nameof(RoleDeleted), () => RoleDeleted(this, new RoleEventArgs(role)));
		}
		public event EventHandler<RoleEventArgs> RoleDeleted;
		private void RaiseRoleUpdated(Role role)
		{
			if (RoleUpdated != null)
				EventHelper.Raise(_logger, nameof(RoleUpdated), () => RoleUpdated(this, new RoleEventArgs(role)));
		}
		
		internal Roles Roles => _roles;
		private readonly Roles _roles;

		/// <summary> Returns the role with the specified id, or null if none was found. </summary>
		public Role GetRole(long id)
		{
			if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
			CheckReady();

			return _roles[id];
		}
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(Server server, string name)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));
			CheckReady();

			/*if (name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				return _roles.Where(x => x.Server.Id == server.Id &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || 
					string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{*/
				return _roles.Where(x => x.Server.Id == server.Id &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			//}
		}
		
		/// <summary> Note: due to current API limitations, the created role cannot be returned. </summary>
		public async Task<Role> CreateRole(Server server, string name)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));
			CheckReady();

			var response1 = await _api.CreateRole(server.Id).ConfigureAwait(false);
			var role = _roles.GetOrAdd(response1.Id, server.Id);
			role.Update(response1);

			//TODO: We shouldnt have to send permissions here, should be fixed on Discord's end soon
			var response2 = await _api.EditRole(role.Server.Id, role.Id,
				name: name,
				permissions: role.Permissions.RawValue).ConfigureAwait(false);
			role.Update(response2);

			return role;
		}
		
		public async Task EditRole(Role role, string name = null, ServerPermissions permissions = null, Color color = null, bool? hoist = null, int? position = null)
		{
			if (role == null) throw new ArgumentNullException(nameof(role));
			CheckReady();

			//TODO: check this null workaround later, should be fixed on Discord's end soon
			var response = await _api.EditRole(role.Server.Id, role.Id, 
				name: name ?? role.Name,
				permissions: (permissions ?? role.Permissions).RawValue, 
				color: (color ?? role.Color).RawValue, 
				hoist: hoist ?? role.IsHoisted).ConfigureAwait(false);

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
				await _api.ReorderRoles(role.Server.Id, roles.Skip(minPos).Select(x => x.Id), minPos).ConfigureAwait(false);
			}
		}

		public async Task DeleteRole(Role role)
		{
			if (role == null) throw new ArgumentNullException(nameof(role));
			CheckReady();

			try { await _api.DeleteRole(role.Server.Id, role.Id).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
	}

		public Task ReorderRoles(Server server, IEnumerable<Role> roles, int startPos = 0)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (roles == null) throw new ArgumentNullException(nameof(roles));
			if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");
			CheckReady();

			return _api.ReorderRoles(server.Id, roles.Select(x => x.Id), startPos);
		}
	}
}