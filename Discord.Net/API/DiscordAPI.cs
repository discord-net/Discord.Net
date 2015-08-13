using Discord.API.Models;
using Discord.Helpers;
using System.Threading.Tasks;

namespace Discord.API
{
	internal static class DiscordAPI
	{
		public const int MaxMessageSize = 2000;

		//Auth
		public static async Task<APIResponses.AuthRegister> LoginAnonymous(string username, HttpOptions options)
		{
			var fingerprintResponse = await Http.Post<APIResponses.AuthFingerprint>(Endpoints.AuthFingerprint, options);
			var registerRequest = new APIRequests.AuthRegisterRequest { Fingerprint = fingerprintResponse.Fingerprint, Username = username };
			var registerResponse = await Http.Post<APIResponses.AuthRegister>(Endpoints.AuthRegister, registerRequest, options);
			return registerResponse;
        }
		public static async Task<APIResponses.AuthLogin> Login(string email, string password, HttpOptions options)
		{
			var request = new APIRequests.AuthLogin { Email = email, Password = password };
			var response = await Http.Post<APIResponses.AuthLogin>(Endpoints.AuthLogin, request, options);
			options.Token = response.Token;
			return response;
		}
		public static Task Logout(HttpOptions options)
			=> Http.Post(Endpoints.AuthLogout, options);

		//Servers
		public static Task<APIResponses.CreateServer> CreateServer(string name, string region, HttpOptions options)
		{
			var request = new APIRequests.CreateServer { Name = name, Region = region };
			return Http.Post<APIResponses.CreateServer>(Endpoints.Servers, request, options);
        }
		public static Task LeaveServer(string id, HttpOptions options)
			=> Http.Delete<APIResponses.DeleteServer>(Endpoints.Server(id), options);

		//Channels
        public static Task<APIResponses.CreateChannel> CreateChannel(string serverId, string name, string channelType, HttpOptions options)
		{
			var request = new APIRequests.CreateChannel { Name = name, Type = channelType };
			return Http.Post<APIResponses.CreateChannel>(Endpoints.ServerChannels(serverId), request, options);
		}
		public static Task<APIResponses.CreateChannel> CreatePMChannel(string myId, string recipientId, HttpOptions options)
		{
			var request = new APIRequests.CreatePMChannel { RecipientId = recipientId };
			return Http.Post<APIResponses.CreateChannel>(Endpoints.UserChannels(myId), request, options);
		}
		public static Task<APIResponses.DestroyChannel> DestroyChannel(string channelId, HttpOptions options)
			=> Http.Delete<APIResponses.DestroyChannel>(Endpoints.Channel(channelId), options);
		public static Task<APIResponses.GetMessages[]> GetMessages(string channelId, HttpOptions options)
			=> Http.Get<APIResponses.GetMessages[]>(Endpoints.ChannelMessages(channelId, 50), options);

		//Members
		public static Task Kick(string serverId, string memberId, HttpOptions options)
			=> Http.Delete(Endpoints.ServerMember(serverId, memberId), options);
		public static Task Ban(string serverId, string memberId, HttpOptions options)
			=> Http.Put(Endpoints.ServerBan(serverId, memberId), options);
		public static Task Unban(string serverId, string memberId, HttpOptions options)
			=> Http.Delete(Endpoints.ServerBan(serverId, memberId), options);

		//Invites
		public static Task<APIResponses.CreateInvite> CreateInvite(string channelId, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass, HttpOptions options)
		{
			var request = new APIRequests.CreateInvite { MaxAge = maxAge, MaxUses = maxUses, IsTemporary = isTemporary, HasXkcdPass = hasXkcdPass };
            return Http.Post<APIResponses.CreateInvite>(Endpoints.ChannelInvites(channelId), request, options);
		}
		public static Task<APIResponses.GetInvite> GetInvite(string id, HttpOptions options)
			=> Http.Get<APIResponses.GetInvite>(Endpoints.Invite(id), options);
		public static Task AcceptInvite(string id, HttpOptions options)
			=> Http.Post<APIResponses.AcceptInvite>(Endpoints.Invite(id), options);
		public static Task DeleteInvite(string id, HttpOptions options)
			=> Http.Delete(Endpoints.Invite(id), options);
		
		//Chat
		public static Task<APIResponses.SendMessage> SendMessage(string channelId, string message, string[] mentions, HttpOptions options)
		{
			var request = new APIRequests.SendMessage { Content = message, Mentions = mentions };
			return Http.Post<APIResponses.SendMessage>(Endpoints.ChannelMessages(channelId), request, options);
		}
		public static Task SendIsTyping(string channelId, HttpOptions options)
			=> Http.Post(Endpoints.ChannelTyping(channelId), options);

		//Voice
		public static Task<APIResponses.GetRegions[]> GetVoiceRegions(HttpOptions options)
			=> Http.Get<APIResponses.GetRegions[]>(Endpoints.VoiceRegions, options);
		public static Task<APIResponses.GetIce> GetVoiceIce(HttpOptions options)
			=> Http.Get<APIResponses.GetIce>(Endpoints.VoiceIce, options);
		public static Task Mute(string serverId, string memberId, HttpOptions options)
		{
			var request = new APIRequests.SetMemberMute { Mute = true };
			return Http.Patch(Endpoints.ServerMember(serverId, memberId), options);
		}
		public static Task Unmute(string serverId, string memberId, HttpOptions options)
		{
			var request = new APIRequests.SetMemberMute { Mute = false };
			return Http.Patch(Endpoints.ServerMember(serverId, memberId), options);
		}
		public static Task Deafen(string serverId, string memberId, HttpOptions options)
		{
			var request = new APIRequests.SetMemberDeaf { Deaf = true };
			return Http.Patch(Endpoints.ServerMember(serverId, memberId), options);
		}
		public static Task Undeafen(string serverId, string memberId, HttpOptions options)
		{
			var request = new APIRequests.SetMemberDeaf { Deaf = false };
			return Http.Patch(Endpoints.ServerMember(serverId, memberId), options);
		}
	}
}
