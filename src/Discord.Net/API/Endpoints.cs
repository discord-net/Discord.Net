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
		public static string Channel(long channelId) => $"channels/{channelId}";
		public static string ChannelInvites(long channelId) => $"channels/{channelId}/invites";
		public static string ChannelMessages(long channelId) => $"channels/{channelId}/messages";
		public static string ChannelMessages(long channelId, int limit) => $"channels/{channelId}/messages?limit={limit}";
		public static string ChannelMessages(long channelId, int limit, long relativeId, string relativeDir) => $"channels/{channelId}/messages?limit={limit}&{relativeDir}={relativeId}";
        public static string ChannelMessage(long channelId, long msgId) => $"channels/{channelId}/messages/{msgId}";
		public static string ChannelMessageAck(long channelId, long msgId) => $"channels/{channelId}/messages/{msgId}/ack";
        public static string ChannelPermission(long channelId, long userOrRoleId) => $"channels/{channelId}/permissions/{userOrRoleId}";
		public static string ChannelTyping(long channelId) => $"channels/{channelId}/typing";

		public const string Servers = "guilds";
		public static string Server(long serverId) => $"guilds/{serverId}";
		public static string ServerBan(long serverId, long userId) => $"guilds/{serverId}/bans/{userId}";
		public static string ServerChannels(long serverId) => $"guilds/{serverId}/channels";
		public static string ServerInvites(long serverId) => $"guilds/{serverId}/invites";
		public static string ServerMember(long serverId, long userId) => $"guilds/{serverId}/members/{userId}";
		public static string ServerPrune(long serverId, int days) => $"guilds/{serverId}/prune?days={days}";
		public static string ServerRoles(long serverId) => $"guilds/{serverId}/roles";
		public static string ServerRole(long serverId, long roleId) => $"guilds/{serverId}/roles/{roleId}";

		public const string Invites = "invite";
		public static string Invite(long inviteId) => $"invite/{inviteId}";
		public static string Invite(string inviteIdOrXkcd) => $"invite/{inviteIdOrXkcd}";
		public static string InviteUrl(long inviteId) => $"https://discord.gg/{inviteId}";
		public static string InviteUrl(string inviteIdOrXkcd) => $"https://discord.gg/{inviteIdOrXkcd}";

		public const string Users = "users";
		public static string UserAvatar(long userId, string avatarId) => BaseApi + $"users/{userId}/avatars/{avatarId}.jpg";
        public static string UserChannels(long userId) => $"users/{userId}/channels";
		public static string UserMe => $"users/@me";

		public const string Voice = "voice";
		public const string VoiceRegions = "voice/regions";

		public const string StatusActiveMaintenance = "scheduled-maintenances/active.json";
		public const string StatusUpcomingMaintenance = "scheduled-maintenances/upcoming.json";
	}
}
