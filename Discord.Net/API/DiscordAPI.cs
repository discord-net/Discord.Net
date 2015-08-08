using Discord.API.Models;
using Discord.Helpers;
using System.Threading.Tasks;

namespace Discord.API
{
	internal static class DiscordAPI
	{
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
		{
			return Http.Post(Endpoints.AuthLogout, options);
		}

		//Servers
		public static Task<APIResponses.CreateServer> CreateServer(string name, string region, HttpOptions options)
		{
			var request = new APIRequests.CreateServer { Name = name, Region = region };
			return Http.Post<APIResponses.CreateServer>(Endpoints.Servers, request, options);
        }
		public static Task LeaveServer(string id, HttpOptions options)
		{
			return Http.Delete<APIResponses.DeleteServer>(Endpoints.Server(id), options);
		}

		//Channels
		public static Task<APIResponses.GetMessages[]> GetMessages(string channelId, HttpOptions options)
		{
			return Http.Get<APIResponses.GetMessages[]>(Endpoints.ChannelMessages(channelId, 50), options);
		}

		//Invites
		public static Task<APIResponses.CreateInvite> CreateInvite(string channelId, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass, HttpOptions options)
		{
			var request = new APIRequests.CreateInvite { MaxAge = maxAge, MaxUses = maxUses, IsTemporary = isTemporary, HasXkcdPass = hasXkcdPass };
            return Http.Post<APIResponses.CreateInvite>(Endpoints.ChannelInvites(channelId), request, options);
		}
		public static Task<APIResponses.GetInvite> GetInvite(string id, HttpOptions options)
		{
			return Http.Get<APIResponses.GetInvite>(Endpoints.Invite(id), options);
		}
		public static Task AcceptInvite(string id, HttpOptions options)
		{
			return Http.Post<APIResponses.AcceptInvite>(Endpoints.Invite(id), options);
		}
		public static Task DeleteInvite(string id, HttpOptions options)
		{
			return Http.Delete(Endpoints.Invite(id), options);
		}
		
		//Chat
		public static Task SendMessage(string channelId, string message, string[] mentions, HttpOptions options)
		{
			var request = new APIRequests.SendMessage { Content = message, Mentions = mentions };
			return Http.Post(Endpoints.ChannelMessages(channelId), request, options);
		}
		public static Task SendIsTyping(string channelId, HttpOptions options)
		{
			return Http.Post(Endpoints.ChannelTyping(channelId), options);
		}

		//Voice
		public static Task<APIResponses.GetRegions[]> GetVoiceRegions(HttpOptions options)
		{
			return Http.Get<APIResponses.GetRegions[]>(Endpoints.VoiceRegions, options);
		}
		public static Task<APIResponses.GetIce> GetVoiceIce(HttpOptions options)
		{
			return Http.Get<APIResponses.GetIce>(Endpoints.VoiceIce, options);
		}
    }
}
