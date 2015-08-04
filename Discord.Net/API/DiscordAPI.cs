using Discord.API.Models;
using Discord.Helpers;
using System.Threading.Tasks;

namespace Discord.API
{
	internal static class DiscordAPI
	{
		public static async Task<AuthRegisterResponse> LoginAnonymous(string username, HttpOptions options)
		{
			var fingerprintResponse = await Http.Post<AuthFingerprintResponse>(Endpoints.AuthFingerprint, options);
			var registerRequest = new AuthRegisterRequest { Fingerprint = fingerprintResponse.Fingerprint, Username = username };
			var registerResponse = await Http.Post<AuthRegisterResponse>(Endpoints.AuthRegister, registerRequest, options);
			return registerResponse;
        }
		public static async Task<AuthLoginResponse> Login(string email, string password, HttpOptions options)
		{
			var request = new AuthLoginRequest { Email = email, Password = password };
			var response = await Http.Post<AuthLoginResponse>(Endpoints.AuthLogin, request, options);
			options.Token = response.Token;
			return response;
		}
		public static Task Logout(HttpOptions options)
		{
			return Http.Post(Endpoints.AuthLogout, options);
		}

		public static Task CreateServer(string name, string region, HttpOptions options)
		{
			var request = new CreateServerRequest { Name = name, Region = region };
			return Http.Post(Endpoints.Servers, request, options);
		}
		public static Task DeleteServer(string id, HttpOptions options)
		{
			return Http.Delete(Endpoints.Server(id), options);
        }

		public static Task<GetInviteResponse> GetInvite(string id, HttpOptions options)
		{
			return Http.Get<GetInviteResponse>(Endpoints.Invite(id), options);
		}
		public static Task AcceptInvite(string id, HttpOptions options)
		{
			return Http.Post(Endpoints.Invite(id), options);
		}
		public static Task DeleteInvite(string id, HttpOptions options)
		{
			return Http.Delete(Endpoints.Invite(id), options);
		}

		public static Task Typing(string channelId, HttpOptions options)
		{
			return Http.Post(Endpoints.ChannelTyping(channelId), options);
		}
		public static Task SendMessage(string channelId, string message, string[] mentions, HttpOptions options)
		{
			var request = new SendMessageRequest { Content = message, Mentions = mentions };
			return Http.Post(Endpoints.ChannelMessages(channelId), request, options);
		}
	}
}
