using System;
using System.Reflection;

namespace Discord
{
	[Flags]
	public enum DiscordVoiceMode
	{
		Disabled = 0x00,
		Incoming = 0x01,
		Outgoing = 0x02,
		Both = Outgoing | Incoming
	}

	public sealed class DiscordClientConfig
	{
		/// <summary> Specifies the minimum log level severity that will be sent to the LogMessage event. Warning: setting this to debug will really hurt performance but should help investigate any internal issues. </summary>
		public LogMessageSeverity LogLevel { get { return _logLevel; } set { SetValue(ref _logLevel, value); } }
		private LogMessageSeverity _logLevel = LogMessageSeverity.Info;

		/// <summary> Max time in milliseconds to wait for DiscordClient to connect and initialize. </summary>
		public int ConnectionTimeout { get { return _connectionTimeout; } set { SetValue(ref _connectionTimeout, value); } }
		private int _connectionTimeout = 30000;
		/// <summary> Gets or sets the time (in milliseconds) to wait after an unexpected disconnect before reconnecting. </summary>
		public int ReconnectDelay { get { return _reconnectDelay; } set { SetValue(ref _reconnectDelay, value); } }
		private int _reconnectDelay = 1000;
		/// <summary> Gets or sets the time (in milliseconds) to wait after an reconnect fails before retrying. </summary>
		public int FailedReconnectDelay { get { return _failedReconnectDelay; } set { SetValue(ref _failedReconnectDelay, value); } }
		private int _failedReconnectDelay = 10000;
		/// <summary> Max time (in milliseconds) to wait for an API request to complete. </summary>
		public int APITimeout { get { return _apiTimeout; } set { SetValue(ref _apiTimeout, value); } }
		private int _apiTimeout = 5000;

		/// <summary> Gets or sets the time (in milliseconds) to wait when the websocket's message queue is empty before checking again. </summary>
		public int WebSocketInterval { get { return _webSocketInterval; } set { SetValue(ref _webSocketInterval, value); } }
		private int _webSocketInterval = 100;
		/// <summary> Gets or sets the time (in milliseconds) to wait when the message queue is empty before checking again. </summary>
		public int MessageQueueInterval { get { return _messageQueueInterval; } set { SetValue(ref _messageQueueInterval, value); } }
		private int _messageQueueInterval = 100;
		/// <summary> Gets or sets the max buffer length (in milliseconds) for outgoing voice packets. This value is the target maximum but is not guaranteed, the buffer will often go slightly above this value. </summary>
		public int VoiceBufferLength { get { return _voiceBufferLength; } set { SetValue(ref _voiceBufferLength, value); } }
		private int _voiceBufferLength = 1000;

		//Experimental Features
#if !DNXCORE50
		/// <summary> (Experimental) Enables the voice websocket and UDP client and specifies how it will be used. Any option other than Disabled requires the opus .dll or .so be in the local lib/ folder. </summary>
		public DiscordVoiceMode VoiceMode { get { return _voiceMode; } set { SetValue(ref _voiceMode, value); } }
		private DiscordVoiceMode _voiceMode = DiscordVoiceMode.Disabled;
		/// <summary> (Experimental) Enables the voice websocket and UDP client. This option requires the libsodium .dll or .so be in the local lib/ folder. </summary>
		public bool EnableVoiceEncryption { get { return _enableVoiceEncryption; } set { SetValue(ref _enableVoiceEncryption, value); } }
		private bool _enableVoiceEncryption = true;
#else
		internal DiscordVoiceMode VoiceMode => DiscordVoiceMode.Disabled;
		internal bool EnableVoiceEncryption => false;
		internal bool EnableVoiceMultiserver => false;
#endif
		/// <summary> (Experimental) Enables or disables the internal message queue. This will allow SendMessage to return immediately and handle messages internally. Messages will set the IsQueued and HasFailed properties to show their progress. </summary>
		public bool UseMessageQueue { get { return _useMessageQueue; } set { SetValue(ref _useMessageQueue, value); } }
		private bool _useMessageQueue = false;
		/// <summary> (Experimental) Maintains the LastActivity property for users, showing when they last made an action (sent message, joined server, typed, etc). </summary>
		public bool TrackActivity { get { return _trackActivity; } set { SetValue(ref _trackActivity, value); } }
		private bool _trackActivity = true;

		/// <summary> (Experimental) Enables the client to be simultaneously connected to multiple channels at once (Discord still limits you to one channel per server). </summary>
		public bool EnableVoiceMultiserver { get { return _enableVoiceMultiserver; } set { SetValue(ref _enableVoiceMultiserver, value); } }
		private bool _enableVoiceMultiserver = false;
		internal bool VoiceOnly { get { return _voiceOnly; } set { SetValue(ref _voiceOnly, value); } }
		private bool _voiceOnly;
		internal uint VoiceClientId { get { return _voiceClientId; } set { SetValue(ref _voiceClientId, value); } }
		private uint _voiceClientId;

		internal string UserAgent
		{
			get
			{
				string version = typeof(DiscordClientConfig).GetTypeInfo().Assembly.GetName().Version.ToString(2);
				return $"Discord.Net/{version} (https://github.com/RogueException/Discord.Net)";
			}
		}

		//Lock
		private bool _isLocked;
		internal void Lock() { _isLocked = true; }
		private void SetValue<T>(ref T storage, T value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to modify a discord client's configuration after it has been created.");
			storage = value;
		}

		public DiscordClientConfig Clone()
		{
			var config = this.MemberwiseClone() as DiscordClientConfig;
			config._isLocked = false;
			return config;
        }
	}
}
