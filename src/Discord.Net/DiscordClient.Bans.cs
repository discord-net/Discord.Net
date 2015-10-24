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
		public Task Ban(Member member)
		{
			if (member == null) throw new ArgumentNullException(nameof(member));
			CheckReady();

			return _api.Ban(member.ServerId, member.Id);
		}

		/// <summary> Unbans a user from the provided server. </summary>
		public async Task Unban(Member member)
		{
			if (member == null) throw new ArgumentNullException(nameof(member));
			CheckReady();

			try { await _api.Unban(member.ServerId, member.Id).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
	}
}