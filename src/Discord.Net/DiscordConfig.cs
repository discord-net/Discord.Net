using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;

namespace Discord
{
	public enum LogSeverity : byte
	{
		Error = 1,
		Warning = 2,
		Info = 3,
		Verbose = 4,
		Debug = 5
	}

	public abstract class Config<T>
		where T : Config<T>
	{
		protected bool _isLocked;
		protected internal void Lock() { _isLocked = true; }
		protected void SetValue<U>(ref U storage, U value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to modify a discord client's configuration after it has been created.");
			storage = value;
		}

		public T Clone()
		{
			var config = MemberwiseClone() as T;
			config._isLocked = false;
			return config;
		}
	}
	
	public class DiscordConfig : Config<DiscordConfig>
    {
        public const int MaxMessageSize = 2000;

        public const string LibName = "Discord.Net";
        public static string LibVersion => typeof(DiscordConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3);
        public const string LibUrl = "https://github.com/RogueException/Discord.Net";

        public const string ClientAPIUrl = "https://discordapp.com/api/";
        public const string StatusAPIUrl = "https://status.discordapp.com/api/v2/";
        public const string CDNUrl = "https://cdn.discordapp.com/";
        public const string InviteUrl = "https://discord.gg/";

        //Global

        /// <summary> Name of your application. This is used both for the token cache directory and user agent. </summary>
        public string AppName { get { return _appName; } set { SetValue(ref _appName, value); UpdateUserAgent(); } }
        private string _appName = null;
        /// <summary> Version of your application. </summary>
        public string AppVersion { get { return _appVersion; } set { SetValue(ref _appVersion, value); UpdateUserAgent(); } }
        private string _appVersion = null;

        /// <summary> Specifies the minimum log level severity that will be sent to the LogMessage event. Warning: setting this to debug will really hurt performance but should help investigate any internal issues. </summary>
        public LogSeverity LogLevel { get { return _logLevel; } set { SetValue(ref _logLevel, value); } }
        private LogSeverity _logLevel = LogSeverity.Info;
        /// <summary> Enables or disables the default event logger. </summary>
        public bool LogEvents { get { return _logEvents; } set { SetValue(ref _logEvents, value); } }
        private bool _logEvents = true;

        /// <summary> User Agent string to use when connecting to Discord. </summary>
        [JsonIgnore]
        public string UserAgent { get { return _userAgent; } }
        private string _userAgent;

        //Rest

        /// <summary> Max time (in milliseconds) to wait for an API request to complete. </summary>
        public int RestTimeout { get { return _restTimeout; } set { SetValue(ref _restTimeout, value); } }
		private int _restTimeout = 10000;

		/// <summary> Enables or disables the internal message queue. This will allow SendMessage/EditMessage to return immediately and handle messages internally. </summary>
		public bool UseMessageQueue { get { return _useMessageQueue; } set { SetValue(ref _useMessageQueue, value); } }
		private bool _useMessageQueue = true;
		/// <summary> Gets or sets the time (in milliseconds) to wait when the message queue is empty before checking again. </summary>
		public int MessageQueueInterval { get { return _messageQueueInterval; } set { SetValue(ref _messageQueueInterval, value); } }
		private int _messageQueueInterval = 100;

        //WebSocket

        /// <summary> Gets or sets the time (in milliseconds) to wait for the websocket to connect and initialize. </summary>
        public int ConnectionTimeout { get { return _connectionTimeout; } set { SetValue(ref _connectionTimeout, value); } }
		private int _connectionTimeout = 30000;
		/// <summary> Gets or sets the time (in milliseconds) to wait after an unexpected disconnect before reconnecting. </summary>
		public int ReconnectDelay { get { return _reconnectDelay; } set { SetValue(ref _reconnectDelay, value); } }
		private int _reconnectDelay = 1000;
		/// <summary> Gets or sets the time (in milliseconds) to wait after an reconnect fails before retrying. </summary>
		public int FailedReconnectDelay { get { return _failedReconnectDelay; } set { SetValue(ref _failedReconnectDelay, value); } }
		private int _failedReconnectDelay = 10000;

		/// <summary> Gets or sets the time (in milliseconds) to wait when the websocket's message queue is empty before checking again. </summary>
		public int WebSocketInterval { get { return _webSocketInterval; } set { SetValue(ref _webSocketInterval, value); } }
		private int _webSocketInterval = 100;

        //Performance

        /// <summary> Cache an encrypted login token to temp dir after success login. </summary>
        public bool CacheToken { get { return _cacheToken; } set { SetValue(ref _cacheToken, value); } }
        private bool _cacheToken = true;
        /// <summary> Instructs Discord to not send send information about offline users, for servers with more than 50 users. </summary>
        public bool UseLargeThreshold { get { return _useLargeThreshold; } set { SetValue(ref _useLargeThreshold, value); } }
        private bool _useLargeThreshold = false;
        /// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
        public int MessageCacheSize { get { return _messageCacheSize; } set { SetValue(ref _messageCacheSize, value); } }
		private int _messageCacheSize = 100;
        /// <summary> Gets or sets whether the permissions cache should be used. This makes operations such as User.GetPermissions(Channel), User.ServerPermissions and Channel.Members </summary>
        public bool UsePermissionsCache { get { return _usePermissionsCache; } set { SetValue(ref _usePermissionsCache, value); } }
        private bool _usePermissionsCache = true;

        public DiscordConfig()
        {
            UpdateUserAgent();
        }
        
        private void UpdateUserAgent()
        {
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(_appName))
            {
                builder.Append(_appName);
                if (!string.IsNullOrEmpty(_appVersion))
                {
                    builder.Append('/');
                    builder.Append(_appVersion);
                }
                builder.Append(' ');
            }
            builder.Append($"DiscordBot ({LibUrl}, v{LibVersion})");
            _userAgent = builder.ToString();
        }
    }
}
