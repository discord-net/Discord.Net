using System;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		/// <summary> Gets more info about the provided invite code. </summary>
		/// <remarks> Supported formats: inviteCode, xkcdCode, https://discord.gg/inviteCode, https://discord.gg/xkcdCode </remarks>
		public async Task<Invite> GetInvite(string inviteIdOrXkcd)
		{
			//This doesn't work well if it's an invite to a different server!

			if (inviteIdOrXkcd == null) throw new ArgumentNullException(nameof(inviteIdOrXkcd));
			CheckReady();

			//Remove trailing slash
			if (inviteIdOrXkcd.Length > 0 && inviteIdOrXkcd[inviteIdOrXkcd.Length - 1] == '/')
				inviteIdOrXkcd = inviteIdOrXkcd.Substring(0, inviteIdOrXkcd.Length - 1);
			//Remove leading URL
			int index = inviteIdOrXkcd.LastIndexOf('/');
			if (index >= 0)
				inviteIdOrXkcd = inviteIdOrXkcd.Substring(index + 1);

			var response = await _api.GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
			var invite = new Invite(this, response.Code, response.XkcdPass, response.Guild.Id, response.Inviter?.Id, response.Channel?.Id);
			invite.Cache(); //Builds references
			return invite;
		}

		/// <summary> Creates a new invite to the default channel of the provided server. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. Set to 0 to have unlimited uses. </param>
		public Task<Invite> CreateInvite(Server server, int maxAge = 1800, int maxUses = 0, bool tempMembership = false, bool hasXkcd = false)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			CheckReady();

			return CreateInvite(server.DefaultChannel, maxAge, maxUses, tempMembership, hasXkcd);
		}
		/// <summary> Creates a new invite to the provided channel. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. Set to 0 to have unlimited uses. </param>
		public async Task<Invite> CreateInvite(Channel channel, int maxAge = 1800, int maxUses = 0, bool tempMembership = false, bool hasXkcd = false)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (maxAge <= 0) throw new ArgumentOutOfRangeException(nameof(maxAge));
			if (maxUses <= 0) throw new ArgumentOutOfRangeException(nameof(maxUses));
			CheckReady();

			var response = await _api.CreateInvite(channel.Id, maxAge: maxAge, maxUses: maxUses, 
				tempMembership: tempMembership, hasXkcd: hasXkcd).ConfigureAwait(false);
			var invite = new Invite(this, response.Code, response.XkcdPass, response.Guild.Id, response.Inviter?.Id, response.Channel?.Id);
			invite.Cache(); //Builds references
			return invite;
		}

		/// <summary> Deletes the provided invite. </summary>
		public async Task DestroyInvite(Invite invite)
		{
			if (invite == null) throw new ArgumentNullException(nameof(invite));
			CheckReady();

			try { await _api.DeleteInvite(invite.Id).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
		
		/// <summary> Accepts the provided invite. </summary>
		public Task AcceptInvite(Invite invite)
		{
			if (invite == null) throw new ArgumentNullException(nameof(invite));
			CheckReady();

			return _api.AcceptInvite(invite.Id);
		}
	}
}