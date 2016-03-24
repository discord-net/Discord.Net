using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Discord
{	
	public class DiscordConfigBuilder
    {
        //Global

        /// <summary> Gets or sets name of your application, used both for the token cache directory and user agent. </summary>
        public string AppName { get; set; } = null;
        /// <summary> Gets or sets url to your application, used in the user agent. </summary>
        public string AppUrl { get; set; } = null;
        /// <summary> Gets or sets the version of your application, used in the user agent. </summary>
        public string AppVersion { get; set; } = null;

        /// <summary> Gets or sets the minimum log level severity that will be sent to the LogMessage event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        //WebSocket

        /// <summary> Gets or sets the time (in milliseconds) to wait for the websocket to connect and initialize. </summary>
        public int ConnectionTimeout { get; set; } = 30000;
		/// <summary> Gets or sets the time (in milliseconds) to wait after an unexpected disconnect before reconnecting. </summary>
		public int ReconnectDelay { get; set; } = 1000;
		/// <summary> Gets or sets the time (in milliseconds) to wait after an reconnect fails before retrying. </summary>
		public int FailedReconnectDelay { get; set; } = 15000;

        //Performance

        /// <summary> Gets or sets whether an encrypted login token should be saved to temp dir after successful login. </summary>
        public bool CacheToken { get; set; } = true;
        /// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
        public int MessageCacheSize { get; set; } = 100;
        /// <summary> 
        /// Gets or sets whether the permissions cache should be used. 
        /// This makes operations such as User.GetPermissions(Channel), User.ServerPermissions, Channel.GetUser, and Channel.Members much faster while increasing memory usage.
        /// </summary>
        public bool UsePermissionsCache { get; set; } = true;
        /// <summary> Gets or sets whether the a copy of a model is generated on an update event to allow you to check which properties changed. </summary>
        public bool EnablePreUpdateEvents { get; set; } = true;
        /// <summary> 
        /// Gets or sets the max number of users a server may have for offline users to be included in the READY packet. Max is 250. 
        /// Decreasing this may reduce CPU usage while increasing login time and network usage. 
        /// </summary>
        public int LargeThreshold { get; set; } = 250;

        //Events

        /// <summary> Gets or sets a handler for all log messages. </summary>
        public EventHandler<LogMessageEventArgs> LogHandler { get; set; }

        public DiscordConfig Build() => new DiscordConfig(this);
    }

    public class DiscordConfig
    {
        public const int MaxMessageSize = 2000;
        internal const int RestTimeout = 10000;
        internal const int MessageQueueInterval = 100;
        internal const int WebSocketQueueInterval = 100;
        internal const int ServerBatchCount = 50;

        public const string LibName = "Discord.Net";
        public static string LibVersion => typeof(DiscordConfigBuilder).GetTypeInfo().Assembly.GetName().Version.ToString(3);
        public const string LibUrl = "https://github.com/RogueException/Discord.Net";

        public const string ClientAPIUrl = "https://discordapp.com/api/";
        public const string StatusAPIUrl = "https://srhpyqt94yxb.statuspage.io/api/v2/"; //"https://status.discordapp.com/api/v2/";
        public const string CDNUrl = "https://cdn.discordapp.com/";
        public const string InviteUrl = "https://discord.gg/";

        public LogSeverity LogLevel { get; }
        
        public string AppName { get; }
        public string AppUrl { get; }
        public string AppVersion { get; }
        public string UserAgent { get; }
        public string CacheDir { get; }

        public int ConnectionTimeout { get; }
        public int ReconnectDelay { get; }
        public int FailedReconnectDelay { get; }

        public int LargeThreshold { get; }
        public int MessageCacheSize { get; }
        public bool UsePermissionsCache { get; }
        public bool EnablePreUpdateEvents { get; }

        internal DiscordConfig(DiscordConfigBuilder builder)
        {
            LogLevel = builder.LogLevel;

            AppName = builder.AppName;
            AppUrl = builder.AppUrl;
            AppVersion = builder.AppVersion;
            UserAgent = GetUserAgent(builder);
            CacheDir = GetCacheDir(builder);

            ConnectionTimeout = builder.ConnectionTimeout;
            ReconnectDelay = builder.ReconnectDelay;
            FailedReconnectDelay = builder.FailedReconnectDelay;
            
            MessageCacheSize = builder.MessageCacheSize;
            UsePermissionsCache = builder.UsePermissionsCache;
            EnablePreUpdateEvents = builder.EnablePreUpdateEvents;
        }

        private static string GetUserAgent(DiscordConfigBuilder builder)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(builder.AppName))
            {
                sb.Append(builder.AppName);
                if (!string.IsNullOrEmpty(builder.AppVersion))
                {
                    sb.Append('/');
                    sb.Append(builder.AppVersion);
                }
                if (!string.IsNullOrEmpty(builder.AppUrl))
                {
                    sb.Append(" (");
                    sb.Append(builder.AppUrl);
                    sb.Append(')');
                }
                sb.Append(' ');
            }
            sb.Append($"DiscordBot ({LibUrl}, v{LibVersion})");
            return sb.ToString();
        }
        private static string GetCacheDir(DiscordConfigBuilder builder)
        {
            if (builder.CacheToken)
                return Path.Combine(Path.GetTempPath(), builder.AppName ?? "Discord.Net");
            else
                return null;
        }
    }
}
