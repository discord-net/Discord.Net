namespace Discord.API
{
	public static class Endpoints
	{
		public const string BaseStatusApi = "https://status.discordapp.com/api/v2/";
		public const string BaseApi = "https://discordapp.com/api/";
		public const string BaseCdn = "https://cdn.discordapp.com/";

		public const string Gateway = "gateway";

		public const string Auth = "auth";
		public const string AuthLogin = "auth/login";
		public const string AuthLogout = "auth/logout";
		
		public const string Channels = "channels";
		public static string Channel(ulong channelId) => $"channels/{channelId}";
		public static string ChannelInvites(ulong channelId) => $"channels/{channelId}/invites";
		public static string ChannelMessages(ulong channelId) => $"channels/{channelId}/messages";
		public static string ChannelMessages(ulong channelId, int limit) => $"channels/{channelId}/messages?limit={limit}";
		public static string ChannelMessages(ulong channelId, int limit, ulong relativeId, string relativeDir) => $"channels/{channelId}/messages?limit={limit}&{relativeDir}={relativeId}";
        public static string ChannelMessage(ulong channelId, ulong msgId) => $"channels/{channelId}/messages/{msgId}";
		public static string ChannelMessageAck(ulong channelId, ulong msgId) => $"channels/{channelId}/messages/{msgId}/ack";
        public static string ChannelPermission(ulong channelId, ulong userOrRoleId) => $"channels/{channelId}/permissions/{userOrRoleId}";
		public static string ChannelTyping(ulong channelId) => $"channels/{channelId}/typing";

		public const string Servers = "guilds";
		public static string Server(ulong serverId) => $"guilds/{serverId}";
		public static string ServerBan(ulong serverId, ulong userId) => $"guilds/{serverId}/bans/{userId}";
		public static string ServerChannels(ulong serverId) => $"guilds/{serverId}/channels";
		public static string ServerInvites(ulong serverId) => $"guilds/{serverId}/invites";
		public static string ServerMember(ulong serverId, ulong userId) => $"guilds/{serverId}/members/{userId}";
		public static string ServerPrune(ulong serverId, int days) => $"guilds/{serverId}/prune?days={days}";
		public static string ServerRoles(ulong serverId) => $"guilds/{serverId}/roles";
		public static string ServerRole(ulong serverId, ulong roleId) => $"guilds/{serverId}/roles/{roleId}";
		public static string ServerIcon(ulong serverId, string iconId) => BaseCdn + $"icons/{serverId}/{iconId}.jpg";

		public const string Invites = "invite";
		public static string Invite(ulong inviteId) => $"invite/{inviteId}";
		public static string Invite(string inviteIdOrXkcd) => $"invite/{inviteIdOrXkcd}";
		public static string InviteUrl(ulong inviteId) => $"https://discord.gg/{inviteId}";
		public static string InviteUrl(string inviteIdOrXkcd) => $"https://discord.gg/{inviteIdOrXkcd}";

		public const string Users = "users";
		public static string UserMe => $"users/@me";
        public static string UserChannels(ulong userId) => $"users/{userId}/channels";
		public static string UserAvatar(ulong serverId, string avatarId) => BaseCdn + $"avatars/{serverId}/{avatarId}.jpg";

		public const string Voice = "voice";
		public const string VoiceRegions = "voice/regions";

		public const string StatusActiveMaintenance = "scheduled-maintenances/active.json";
		public const string StatusUpcomingMaintenance = "scheduled-maintenances/upcoming.json";
	}
}
