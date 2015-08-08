namespace Discord.API
{
	internal static class Endpoints
	{
		public static readonly string BaseUrl = "discordapp.com";
		public static readonly string BaseHttps = $"https://{BaseUrl}";

		// /api
		public static readonly string BaseApi = $"{BaseHttps}/api";
		public static readonly string Track = $"{BaseApi}/track";

		// /api/auth
		public static readonly string Auth = $"{BaseApi}/auth";
        public static readonly string AuthFingerprint = $"{Auth}fingerprint";
		public static readonly string AuthRegister = $"{Auth}/register";
		public static readonly string AuthLogin = $"{Auth}/login";
		public static readonly string AuthLogout = $"{Auth}/logout";

		// /api/guilds
		public static readonly string Servers = $"{BaseApi}/guilds";
		public static string Server(string id) => $"{Servers}/{id}";
		public static string ServerMember(string serverId, string userId) => $"{Servers}/{serverId}/members/{userId}";
		public static string ServerBan(string serverId, string userId) => $"{Servers}/{serverId}/bans/{userId}";

		// /api/invites
		public static readonly string Invites = $"{BaseApi}/invite";
		public static string Invite(string id) => $"{Invites}/{id}";

		// /api/channels
		public static readonly string Channels = $"{BaseApi}/channels";
		public static string Channel(string id) => $"{Channels}/{id}";
        public static string ChannelTyping(string id) => $"{Channels}/{id}/typing";
        public static string ChannelMessages(string id) => $"{Channels}/{id}/messages";
		public static string ChannelMessages(string id, int limit) => $"{Channels}/{id}/messages?limit={limit}";
        public static string ChannelInvites(string id) => $"{Channels}/{id}/invites";

		//  /api/voice
		public static readonly string Voice = $"{BaseApi}/voice";
		public static readonly string VoiceRegions = $"{Voice}/regions";
		public static readonly string VoiceIce = $"{Voice}/ice";

		//Web Sockets
		public static readonly string BaseWss = "wss://" + BaseUrl;
		public static readonly string WebSocket_Hub = $"{BaseWss}/hub";
    }
}
