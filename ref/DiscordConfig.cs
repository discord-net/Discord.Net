using System;

namespace Discord
{	
	public class DiscordConfig
    {
        public const int MaxMessageSize = 2000;
        public const string LibName = "Discord.Net";
        public static readonly string LibVersion = null;
        public const string LibUrl = "https://github.com/RogueException/Discord.Net";

        public const string ClientAPIUrl = "https://discordapp.com/api/";
        public const string StatusAPIUrl = "https://srhpyqt94yxb.statuspage.io/api/v2/"; //"https://status.discordapp.com/api/v2/";
        public const string CDNUrl = "https://cdn.discordapp.com/";
        public const string InviteUrl = "https://discord.gg/";

        public string AppName { get; set; } = null;
        public string AppUrl { get; set; } = null;
        public string AppVersion { get; set; } = null;
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
        
        public int ConnectionTimeout { get; set; } = 30000;
		public int ReconnectDelay { get; set; } = 1000;
		public int FailedReconnectDelay { get; set; } = 15000;
        
        public bool CacheToken { get; set; } = true;
        public int MessageCacheSize { get; set; } = 100;

        public bool UsePermissionsCache { get; set; } = true;
        public bool EnablePreUpdateEvents { get; set; } = true;
        public int LargeThreshold { get; set; } = 250;
        
        public EventHandler<LogMessageEventArgs> LogHandler { get; set; }
        public string UserAgent { get; }
    }
}
