using Discord.Net;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public class BanEventArgs : EventArgs
	{
		public string UserId { get; }
		public Server Server { get; }

		internal BanEventArgs(string userId, Server server)
		{
			UserId = userId;
			Server = server;
		}
	}

	public partial class DiscordClient
	{
		public event EventHandler<BanEventArgs> BanAdded;
		private void RaiseBanAdded(string userId, Server server)
		{
			if (BanAdded != null)
				RaiseEvent(nameof(BanAdded), () => BanAdded(this, new BanEventArgs(userId, server)));
		}
		public event EventHandler<BanEventArgs> BanRemoved;
		private void RaiseBanRemoved(string userId, Server server)
		{
			if (BanRemoved != null)
				RaiseEvent(nameof(BanRemoved), () => BanRemoved(this, new BanEventArgs(userId, server)));
		}

		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(User member)
		{
			if (member == null) throw new ArgumentNullException(nameof(member));
			if (member.Server == null) throw new ArgumentException("Unable to ban a user in a private chat.");
			CheckReady();

			return _api.Ban(member.Server.Id, member.Id);
		}

		/// <summary> Unbans a user from the provided server. </summary>
		public async Task Unban(Server server, string userId)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (userId == null) throw new ArgumentNullException(nameof(userId));
			CheckReady();

			try { await _api.Unban(server.Id, userId).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
	}
}