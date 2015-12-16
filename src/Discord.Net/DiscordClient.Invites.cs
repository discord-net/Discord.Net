using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Linq;
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
			if (inviteIdOrXkcd == null) throw new ArgumentNullException(nameof(inviteIdOrXkcd));
			CheckReady();

			//Remove trailing slash
			if (inviteIdOrXkcd.Length > 0 && inviteIdOrXkcd[inviteIdOrXkcd.Length - 1] == '/')
				inviteIdOrXkcd = inviteIdOrXkcd.Substring(0, inviteIdOrXkcd.Length - 1);
			//Remove leading URL
			int index = inviteIdOrXkcd.LastIndexOf('/');
			if (index >= 0)
				inviteIdOrXkcd = inviteIdOrXkcd.Substring(index + 1);

			var response = await _rest.Send(new GetInviteRequest(inviteIdOrXkcd)).ConfigureAwait(false);
			var invite = new Invite(response.Code, response.XkcdPass);
			invite.Update(response);
            return invite;
		}

		/// <summary> Gets all active (non-expired) invites to a provided server. </summary>
		public async Task<Invite[]> GetInvites(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			CheckReady();

			var response = await _rest.Send(new GetInvitesRequest(server.Id)).ConfigureAwait(false);
			return response.Select(x =>
			{
				var invite = new Invite(x.Code, x.XkcdPass);
				invite.Update(x);
				return invite;
			}).ToArray();
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
		public async Task<Invite> CreateInvite(Channel channel, int maxAge = 1800, int maxUses = 0, bool isTemporary = false, bool withXkcd = false)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (maxAge < 0) throw new ArgumentOutOfRangeException(nameof(maxAge));
			if (maxUses < 0) throw new ArgumentOutOfRangeException(nameof(maxUses));
			CheckReady();

            var request = new CreateInviteRequest(channel.Id)
            {
                MaxAge = maxAge,
                MaxUses = maxUses,
                IsTemporary = isTemporary,
                WithXkcdPass = withXkcd
            };

            var response = await _rest.Send(request).ConfigureAwait(false);
			var invite = new Invite(response.Code, response.XkcdPass);
			return invite;
		}

		/// <summary> Deletes the provided invite. </summary>
		public async Task DeleteInvite(Invite invite)
		{
			if (invite == null) throw new ArgumentNullException(nameof(invite));
			CheckReady();

			try { await _rest.Send(new DeleteInviteRequest(invite.Code)).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
		
		/// <summary> Accepts the provided invite. </summary>
		public Task AcceptInvite(Invite invite)
		{
			if (invite == null) throw new ArgumentNullException(nameof(invite));
			CheckReady();

			return _rest.Send(new AcceptInviteRequest(invite.Code));
		}
	}
}