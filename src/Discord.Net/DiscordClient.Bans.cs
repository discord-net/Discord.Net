using Discord.Net;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		public event EventHandler<BanEventArgs> BanAdded;
		private void RaiseBanAdded(string userId, Server server)
		{
			if (BanAdded != null)
				RaiseEvent(nameof(BanAdded), () => BanAdded(this, new BanEventArgs(_users[userId], userId, server)));
		}
		public event EventHandler<BanEventArgs> BanRemoved;
		private void RaiseBanRemoved(string userId, Server server)
		{
			if (BanRemoved != null)
				RaiseEvent(nameof(BanRemoved), () => BanRemoved(this, new BanEventArgs(_users[userId], userId, server)));
		}

		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(Member member)
			=> Ban(member?.ServerId, member?.UserId);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(Server server, User user)
			=> Ban(server?.Id, user?.Id);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(Server server, string userId)
			=> Ban(server?.Id, userId);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(string server, User user)
			=> Ban(server, user?.Id);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(string serverId, string userId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _api.Ban(serverId, userId);
		}

		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(Member member)
			=> Unban(member?.ServerId, member?.UserId);
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(Server server, User user)
			=> Unban(server?.Id, user?.Id);
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(Server server, string userId)
			=> Unban(server?.Id, userId);
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(string server, User user)
			=> Unban(server, user?.Id);
		/// <summary> Unbans a user from the provided server. </summary>
		public async Task Unban(string serverId, string userId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			try { await _api.Unban(serverId, userId).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
	}
}