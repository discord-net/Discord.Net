using Discord.Net;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		/// <summary> Creates a new invite to the default channel of the provided server. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. Set to 0 to have unlimited uses. </param>
		public Task<Invite> CreateInvite(Server server, int maxAge = 1800, int maxUses = 0, bool tempMembership = false, bool hasXkcd = false)
			=> CreateInvite(server?.DefaultChannelId, maxAge, maxUses, tempMembership, hasXkcd);
		/// <summary> Creates a new invite to the provided channel. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. Set to 0 to have unlimited uses. </param>
		public Task<Invite> CreateInvite(Channel channel, int maxAge = 1800, int maxUses = 0, bool tempMembership = false, bool hasXkcd = false)
			=> CreateInvite(channel?.Id, maxAge, maxUses, tempMembership, hasXkcd);
		/// <summary> Creates a new invite to the provided channel. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. Set to 0 to have unlimited uses. </param>
		public async Task<Invite> CreateInvite(string serverOrChannelId, int maxAge = 1800, int maxUses = 0, bool tempMembership = false, bool hasXkcd = false)
		{
			CheckReady();
			if (serverOrChannelId == null) throw new ArgumentNullException(nameof(serverOrChannelId));
			if (maxAge <= 0) throw new ArgumentOutOfRangeException(nameof(maxAge));
			if (maxUses <= 0) throw new ArgumentOutOfRangeException(nameof(maxUses));

			var response = await _api.CreateInvite(serverOrChannelId, maxAge, maxUses, tempMembership, hasXkcd).ConfigureAwait(false);
			var invite = new Invite(this, response.Code, response.XkcdPass, response.Guild.Id);
			invite.Update(response);
			return invite;
		}

		/// <summary> Deletes the provided invite. </summary>
		public async Task DestroyInvite(string inviteId)
		{
			CheckReady();
			if (inviteId == null) throw new ArgumentNullException(nameof(inviteId));

			try
			{
				//Check if this is a human-readable link and get its ID
				var response = await _api.GetInvite(inviteId).ConfigureAwait(false);
				await _api.DeleteInvite(response.Code).ConfigureAwait(false);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		/// <summary> Gets more info about the provided invite code. </summary>
		/// <remarks> Supported formats: inviteCode, xkcdCode, https://discord.gg/inviteCode, https://discord.gg/xkcdCode </remarks>
		public async Task<Invite> GetInvite(string inviteIdOrXkcd)
		{
			CheckReady();
			if (inviteIdOrXkcd == null) throw new ArgumentNullException(nameof(inviteIdOrXkcd));

			var response = await _api.GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
			var invite = new Invite(this, response.Code, response.XkcdPass, response.Guild.Id);
			invite.Update(response);
			return invite;
		}

		/// <summary> Accepts the provided invite. </summary>
		public Task AcceptInvite(Invite invite)
		{
			CheckReady();
			if (invite == null) throw new ArgumentNullException(nameof(invite));

			return _api.AcceptInvite(invite.Id);
		}
		/// <summary> Accepts the provided invite. </summary>
		public async Task AcceptInvite(string inviteId)
		{
			CheckReady();
			if (inviteId == null) throw new ArgumentNullException(nameof(inviteId));

			//Remove trailing slash and any non-code url parts
			if (inviteId.Length > 0 && inviteId[inviteId.Length - 1] == '/')
				inviteId = inviteId.Substring(0, inviteId.Length - 1);
			int index = inviteId.LastIndexOf('/');
			if (index >= 0)
				inviteId = inviteId.Substring(index + 1);

			//Check if this is a human-readable link and get its ID
			var invite = await GetInvite(inviteId).ConfigureAwait(false);
			await _api.AcceptInvite(invite.Id).ConfigureAwait(false);
		}
	}
}