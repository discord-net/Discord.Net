using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Servers : AsyncCollection<long, Server>
	{
		public Servers(DiscordClient client, object writerLock)
			: base(client, writerLock) { }
		
		public Server GetOrAdd(long id)
			=> GetOrAdd(id, () => new Server(_client, id));
	}

	public class ServerEventArgs : EventArgs
	{
		public Server Server { get; }

		public ServerEventArgs(Server server) { Server = server; }
	}

	public partial class DiscordClient
	{
		public event AsyncEventHandler<ServerEventArgs> JoinedServer { add { _joinedServer.Add(value); } remove { _joinedServer.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _joinedServer = new AsyncEvent<ServerEventArgs>(nameof(JoinedServer));
		private Task RaiseJoinedServer(Server server)
			=> RaiseEvent(_joinedServer, new ServerEventArgs(server));

		public event AsyncEventHandler<ServerEventArgs> LeftServer { add { _leftServer.Add(value); } remove { _leftServer.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _leftServer = new AsyncEvent<ServerEventArgs>(nameof(LeftServer));
		private Task RaiseLeftServer(Server server)
			=> RaiseEvent(_leftServer, new ServerEventArgs(server));

		public event AsyncEventHandler<ServerEventArgs> ServerUpdated { add { _serverUpdated.Add(value); } remove { _serverUpdated.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _serverUpdated = new AsyncEvent<ServerEventArgs>(nameof(ServerUpdated));
		private Task RaiseServerUpdated(Server server)
			=> RaiseEvent(_serverUpdated, new ServerEventArgs(server));

		public event AsyncEventHandler<ServerEventArgs> ServerAvailable { add { _serverAvailable.Add(value); } remove { _serverAvailable.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _serverAvailable = new AsyncEvent<ServerEventArgs>(nameof(ServerAvailable));
		private Task RaiseServerAvailable(Server server)
			=> RaiseEvent(_serverAvailable, new ServerEventArgs(server));

		public event AsyncEventHandler<ServerEventArgs> ServerUnavailable { add { _serverUnavailable.Add(value); } remove { _serverUnavailable.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _serverUnavailable = new AsyncEvent<ServerEventArgs>(nameof(ServerUnavailable));
		private Task RaiseServerUnavailable(Server server)
			=> RaiseEvent(_serverUnavailable, new ServerEventArgs(server));


		/// <summary> Returns a collection of all servers this client is a member of. </summary>
		public IEnumerable<Server> AllServers { get { CheckReady(); return _servers; } }
		internal Servers Servers => _servers;
		private readonly Servers _servers;

		/// <summary> Returns the server with the specified id, or null if none was found. </summary>
		public Server GetServer(long id)
		{
			if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
			CheckReady();

			return _servers[id];
		}

		/// <summary> Returns all servers with the specified name. </summary>
		/// <remarks> Search is case-insensitive. </remarks>
		public IEnumerable<Server> FindServers(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			CheckReady();

			return _servers.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary> Creates a new server with the provided name and region (see Regions). </summary>
		public async Task<Server> CreateServer(string name, Region region)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (region == null) throw new ArgumentNullException(nameof(region));
			CheckReady();

			var response = await _api.CreateServer(name, region.Id).ConfigureAwait(false);
			var server = _servers.GetOrAdd(response.Id);
			server.Update(response);
			return server;
		}
		
		/// <summary> Edits the provided server, changing only non-null attributes. </summary>
		public async Task EditServer(Server server, string name = null, string region = null, Stream icon = null, ImageType iconType = ImageType.Png)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			CheckReady();

			var response = await _api.EditServer(server.Id, name: name ?? server.Name, region: region, icon: icon, iconType: iconType).ConfigureAwait(false);
			server.Update(response);
		}
		
		/// <summary> Leaves the provided server, destroying it if you are the owner. </summary>
		public async Task LeaveServer(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			CheckReady();

			try { await _api.LeaveServer(server.Id).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		public async Task<IEnumerable<Region>> GetVoiceRegions()
		{
			CheckReady();

			return (await _api.GetVoiceRegions()).Select(x => new Region { Id = x.Id, Name = x.Name, Hostname = x.Hostname, Port = x.Port });
		}
	}
}