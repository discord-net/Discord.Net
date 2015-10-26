namespace Discord.API
{
	public static class Endpoints
	{
		public const string BaseStatusApi = "https://status.discordapp.com/api/v2/";
		public const string BaseApi = "https://discordapp.com/api/";

		public const string Gateway = "gateway";

		public const string Auth = "auth";
		public const string AuthLogin = "auth/login";
		public const string AuthLogout = "auth/logout";
		
		public const string Channels = "channels";
		public static string Channel(string channelId) => $"channels/{channelId}";
		public static string ChannelTyping(string channelId) => $"channels/{channelId}/typing";
		public static string ChannelMessages(string channelId) => $"channels/{channelId}/messages";
		public static string ChannelMessages(string channelId, int limit) => $"channels/{channelId}/messages?limit={limit}";
		public static string ChannelMessages(string channelId, int limit, string beforeId) => $"channels/{channelId}/messages?limit={limit}&before={beforeId}";
		public static string ChannelMessage(string channelId, string msgId) => $"channels/{channelId}/messages/{msgId}";
		public static string ChannelMessageAck(string channelId, string msgId) => $"channels/{channelId}/messages/{msgId}/ack";
		public static string ChannelInvites(string channelId) => $"channels/{channelId}/invites";
        public static string ChannelPermission(string channelId, string userOrRoleId) => $"channels/{channelId}/permissions/{userOrRoleId}";

		public const string Servers = "guilds";
		public static string Server(string serverId) => $"guilds/{serverId}";
		public static string ServerChannels(string serverId) => $"guilds/{serverId}/channels";
		public static string ServerMember(string serverId, string userId) => $"guilds/{serverId}/members/{userId}";
		public static string ServerBan(string serverId, string userId) => $"guilds/{serverId}/bans/{userId}";
		public static string ServerRoles(string serverId) => $"guilds/{serverId}/roles";
		public static string ServerRole(string serverId, string roleId) => $"guilds/{serverId}/roles/{roleId}";
		public static string ServerPrune(string serverId, int days) => $"guilds/{serverId}/prune?days={days}";

		public const string Invites = "invite";
		public static string Invite(string inviteId) => $"invite/{inviteId}";
		public static string InviteUrl(string inviteId) => $"https://discord.gg/{inviteId}";

		public const string Users = "users";
		public static string UserMe => $"users/@me";
        public static string UserChannels(string userId) => $"users/{userId}/channels";
		public static string UserAvatar(string userId, string avatarId) => $"users/{userId}/avatars/{avatarId}.jpg";
		
		public const string Voice = "voice";
		public const string VoiceRegions = "voice/regions";
		//public const string VoiceIce = "voice/ice";

		public const string StatusActiveMaintenance = "scheduled-maintenances/active.json";
		public const string StatusUpcomingMaintenance = "scheduled-maintenances/upcoming.json";
	}
}
