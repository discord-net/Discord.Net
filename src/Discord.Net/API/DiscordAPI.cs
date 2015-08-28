using Discord.API.Models;
using Discord.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Discord.API
{
	internal class DiscordAPI
	{
		public const int MaxMessageSize = 2000;
		private readonly JsonHttpClient _http;

		public DiscordAPI(JsonHttpClient http)
		{
			_http = http;
        }

		//Auth
		public Task<APIResponses.Gateway> GetWebSocket()
			=> _http.Get<APIResponses.Gateway>(Endpoints.Gateway);
        public async Task<APIResponses.AuthRegister> LoginAnonymous(string username)
		{
			var fingerprintResponse = await _http.Post<APIResponses.AuthFingerprint>(Endpoints.AuthFingerprint);
			var registerRequest = new APIRequests.AuthRegisterRequest { Fingerprint = fingerprintResponse.Fingerprint, Username = username };
			var registerResponse = await _http.Post<APIResponses.AuthRegister>(Endpoints.AuthRegister, registerRequest);
			return registerResponse;
        }
		public async Task<APIResponses.AuthLogin> Login(string email, string password)
		{
			var request = new APIRequests.AuthLogin { Email = email, Password = password };
			var response = await _http.Post<APIResponses.AuthLogin>(Endpoints.AuthLogin, request);
			return response;
		}
		public Task Logout()
			=> _http.Post(Endpoints.AuthLogout);

		//Servers
		public Task<APIResponses.CreateServer> CreateServer(string name, string region)
		{
			var request = new APIRequests.CreateServer { Name = name, Region = region };
			return _http.Post<APIResponses.CreateServer>(Endpoints.Servers, request);
        }
		public Task LeaveServer(string id)
			=> _http.Delete<APIResponses.DeleteServer>(Endpoints.Server(id));

		//Channels
        public Task<APIResponses.CreateChannel> CreateChannel(string serverId, string name, string channelType)
		{
			var request = new APIRequests.CreateChannel { Name = name, Type = channelType };
			return _http.Post<APIResponses.CreateChannel>(Endpoints.ServerChannels(serverId), request);
		}
		public Task<APIResponses.CreateChannel> CreatePMChannel(string myId, string recipientId)
		{
			var request = new APIRequests.CreatePMChannel { RecipientId = recipientId };
			return _http.Post<APIResponses.CreateChannel>(Endpoints.UserChannels(myId), request);
		}
		public Task<APIResponses.DestroyChannel> DestroyChannel(string channelId)
			=> _http.Delete<APIResponses.DestroyChannel>(Endpoints.Channel(channelId));
		public Task<APIResponses.GetMessages[]> GetMessages(string channelId, int count)
			=> _http.Get<APIResponses.GetMessages[]>(Endpoints.ChannelMessages(channelId, count));

		//Members
		public Task Kick(string serverId, string memberId)
			=> _http.Delete(Endpoints.ServerMember(serverId, memberId));
		public Task Ban(string serverId, string memberId)
			=> _http.Put(Endpoints.ServerBan(serverId, memberId));
		public Task Unban(string serverId, string memberId)
			=> _http.Delete(Endpoints.ServerBan(serverId, memberId));

		//Invites
		public Task<APIResponses.CreateInvite> CreateInvite(string channelId, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			var request = new APIRequests.CreateInvite { MaxAge = maxAge, MaxUses = maxUses, IsTemporary = isTemporary, HasXkcdPass = hasXkcdPass };
            return _http.Post<APIResponses.CreateInvite>(Endpoints.ChannelInvites(channelId), request);
		}
		public Task<APIResponses.GetInvite> GetInvite(string id)
			=> _http.Get<APIResponses.GetInvite>(Endpoints.Invite(id));
		public Task AcceptInvite(string id)
			=> _http.Post<APIResponses.AcceptInvite>(Endpoints.Invite(id));
		public Task DeleteInvite(string id)
			=> _http.Delete(Endpoints.Invite(id));
		
		//Chat
		public Task<APIResponses.SendMessage> SendMessage(string channelId, string message, string[] mentions, string nonce)
		{
			var request = new APIRequests.SendMessage { Content = message, Mentions = mentions, Nonce = nonce };
			return _http.Post<APIResponses.SendMessage>(Endpoints.ChannelMessages(channelId), request);
		}
		public Task<APIResponses.EditMessage> EditMessage(string channelId, string messageId, string message, string[] mentions)
		{
			var request = new APIRequests.EditMessage { Content = message, Mentions = mentions };
			return _http.Patch<APIResponses.EditMessage>(Endpoints.ChannelMessage(channelId, messageId), request);
		}
		public Task SendIsTyping(string channelId)
			=> _http.Post(Endpoints.ChannelTyping(channelId));
		public Task DeleteMessage(string channelId, string msgId)
			=> _http.Delete(Endpoints.ChannelMessage(channelId, msgId));
		public Task SendFile(string channelId, Stream stream, string filename = null)
			=> _http.File<APIResponses.SendMessage>(Endpoints.ChannelMessages(channelId), stream, filename);

		//Voice
		public Task<APIResponses.GetRegions[]> GetVoiceRegions()
			=> _http.Get<APIResponses.GetRegions[]>(Endpoints.VoiceRegions);
		public Task<APIResponses.GetIce> GetVoiceIce()
			=> _http.Get<APIResponses.GetIce>(Endpoints.VoiceIce);
		public Task Mute(string serverId, string memberId)
		{
			var request = new APIRequests.SetMemberMute { Mute = true };
			return _http.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public Task Unmute(string serverId, string memberId)
		{
			var request = new APIRequests.SetMemberMute { Mute = false };
			return _http.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public Task Deafen(string serverId, string memberId)
		{
			var request = new APIRequests.SetMemberDeaf { Deaf = true };
			return _http.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public Task Undeafen(string serverId, string memberId)
		{
			var request = new APIRequests.SetMemberDeaf { Deaf = false };
			return _http.Patch(Endpoints.ServerMember(serverId, memberId));
		}

		//Profile
		public Task<SelfUserInfo> ChangeUsername(string newUsername, string currentEmail, string currentPassword)
		{
			var request = new APIRequests.ChangeUsername { Username = newUsername, CurrentEmail = currentEmail, CurrentPassword = currentPassword };
			return _http.Patch<SelfUserInfo>(Endpoints.UserMe, request);
		}
		public Task<SelfUserInfo> ChangeEmail(string newEmail, string currentPassword)
		{
			var request = new APIRequests.ChangeEmail { NewEmail = newEmail, CurrentPassword = currentPassword };
			return _http.Patch<SelfUserInfo>(Endpoints.UserMe, request);
		}
		public Task<SelfUserInfo> ChangePassword(string newPassword, string currentEmail, string currentPassword)
		{
			var request = new APIRequests.ChangePassword { NewPassword = newPassword, CurrentEmail = currentEmail, CurrentPassword = currentPassword };
			return _http.Patch<SelfUserInfo>(Endpoints.UserMe, request);
		}
		public Task<SelfUserInfo> ChangeAvatar(AvatarImageType imageType, byte[] bytes, string currentEmail, string currentPassword)
		{
			string base64 = Convert.ToBase64String(bytes);
			string type = imageType == AvatarImageType.Jpeg ? "image/jpeg;base64" : "image/png;base64";
			var request = new APIRequests.ChangeAvatar { Avatar = $"data:{type},/9j/{base64}", CurrentEmail = currentEmail, CurrentPassword = currentPassword };
			return _http.Patch<SelfUserInfo>(Endpoints.UserMe, request);
		}
	}
}
