using Discord.API;
using Discord.Net;
using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	/// <summary> A lightweight wrapper around the Discord API. </summary>
	public class DiscordAPIClient
	{
		private readonly DiscordAPIClientConfig _config;

		internal RestClient RestClient => _rest;
		private readonly RestClient _rest;

		public DiscordAPIClient(DiscordAPIClientConfig config = null)
		{
			_config = config ?? new DiscordAPIClientConfig();
            _rest = new RestClient(_config);
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
		public Task<GatewayResponse> Gateway()
			=> _rest.Get<GatewayResponse>(Endpoints.Gateway);
		public async Task<LoginResponse> Login(string email, string password)
		{
			if (email == null) throw new ArgumentNullException(nameof(email));
			if (password == null) throw new ArgumentNullException(nameof(password));

			var request = new LoginRequest { Email = email, Password = password };
			return await _rest.Post<LoginResponse>(Endpoints.AuthLogin, request).ConfigureAwait(false);
		}
		public Task Logout()
			=> _rest.Post(Endpoints.AuthLogout);

		//Channels
        public Task<CreateChannelResponse> CreateChannel(string serverId, string name, string channelType)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (channelType == null) throw new ArgumentNullException(nameof(channelType));

			var request = new CreateChannelRequest { Name = name, Type = channelType };
			return _rest.Post<CreateChannelResponse>(Endpoints.ServerChannels(serverId), request);
		}
		public Task<CreateChannelResponse> CreatePMChannel(string myId, string recipientId)
		{
			if (myId == null) throw new ArgumentNullException(nameof(myId));
			if (recipientId == null) throw new ArgumentNullException(nameof(recipientId));

			var request = new CreatePMChannelRequest { RecipientId = recipientId };
			return _rest.Post<CreateChannelResponse>(Endpoints.UserChannels(myId), request);
		}
		public Task<DestroyChannelResponse> DestroyChannel(string channelId)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			return _rest.Delete<DestroyChannelResponse>(Endpoints.Channel(channelId));
		}
		public Task<EditChannelResponse> EditChannel(string channelId, string name = null, string topic = null)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			var request = new EditChannelRequest { Name = name, Topic = topic };
			return _rest.Patch<EditChannelResponse>(Endpoints.Channel(channelId), request);
		}
		public Task ReorderChannels(string serverId, IEnumerable<string> channelIds, int startPos = 0)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (channelIds == null) throw new ArgumentNullException(nameof(channelIds));
			if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");

			uint pos = (uint)startPos;
			var channels = channelIds.Select(x => new ReorderChannelsRequest.Channel { Id = x, Position = pos++ });
			var request = new ReorderChannelsRequest(channels);
			return _rest.Patch(Endpoints.ServerChannels(serverId), request);
		}
		public Task<GetMessagesResponse> GetMessages(string channelId, int count)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			return _rest.Get<GetMessagesResponse>(Endpoints.ChannelMessages(channelId, count));
		}

		//Incidents
		public Task<GetIncidentsResponse> GetActiveIncidents()
		{
			return _rest.Get<GetIncidentsResponse>(Endpoints.StatusActiveMaintenance);
		}
		public Task<GetIncidentsResponse> GetUpcomingIncidents()
		{
			return _rest.Get<GetIncidentsResponse>(Endpoints.StatusUpcomingMaintenance);
		}

		//Invites
		public Task<CreateInviteResponse> CreateInvite(string channelId, int maxAge, int maxUses, bool tempMembership, bool hasXkcd)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			var request = new CreateInviteRequest { MaxAge = maxAge, MaxUses = maxUses, IsTemporary = tempMembership, WithXkcdPass = hasXkcd };
			return _rest.Post<CreateInviteResponse>(Endpoints.ChannelInvites(channelId), request);
		}
		public Task<GetInviteResponse> GetInvite(string inviteIdOrXkcd)
		{
			if (inviteIdOrXkcd == null) throw new ArgumentNullException(nameof(inviteIdOrXkcd));

			return _rest.Get<GetInviteResponse>(Endpoints.Invite(inviteIdOrXkcd));
		}
		public Task<AcceptInviteResponse> AcceptInvite(string inviteId)
		{
			if (inviteId == null) throw new ArgumentNullException(nameof(inviteId));

			return _rest.Post<AcceptInviteResponse>(Endpoints.Invite(inviteId));
		}
		public Task DeleteInvite(string inviteId)
		{
			if (inviteId == null) throw new ArgumentNullException(nameof(inviteId));

			return _rest.Delete(Endpoints.Invite(inviteId));
		}

		//Users
		public Task EditUser(string serverId, string userId, bool? mute = null, bool? deaf = null, IEnumerable<string> roles = null)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			var request = new EditMemberRequest { Mute = mute, Deaf = deaf, Roles = roles };
			return _rest.Patch(Endpoints.ServerMember(serverId, userId), request);
		}
		public Task KickUser(string serverId, string userId)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _rest.Delete(Endpoints.ServerMember(serverId, userId));
		}
		public Task BanUser(string serverId, string userId)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _rest.Put(Endpoints.ServerBan(serverId, userId));
		}
		public Task UnbanUser(string serverId, string userId)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _rest.Delete(Endpoints.ServerBan(serverId, userId));
		}
		public async Task<int> PruneUsers(string serverId, int days, bool simulate)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (days <= 0) throw new ArgumentOutOfRangeException(nameof(days));

			PruneUsersResponse response;
            if (simulate)
				response = await _rest.Get<PruneUsersResponse>(Endpoints.ServerPrune(serverId, days));
			else
				response = await _rest.Post<PruneUsersResponse>(Endpoints.ServerPrune(serverId, days));
			return response.Pruned ?? 0;
        }

		//Messages
		public Task<SendMessageResponse> SendMessage(string channelId, string message, IEnumerable<string> mentionedUserIds = null, string nonce = null, bool isTTS = false)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (message == null) throw new ArgumentNullException(nameof(message));

			var request = new SendMessageRequest { Content = message, Mentions = mentionedUserIds ?? new string[0], Nonce = nonce, IsTTS = isTTS ? true : false };
			return _rest.Post<SendMessageResponse>(Endpoints.ChannelMessages(channelId), request);
		}
		public Task<SendMessageResponse> SendFile(string channelId, string filePath)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (filePath == null) throw new ArgumentNullException(nameof(filePath));

			return _rest.PostFile<SendMessageResponse>(Endpoints.ChannelMessages(channelId), filePath);
		}
		public Task DeleteMessage(string messageId, string channelId)
		{
			if (messageId == null) throw new ArgumentNullException(nameof(messageId));
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			return _rest.Delete(Endpoints.ChannelMessage(channelId, messageId));
		}
		public Task<EditMessageResponse> EditMessage(string messageId, string channelId, string message = null, IEnumerable<string> mentionedUserIds = null)
		{
			if (messageId == null) throw new ArgumentNullException(nameof(messageId));
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			var request = new EditMessageRequest { Content = message, Mentions = mentionedUserIds };
			return _rest.Patch<EditMessageResponse>(Endpoints.ChannelMessage(channelId, messageId), request);
		}
		public Task AckMessage(string messageId, string channelId)
		{
			if (messageId == null) throw new ArgumentNullException(nameof(messageId));
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			return _rest.Post(Endpoints.ChannelMessageAck(channelId, messageId));
		}
        public Task SendIsTyping(string channelId)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			return _rest.Post(Endpoints.ChannelTyping(channelId));
		}

		//Permissions
		public Task SetChannelPermissions(string channelId, string userOrRoleId, string idType, uint allow = 0, uint deny = 0)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (userOrRoleId == null) throw new ArgumentNullException(nameof(userOrRoleId));
			if (idType == null) throw new ArgumentNullException(nameof(idType));

			var request = new SetChannelPermissionsRequest { Id = userOrRoleId, Type = idType, Allow = allow, Deny = deny };
			return _rest.Put(Endpoints.ChannelPermission(channelId, userOrRoleId), request);
		}
		public Task DeleteChannelPermissions(string channelId, string userOrRoleId)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (userOrRoleId == null) throw new ArgumentNullException(nameof(userOrRoleId));

			return _rest.Delete(Endpoints.ChannelPermission(channelId, userOrRoleId), null);
		}

		//Roles
		public Task<RoleInfo> CreateRole(string serverId)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			
			return _rest.Post<RoleInfo>(Endpoints.ServerRoles(serverId));
		}
		public Task DeleteRole(string serverId, string roleId)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (roleId == null) throw new ArgumentNullException(nameof(roleId));

			return _rest.Delete(Endpoints.ServerRole(serverId, roleId));
		}
		public Task<RoleInfo> EditRole(string serverId, string roleId, string name = null, uint? permissions = null, uint? color = null, bool? hoist = null)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (roleId == null) throw new ArgumentNullException(nameof(roleId));

			var request = new EditRoleRequest { Name = name, Permissions = permissions, Hoist = hoist, Color = color };
			return _rest.Patch<RoleInfo>(Endpoints.ServerRole(serverId, roleId), request);
		}
		public Task ReorderRoles(string serverId, IEnumerable<string> roleIds, int startPos = 0)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (roleIds == null) throw new ArgumentNullException(nameof(roleIds));
			if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");

			uint pos = (uint)startPos;
			var roles = roleIds.Select(x => new ReorderRolesRequest.Role { Id = x, Position = pos++ });
			var request = new ReorderRolesRequest(roles);
			return _rest.Patch(Endpoints.ServerRoles(serverId), request);
		}

		//Servers
		public Task<CreateServerResponse> CreateServer(string name, string region)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (region == null) throw new ArgumentNullException(nameof(region));

			var request = new CreateServerRequest { Name = name, Region = region };
			return _rest.Post<CreateServerResponse>(Endpoints.Servers, request);
		}
		public Task LeaveServer(string serverId)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			return _rest.Delete<DeleteServerResponse>(Endpoints.Server(serverId));
		}
		public Task<EditServerResponse> EditServer(string serverId, string name = null, string region = null, ImageType iconType = ImageType.Png, byte[] icon = null)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			var request = new EditServerRequest { Name = name, Region = region, Icon = Base64Picture(iconType, icon) };
			return _rest.Patch<EditServerResponse>(Endpoints.Server(serverId), request);
		}

		//User
		public Task<EditUserResponse> EditUser(string currentPassword = "",
			string username = null, string email = null, string password = null,
			ImageType avatarType = ImageType.Png, byte[] avatar = null)
		{
			if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));
			
			var request = new EditUserRequest { CurrentPassword = currentPassword, Username = username, Email = email, Password = password, Avatar = Base64Picture(avatarType, avatar) };
			return _rest.Patch<EditUserResponse>(Endpoints.UserMe, request);
		}

		//Voice
		public Task<GetRegionsResponse> GetVoiceRegions()
			=> _rest.Get<GetRegionsResponse>(Endpoints.VoiceRegions);
		/*public Task<GetIceResponse> GetVoiceIce()
			=> _rest.Get<GetIceResponse>(Endpoints.VoiceIce);*/

		private string Base64Picture(ImageType type, byte[] data)
		{
			if (type == ImageType.None)
				return "";
			else if (data != null)
			{
				string base64 = Convert.ToBase64String(data);
				string imageType = type == ImageType.Jpeg ? "image/jpeg;base64" : "image/png;base64";
				return $"data:{imageType},{base64}";
			}
			return null;
		}
	}
}
