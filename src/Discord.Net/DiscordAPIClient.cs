using Discord.API;
using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	public enum RelativeDirection { Before, After }

	/// <summary> A lightweight wrapper around the Discord API. </summary>
	public class DiscordAPIClient
	{
		public static readonly string Version = DiscordClient.Version;

		private readonly DiscordConfig _config;

		internal RestClient RestClient => _rest;
		private readonly RestClient _rest;

		public DiscordAPIClient(DiscordConfig config = null, Logger logger = null)
		{
			_config = config ?? new DiscordConfig();
            _rest = new RestClient(_config, logger);
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
        public Task<CreateChannelResponse> CreateChannel(ulong serverId, string name, string channelType)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (channelType == null) throw new ArgumentNullException(nameof(channelType));

			var request = new CreateChannelRequest { Name = name, Type = channelType };
			return _rest.Post<CreateChannelResponse>(Endpoints.ServerChannels(serverId), request);
		}
		public Task<CreateChannelResponse> CreatePMChannel(ulong myId, ulong recipientId)
		{
			var request = new CreatePMChannelRequest { RecipientId = recipientId };
			return _rest.Post<CreateChannelResponse>(Endpoints.UserChannels(myId), request);
		}
		public Task<DestroyChannelResponse> DestroyChannel(ulong channelId)
		{
			return _rest.Delete<DestroyChannelResponse>(Endpoints.Channel(channelId));
		}
		public Task<EditChannelResponse> EditChannel(ulong channelId, string name = null, string topic = null)
		{
			var request = new EditChannelRequest { Name = name, Topic = topic };
			return _rest.Patch<EditChannelResponse>(Endpoints.Channel(channelId), request);
		}
		public Task ReorderChannels(ulong serverId, IEnumerable<ulong> channelIds, int startPos = 0)
		{
			if (channelIds == null) throw new ArgumentNullException(nameof(channelIds));
			if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");

			uint pos = (uint)startPos;
			var channels = channelIds.Select(x => new ReorderChannelsRequest.Channel { Id = x, Position = pos++ });
			var request = new ReorderChannelsRequest(channels);
			return _rest.Patch(Endpoints.ServerChannels(serverId), request);
		}
		public Task<GetMessagesResponse> GetMessages(ulong channelId, int count, ulong? relativeMessageId = null, RelativeDirection relativeDir = RelativeDirection.Before)
		{
			if (relativeMessageId != null)
				return _rest.Get<GetMessagesResponse>(Endpoints.ChannelMessages(channelId, count, relativeMessageId.Value, relativeDir == RelativeDirection.Before ? "before" : "after"));
			else
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
		public Task<CreateInviteResponse> CreateInvite(ulong channelId, int maxAge, int maxUses, bool tempMembership, bool hasXkcd)
		{
			var request = new CreateInviteRequest { MaxAge = maxAge, MaxUses = maxUses, IsTemporary = tempMembership, WithXkcdPass = hasXkcd };
			return _rest.Post<CreateInviteResponse>(Endpoints.ChannelInvites(channelId), request);
		}
		public Task<GetInviteResponse> GetInvite(string inviteIdOrXkcd)
		{
			if (inviteIdOrXkcd == null) throw new ArgumentNullException(nameof(inviteIdOrXkcd));

			return _rest.Get<GetInviteResponse>(Endpoints.Invite(inviteIdOrXkcd));
		}
		public Task<GetInvitesResponse> GetInvites(ulong serverId)
		{
			return _rest.Get<GetInvitesResponse>(Endpoints.ServerInvites(serverId));
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
		public Task EditUser(ulong serverId, ulong userId, bool? mute = null, bool? deaf = null, ulong? voiceChannelId = null, IEnumerable<ulong> roleIds = null)
		{
			var request = new EditMemberRequest { Mute = mute, Deaf = deaf, ChannelId = voiceChannelId, Roles = roleIds };
			return _rest.Patch(Endpoints.ServerMember(serverId, userId), request);
		}
		public Task KickUser(ulong serverId, ulong userId)
		{
			return _rest.Delete(Endpoints.ServerMember(serverId, userId));
		}
		public Task BanUser(ulong serverId, ulong userId)
		{
			return _rest.Put(Endpoints.ServerBan(serverId, userId));
		}
		public Task UnbanUser(ulong serverId, ulong userId)
		{
			return _rest.Delete(Endpoints.ServerBan(serverId, userId));
		}
		public Task<PruneUsersResponse> PruneUsers(ulong serverId, int days, bool simulate)
		{			
            if (simulate)
				return _rest.Get<PruneUsersResponse>(Endpoints.ServerPrune(serverId, days));
			else
				return _rest.Post<PruneUsersResponse>(Endpoints.ServerPrune(serverId, days));
        }

		//Messages
		public Task<SendMessageResponse> SendMessage(ulong channelId, string message, IEnumerable<ulong> mentionedUserIds = null, string nonce = null, bool isTTS = false)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			var request = new SendMessageRequest { Content = message, Mentions = mentionedUserIds ?? new ulong[0], Nonce = nonce, IsTTS = isTTS ? true : false };
			return _rest.Post<SendMessageResponse>(Endpoints.ChannelMessages(channelId), request);
		}
		public Task<SendMessageResponse> SendFile(ulong channelId, string filename, Stream stream)
		{
			if (filename == null) throw new ArgumentNullException(nameof(filename));
			if (stream == null) throw new ArgumentNullException(nameof(stream));

			return _rest.PostFile<SendMessageResponse>(Endpoints.ChannelMessages(channelId), filename, stream);
		}
		public Task DeleteMessage(ulong messageId, ulong channelId)
		{
			return _rest.Delete(Endpoints.ChannelMessage(channelId, messageId));
		}
		public Task<EditMessageResponse> EditMessage(ulong messageId, ulong channelId, string message = null, IEnumerable<ulong> mentionedUserIds = null)
		{
			var request = new EditMessageRequest { Content = message, Mentions = mentionedUserIds };
			return _rest.Patch<EditMessageResponse>(Endpoints.ChannelMessage(channelId, messageId), request);
		}
		public Task AckMessage(ulong messageId, ulong channelId)
		{
			return _rest.Post(Endpoints.ChannelMessageAck(channelId, messageId));
		}
        public Task SendIsTyping(ulong channelId)
		{
			return _rest.Post(Endpoints.ChannelTyping(channelId));
		}

		//Permissions
		public Task SetChannelPermissions(ulong channelId, ulong userOrRoleId, string idType, uint allow = 0, uint deny = 0)
		{
			if (idType == null) throw new ArgumentNullException(nameof(idType));

			var request = new SetChannelPermissionsRequest { Id = userOrRoleId, Type = idType, Allow = allow, Deny = deny };
			return _rest.Put(Endpoints.ChannelPermission(channelId, userOrRoleId), request);
		}
		public Task DeleteChannelPermissions(ulong channelId, ulong userOrRoleId)
		{
			return _rest.Delete(Endpoints.ChannelPermission(channelId, userOrRoleId), null);
		}

		//Roles
		public Task<RoleInfo> CreateRole(ulong serverId)
		{			
			return _rest.Post<RoleInfo>(Endpoints.ServerRoles(serverId));
		}
		public Task DeleteRole(ulong serverId, ulong roleId)
		{
			return _rest.Delete(Endpoints.ServerRole(serverId, roleId));
		}
		public Task<RoleInfo> EditRole(ulong serverId, ulong roleId, string name = null, uint? permissions = null, uint? color = null, bool? hoist = null)
		{
			var request = new EditRoleRequest { Name = name, Permissions = permissions, Hoist = hoist, Color = color };
			return _rest.Patch<RoleInfo>(Endpoints.ServerRole(serverId, roleId), request);
		}
		public Task ReorderRoles(ulong serverId, IEnumerable<ulong> roleIds, int startPos = 0)
		{
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
		public Task LeaveServer(ulong serverId)
		{
			return _rest.Delete<DeleteServerResponse>(Endpoints.Server(serverId));
		}
		public Task<EditServerResponse> EditServer(ulong serverId, string name = null, string region = null, 
			Stream icon = null, ImageType iconType = ImageType.Png, string existingIcon = null)
		{
			var request = new EditServerRequest { Name = name, Region = region, Icon = Base64Picture(icon, iconType, existingIcon) };
			return _rest.Patch<EditServerResponse>(Endpoints.Server(serverId), request);
		}

		//User
		public Task<EditUserResponse> EditProfile(string currentPassword = "",
			string username = null, string email = null, string password = null,
			Stream avatar = null, ImageType avatarType = ImageType.Png, string existingAvatar = null)
		{
			if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));
			
			var request = new EditUserRequest { CurrentPassword = currentPassword, Username = username,
				Email = email, Password = password, Avatar = Base64Picture(avatar, avatarType, existingAvatar) };
			return _rest.Patch<EditUserResponse>(Endpoints.UserMe, request);
		}

		//Voice
		public Task<GetRegionsResponse> GetVoiceRegions()
			=> _rest.Get<GetRegionsResponse>(Endpoints.VoiceRegions);

		private string Base64Picture(Stream stream, ImageType type, string existingId)
		{
			if (type == ImageType.None)
				return null;
			else if (stream != null)
			{
				byte[] bytes = new byte[stream.Length - stream.Position];
				stream.Read(bytes, 0, bytes.Length);

				string base64 = Convert.ToBase64String(bytes);
				string imageType = type == ImageType.Jpeg ? "image/jpeg;base64" : "image/png;base64";
				return $"data:{imageType},{base64}";
			}
			return existingId;
		}
	}
}
