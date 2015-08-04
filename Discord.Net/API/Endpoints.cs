namespace Discord.API
{
	internal static class Endpoints
	{
		public static readonly string BaseUrl = "discordapp.com/";
		public static readonly string BaseHttps = "https://" + BaseUrl;
		public static readonly string BaseWss = "wss://" + BaseUrl;

		public static readonly string Auth = $"{BaseHttps}/api/auth";
        public static readonly string AuthFingerprint = $"{Auth}fingerprint";
		public static readonly string AuthRegister = $"{Auth}/register";
		public static readonly string AuthLogin = $"{Auth}/login";
		public static readonly string AuthLogout = $"{Auth}/logout";

		public static readonly string Servers = $"{BaseHttps}/api/guilds";
		public static string Server(string id) { return $"{Servers}/{id}"; }
		public static string ServerMessages(string id) { return $"{Servers}/{id}/messages?limit=50"; }

		public static readonly string Invites = $"{BaseHttps}/api/invite";
		public static string Invite(string id) { return $"{Invites}/{id}"; }

		public static readonly string Channels = $"{BaseHttps}/api/channels";
		public static string Channel(string id) { return $"{Channels}/{id}"; }
        public static string ChannelTyping(string id) { return $"{Channels}/{id}/typing"; }
        public static string ChannelMessages(string id) { return $"{Channels}/{id}/messages"; }

		public static readonly string WebSocket_Hub = BaseWss + "hub";
		
    }
}
