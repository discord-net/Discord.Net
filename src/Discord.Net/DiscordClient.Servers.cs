using Discord.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Servers : AsyncCollection<ulong, Server>
	{
		public Servers(DiscordClient client, object writerLock)
			: base(client, writerLock) { }
		
		public Server GetOrAdd(ulong id)
			=> GetOrAdd(id, () => new Server(_client, id));
	}

	public class ServerEventArgs : EventArgs
	{
		public Server Server { get; }

		public ServerEventArgs(Server server) { Server = server; }
	}

	public partial class DiscordClient
	{
		public event EventHandler<ServerEventArgs> JoinedServer;
		private void RaiseJoinedServer(Server server)
		{
			if (JoinedServer != null)
				EventHelper.Raise(_logger, nameof(JoinedServer), () => JoinedServer(this, new ServerEventArgs(server)));
		}
		public event EventHandler<ServerEventArgs> LeftServer;
		private void RaiseLeftServer(Server server)
		{
			if (LeftServer != null)
				EventHelper.Raise(_logger, nameof(LeftServer), () => LeftServer(this, new ServerEventArgs(server)));
		}
		public event EventHandler<ServerEventArgs> ServerUpdated;
		private void RaiseServerUpdated(Server server)
		{
			if (ServerUpdated != null)
				EventHelper.Raise(_logger, nameof(ServerUpdated), () => ServerUpdated(this, new ServerEventArgs(server)));
		}
		public event EventHandler<ServerEventArgs> ServerUnavailable;
		private void RaiseServerUnavailable(Server server)
		{
			if (ServerUnavailable != null)
				EventHelper.Raise(_logger, nameof(ServerUnavailable), () => ServerUnavailable(this, new ServerEventArgs(server)));
		}
		public event EventHandler<ServerEventArgs> ServerAvailable;
		private void RaiseServerAvailable(Server server)
		{
			if (ServerAvailable != null)
				EventHelper.Raise(_logger, nameof(ServerAvailable), () => ServerAvailable(this, new ServerEventArgs(server)));
		}

		/// <summary> Returns a collection of all servers this client is a member of. </summary>
		public IEnumerable<Server> AllServers { get { CheckReady(); return _servers; } }
		internal Servers Servers => _servers;
		private readonly Servers _servers;

		/// <summary> Returns the server with the specified id, or null if none was found. </summary>
		public Server GetServer(ulong id)
		{
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

			var response = await _api.EditServer(
                server.Id, name: name ?? server.Name, 
                region: region ?? server.Region,
                icon: icon, 
                iconType: iconType,
                existingIcon: server.IconId,
                afkChannelId: server.AFKChannel?.Id,
                afkTimeout: server.AFKTimeout).ConfigureAwait(false);
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

            var regions = await _api.GetVoiceRegions().ConfigureAwait(false);
            return regions.Select(x => new Region { Id = x.Id, Name = x.Name, Hostname = x.Hostname, Port = x.Port });
		}
	}
}