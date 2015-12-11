using Newtonsoft.Json;
using System;
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

	public abstract class BaseConfig<T>
		where T : BaseConfig<T>
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
	
	public class DiscordConfig : BaseConfig<DiscordConfig>
	{
		//Global

		/// <summary> Specifies the minimum log level severity that will be sent to the LogMessage event. Warning: setting this to debug will really hurt performance but should help investigate any internal issues. </summary>
		public LogSeverity LogLevel { get { return _logLevel; } set { SetValue(ref _logLevel, value); } }
		private LogSeverity _logLevel = LogSeverity.Info;

		/// <summary> Name of your application. </summary>
		public string AppName { get { return _appName; } set { SetValue(ref _appName, value); UpdateUserAgent(); } }
        private string _appName = null;
        /// <summary> Version of your application. </summary>
        public string AppVersion { get { return _appVersion; } set { SetValue(ref _appVersion, value); UpdateUserAgent(); } }
        private string _appVersion = null;

        /// <summary> User Agent string to use when connecting to Discord. </summary>
        [JsonIgnore]
        public string UserAgent { get { return _userAgent; } }
        private string _userAgent;
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
            builder.Append($"DiscordBot (https://github.com/RogueException/Discord.Net, v{DiscordClient.Version})");
            _userAgent = builder.ToString();
        }

        //Rest

        /// <summary> Max time (in milliseconds) to wait for an API request to complete. </summary>
        public int RestTimeout { get { return _restTimeout; } set { SetValue(ref _restTimeout, value); } }
		private int _restTimeout = 10000;

		/// <summary> Enables or disables the internal message queue. This will allow SendMessage to return immediately and handle messages internally. Messages will set the IsQueued and HasFailed properties to show their progress. </summary>
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

		/// <summary> Instructs Discord to not send send information about offline users, for servers with more than 50 users. </summary>
		public bool UseLargeThreshold { get { return _useLargeThreshold; } set { SetValue(ref _useLargeThreshold, value); } }
		private bool _useLargeThreshold = false;
		/// <summary> Acknowledges all incoming messages so that they appear read. </summary>
		public bool AckMessages { get { return _ackMessages; } set { SetValue(ref _ackMessages, value); } }
		private bool _ackMessages = false;

		//Cache

		/// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
		public int MessageCacheSize { get { return _messageCacheSize; } set { SetValue(ref _messageCacheSize, value); } }
		private int _messageCacheSize = 100;
        /// <summary> Gets or sets whether the permissions cache should be used. This makes operations such as User.GetPermissions(Channel), User.ServerPermissions and Channel.Members </summary>
        public bool UsePermissionsCache { get { return _usePermissionsCache; } set { SetValue(ref _usePermissionsCache, value); } }
        private bool _usePermissionsCache = true;
        /// <summary> Maintains the LastActivity property for users, showing when they last made an action (sent message, joined server, typed, etc). </summary>
        public bool TrackActivity { get { return _trackActivity; } set { SetValue(ref _trackActivity, value); } }
		private bool _trackActivity = true;

        public DiscordConfig()
        {
            UpdateUserAgent();
        }
	}
}
