using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.API
{
	internal class DiscordAPIClient
	{
		public const int MaxMessageSize = 2000;

		public RestClient RestClient => _rest;
		private readonly RestClient _rest;

		public DiscordAPIClient(LogMessageSeverity logLevel, int timeout)
		{
			_rest = new RestClient(logLevel, timeout);
        }

		private string _token;
		public string Token
		{
			get { return _token; }
			set { _token = value; _rest.SetToken(value); }
		}
		private CancellationToken _cancelToken;
		public CancellationToken CancelToken
		{
			get { return _cancelToken; }
			set { _cancelToken = value; _rest.SetCancelToken(value); }
		}

		//Auth
		public Task<Responses.Gateway> GetWebSocketEndpoint()
			=> _rest.Get<Responses.Gateway>(Endpoints.Gateway);
        public async Task<Responses.AuthRegister> LoginAnonymous(string username)
		{
			var fingerprintResponse = await _rest.Post<Responses.AuthFingerprint>(Endpoints.AuthFingerprint).ConfigureAwait(false);
			var registerRequest = new Requests.AuthRegister { Fingerprint = fingerprintResponse.Fingerprint, Username = username };
			var registerResponse = await _rest.Post<Responses.AuthRegister>(Endpoints.AuthRegister, registerRequest).ConfigureAwait(false);
			return registerResponse;
        }
		public async Task<Responses.AuthLogin> Login(string email, string password)
		{
			var request = new Requests.AuthLogin { Email = email, Password = password };
			var response = await _rest.Post<Responses.AuthLogin>(Endpoints.AuthLogin, request).ConfigureAwait(false);
			return response;
		}
		public Task Logout()
			=> _rest.Post(Endpoints.AuthLogout);

		//Servers
		public Task<Responses.CreateServer> CreateServer(string name, string region)
		{
			var request = new Requests.CreateServer { Name = name, Region = region };
			return _rest.Post<Responses.CreateServer>(Endpoints.Servers, request);
        }
		public Task LeaveServer(string id)
			=> _rest.Delete<Responses.DeleteServer>(Endpoints.Server(id));

		//Channels
        public Task<Responses.CreateChannel> CreateChannel(string serverId, string name, string channelType)
		{
			var request = new Requests.CreateChannel { Name = name, Type = channelType };
			return _rest.Post<Responses.CreateChannel>(Endpoints.ServerChannels(serverId), request);
		}
		public Task<Responses.CreateChannel> CreatePMChannel(string myId, string recipientId)
		{
			var request = new Requests.CreatePMChannel { RecipientId = recipientId };
			return _rest.Post<Responses.CreateChannel>(Endpoints.UserChannels(myId), request);
		}
		public Task<Responses.DestroyChannel> DestroyChannel(string channelId)
			=> _rest.Delete<Responses.DestroyChannel>(Endpoints.Channel(channelId));
		public Task<Responses.GetMessages[]> GetMessages(string channelId, int count)
			=> _rest.Get<Responses.GetMessages[]>(Endpoints.ChannelMessages(channelId, count));

		//Members
		public Task Kick(string serverId, string userId)
			=> _rest.Delete(Endpoints.ServerMember(serverId, userId));
		public Task Ban(string serverId, string userId)
			=> _rest.Put(Endpoints.ServerBan(serverId, userId));
		public Task Unban(string serverId, string userId)
			=> _rest.Delete(Endpoints.ServerBan(serverId, userId));
		public Task SetMemberRoles(string serverId, string userId, string[] roles)
		{
			var request = new Requests.ModifyMember { Roles = roles };
			return _rest.Patch(Endpoints.ServerMember(serverId, userId));
		}

		//Invites
		public Task<Responses.CreateInvite> CreateInvite(string channelId, int maxAge, int maxUses, bool isTemporary, bool withXkcdPass)
		{
			var request = new Requests.CreateInvite { MaxAge = maxAge, MaxUses = maxUses, IsTemporary = isTemporary, WithXkcdPass = withXkcdPass };
            return _rest.Post<Responses.CreateInvite>(Endpoints.ChannelInvites(channelId), request);
		}
		public Task<Responses.GetInvite> GetInvite(string id)
			=> _rest.Get<Responses.GetInvite>(Endpoints.Invite(id));
		public Task AcceptInvite(string id)
			=> _rest.Post<Responses.AcceptInvite>(Endpoints.Invite(id));
		public Task DeleteInvite(string id)
			=> _rest.Delete(Endpoints.Invite(id));

		//Roles
		public Task CreateRole(string serverId)
		{
			//TODO: Return a result when Discord starts giving us one
			return _rest.Post(Endpoints.ServerRoles(serverId));
		}
		public Task RenameRole(string serverId, string roleId, string newName)
		{
			var request = new Requests.ModifyRole { Name = newName };
			return _rest.Patch(Endpoints.ServerRole(serverId, roleId), request);
		}
		public Task SetRolePermissions(string serverId, string roleId, PackedPermissions permissions)
		{
			var request = new Requests.ModifyRole { Permissions = permissions.RawValue };
			return _rest.Patch(Endpoints.ServerRole(serverId, roleId), request);
		}
		public Task DeleteRole(string serverId, string roleId)
		{
			return _rest.Delete(Endpoints.ServerRole(serverId, roleId));
		}

		//Permissions
		public Task SetChannelPermissions(string channelId, string userOrRoleId, string idType, PackedPermissions allow, PackedPermissions deny)
		{
			var request = new Requests.SetChannelPermissions { Id = userOrRoleId, Type = idType, Allow = allow.RawValue, Deny = deny.RawValue };
			return _rest.Put(Endpoints.ChannelPermission(channelId, userOrRoleId), request);
		}
		public Task DeleteChannelPermissions(string channelId, string userOrRoleId)
		{
			return _rest.Delete(Endpoints.ChannelPermission(channelId, userOrRoleId), null);
		}

		//Chat
		public Task<Responses.SendMessage> SendMessage(string channelId, string message, string[] mentions, string nonce, bool isTTS)
		{
			var request = new Requests.SendMessage { Content = message, Mentions = mentions, Nonce = nonce, IsTTS = isTTS };
			return _rest.Post<Responses.SendMessage>(Endpoints.ChannelMessages(channelId), request);
		}
		public Task<Responses.EditMessage> EditMessage(string messageId, string channelId, string message, string[] mentions)
		{
			var request = new Requests.EditMessage { Content = message, Mentions = mentions };
			return _rest.Patch<Responses.EditMessage>(Endpoints.ChannelMessage(channelId, messageId), request);
		}
		public Task SendIsTyping(string channelId)
			=> _rest.Post(Endpoints.ChannelTyping(channelId));
		public Task DeleteMessage(string channelId, string msgId)
			=> _rest.Delete(Endpoints.ChannelMessage(channelId, msgId));
		public Task SendFile(string channelId, string filePath)
			=> _rest.PostFile<Responses.SendMessage>(Endpoints.ChannelMessages(channelId), filePath);

		//Voice
		public Task<Responses.GetRegions[]> GetVoiceRegions()
			=> _rest.Get<Responses.GetRegions[]>(Endpoints.VoiceRegions);
		public Task<Responses.GetIce> GetVoiceIce()
			=> _rest.Get<Responses.GetIce>(Endpoints.VoiceIce);
		public Task Mute(string serverId, string memberId)
		{
			var request = new Requests.SetMemberMute { Value = true };
			return _rest.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public Task Unmute(string serverId, string memberId)
		{
			var request = new Requests.SetMemberMute { Value = false };
			return _rest.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public Task Deafen(string serverId, string memberId)
		{
			var request = new Requests.SetMemberDeaf { Value = true };
			return _rest.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public Task Undeafen(string serverId, string memberId)
		{
			var request = new Requests.SetMemberDeaf { Value = false };
			return _rest.Patch(Endpoints.ServerMember(serverId, memberId));
		}

		//Profile
		public Task<Responses.ChangeProfile> ChangeUsername(string newUsername, string currentEmail, string currentPassword)
		{
			var request = new Requests.ChangeUsername { Username = newUsername, CurrentEmail = currentEmail, CurrentPassword = currentPassword };
			return _rest.Patch<Responses.ChangeProfile>(Endpoints.UserMe, request);
		}
		public Task<Responses.ChangeProfile> ChangeEmail(string newEmail, string currentPassword)
		{
			var request = new Requests.ChangeEmail { NewEmail = newEmail, CurrentPassword = currentPassword };
			return _rest.Patch<Responses.ChangeProfile>(Endpoints.UserMe, request);
		}
		public Task<Responses.ChangeProfile> ChangePassword(string newPassword, string currentEmail, string currentPassword)
		{
			var request = new Requests.ChangePassword { NewPassword = newPassword, CurrentEmail = currentEmail, CurrentPassword = currentPassword };
			return _rest.Patch<Responses.ChangeProfile>(Endpoints.UserMe, request);
		}
		public Task<Responses.ChangeProfile> ChangeAvatar(AvatarImageType imageType, byte[] bytes, string currentEmail, string currentPassword)
		{
			string base64 = Convert.ToBase64String(bytes);
			string type = imageType == AvatarImageType.Jpeg ? "image/jpeg;base64" : "image/png;base64";
			var request = new Requests.ChangeAvatar { Avatar = $"data:{type},/9j/{base64}", CurrentEmail = currentEmail, CurrentPassword = currentPassword };
			return _rest.Patch<Responses.ChangeProfile>(Endpoints.UserMe, request);
		}

		//Other
		/*public Task<Responses.Status> GetUnresolvedIncidents()
		{
			return _rest.Get<Responses.Status>(Endpoints.StatusUnresolvedMaintenance);
		}
		public Task<Responses.Status> GetActiveIncidents()
		{
			return _rest.Get<Responses.Status>(Endpoints.StatusActiveMaintenance);
		}*/
	}
}
