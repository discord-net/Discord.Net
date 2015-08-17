using Discord.API.Models;
using Discord.Helpers;
using System.Threading.Tasks;

namespace Discord.API
{
	internal static class DiscordAPI
	{
		public const int MaxMessageSize = 2000;

		//Auth
		public static async Task<APIResponses.AuthRegister> LoginAnonymous(string username)
		{
			var fingerprintResponse = await Http.Post<APIResponses.AuthFingerprint>(Endpoints.AuthFingerprint);
			var registerRequest = new APIRequests.AuthRegisterRequest { Fingerprint = fingerprintResponse.Fingerprint, Username = username };
			var registerResponse = await Http.Post<APIResponses.AuthRegister>(Endpoints.AuthRegister, registerRequest);
			return registerResponse;
        }
		public static async Task<APIResponses.AuthLogin> Login(string email, string password)
		{
			var request = new APIRequests.AuthLogin { Email = email, Password = password };
			var response = await Http.Post<APIResponses.AuthLogin>(Endpoints.AuthLogin, request);
			return response;
		}
		public static Task Logout()
			=> Http.Post(Endpoints.AuthLogout);

		//Servers
		public static Task<APIResponses.CreateServer> CreateServer(string name, string region)
		{
			var request = new APIRequests.CreateServer { Name = name, Region = region };
			return Http.Post<APIResponses.CreateServer>(Endpoints.Servers, request);
        }
		public static Task LeaveServer(string id)
			=> Http.Delete<APIResponses.DeleteServer>(Endpoints.Server(id));

		//Channels
        public static Task<APIResponses.CreateChannel> CreateChannel(string serverId, string name, string channelType)
		{
			var request = new APIRequests.CreateChannel { Name = name, Type = channelType };
			return Http.Post<APIResponses.CreateChannel>(Endpoints.ServerChannels(serverId), request);
		}
		public static Task<APIResponses.CreateChannel> CreatePMChannel(string myId, string recipientId)
		{
			var request = new APIRequests.CreatePMChannel { RecipientId = recipientId };
			return Http.Post<APIResponses.CreateChannel>(Endpoints.UserChannels(myId), request);
		}
		public static Task<APIResponses.DestroyChannel> DestroyChannel(string channelId)
			=> Http.Delete<APIResponses.DestroyChannel>(Endpoints.Channel(channelId));
		public static Task<APIResponses.GetMessages[]> GetMessages(string channelId, int count)
			=> Http.Get<APIResponses.GetMessages[]>(Endpoints.ChannelMessages(channelId, count));

		//Members
		public static Task Kick(string serverId, string memberId)
			=> Http.Delete(Endpoints.ServerMember(serverId, memberId));
		public static Task Ban(string serverId, string memberId)
			=> Http.Put(Endpoints.ServerBan(serverId, memberId));
		public static Task Unban(string serverId, string memberId)
			=> Http.Delete(Endpoints.ServerBan(serverId, memberId));

		//Invites
		public static Task<APIResponses.CreateInvite> CreateInvite(string channelId, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			var request = new APIRequests.CreateInvite { MaxAge = maxAge, MaxUses = maxUses, IsTemporary = isTemporary, HasXkcdPass = hasXkcdPass };
            return Http.Post<APIResponses.CreateInvite>(Endpoints.ChannelInvites(channelId), request);
		}
		public static Task<APIResponses.GetInvite> GetInvite(string id)
			=> Http.Get<APIResponses.GetInvite>(Endpoints.Invite(id));
		public static Task AcceptInvite(string id)
			=> Http.Post<APIResponses.AcceptInvite>(Endpoints.Invite(id));
		public static Task DeleteInvite(string id)
			=> Http.Delete(Endpoints.Invite(id));
		
		//Chat
		public static Task<APIResponses.SendMessage> SendMessage(string channelId, string message, string[] mentions)
		{
			var request = new APIRequests.SendMessage { Content = message, Mentions = mentions };
			return Http.Post<APIResponses.SendMessage>(Endpoints.ChannelMessages(channelId), request);
		}
		public static Task<APIResponses.EditMessage> EditMessage(string channelId, string messageId, string message, string[] mentions)
		{
			var request = new APIRequests.EditMessage { Content = message, Mentions = mentions };
			return Http.Patch<APIResponses.EditMessage>(Endpoints.ChannelMessage(channelId, messageId), request);
		}
		public static Task SendIsTyping(string channelId)
			=> Http.Post(Endpoints.ChannelTyping(channelId));
		public static Task DeleteMessage(string channelId, string msgId)
			=> Http.Delete(Endpoints.ChannelMessage(channelId, msgId));

		//Voice
		public static Task<APIResponses.GetRegions[]> GetVoiceRegions()
			=> Http.Get<APIResponses.GetRegions[]>(Endpoints.VoiceRegions);
		public static Task<APIResponses.GetIce> GetVoiceIce()
			=> Http.Get<APIResponses.GetIce>(Endpoints.VoiceIce);
		public static Task Mute(string serverId, string memberId)
		{
			var request = new APIRequests.SetMemberMute { Mute = true };
			return Http.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public static Task Unmute(string serverId, string memberId)
		{
			var request = new APIRequests.SetMemberMute { Mute = false };
			return Http.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public static Task Deafen(string serverId, string memberId)
		{
			var request = new APIRequests.SetMemberDeaf { Deaf = true };
			return Http.Patch(Endpoints.ServerMember(serverId, memberId));
		}
		public static Task Undeafen(string serverId, string memberId)
		{
			var request = new APIRequests.SetMemberDeaf { Deaf = false };
			return Http.Patch(Endpoints.ServerMember(serverId, memberId));
		}
	}
}
