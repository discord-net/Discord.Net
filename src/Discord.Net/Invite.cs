using Newtonsoft.Json;

namespace Discord
{
	public sealed class Invite
	{
		private readonly DiscordClient _client;

		public int MaxAge, Uses, MaxUses;
		public bool IsRevoked, IsTemporary;
		public readonly string Code, XkcdPass;

		public string Url => API.Endpoints.InviteUrl(XkcdPass ?? Code);

		public string InviterId { get; internal set; }
		[JsonIgnore]
		public User Inviter => _client.GetUser(InviterId);

		public string ServerId { get; internal set; }
		[JsonIgnore]
		public Server Server => _client.GetServer(ServerId);

		public string ChannelId { get; internal set; }
		[JsonIgnore]
		public Channel Channel => _client.GetChannel(ChannelId);

		internal Invite(string code, string xkcdPass, DiscordClient client)
		{
			Code = code;
			XkcdPass = xkcdPass;
			_client = client;
		}		
	}
}
