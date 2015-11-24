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

	public class DiscordWSClientConfig : DiscordAPIClientConfig
	{
		/// <summary> Max time in milliseconds to wait for DiscordClient to connect and initialize. </summary>
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
		/// <summary> Gets or sets the max buffer length (in milliseconds) for outgoing voice packets. This value is the target maximum but is not guaranteed, the buffer will often go slightly above this value. </summary>
		public int VoiceBufferLength { get { return _voiceBufferLength; } set { SetValue(ref _voiceBufferLength, value); } }
		private int _voiceBufferLength = 1000;
		/// <summary> Gets or sets the bitrate used (in kbit/s, between 1 and 512) for outgoing voice packets. A null value will use default opus settings. </summary>
		public int? VoiceBitrate { get { return _voiceBitrate; } set { SetValue(ref _voiceBitrate, value); } }
		private int? _voiceBitrate = null;

		//Experimental Features
		/// <summary> (Experimental) Enables the voice websocket and UDP client and specifies how it will be used. Any option other than Disabled requires the opus .dll or .so be in the local lib/ folder. </summary>
		public DiscordVoiceMode VoiceMode { get { return _voiceMode; } set { SetValue(ref _voiceMode, value); } }
		private DiscordVoiceMode _voiceMode = DiscordVoiceMode.Disabled;
		/// <summary> (Experimental) Enables the voice websocket and UDP client. This option requires the libsodium .dll or .so be in the local lib/ folder. </summary>
		public bool EnableVoiceEncryption { get { return _enableVoiceEncryption; } set { SetValue(ref _enableVoiceEncryption, value); } }
		private bool _enableVoiceEncryption = true;
		/// <summary> (Experimental) Instructs Discord to not send send information about offline users, for servers with more than 50 users. </summary>
		public bool UseLargeThreshold { get { return _useLargeThreshold; } set { SetValue(ref _useLargeThreshold, value); } }
		private bool _useLargeThreshold = false;

		//Internals
		internal bool VoiceOnly { get { return _voiceOnly; } set { SetValue(ref _voiceOnly, value); } }
		private bool _voiceOnly;
		internal uint VoiceClientId { get { return _voiceClientId; } set { SetValue(ref _voiceClientId, value); } }
		private uint _voiceClientId;

		internal virtual bool EnableVoice => _voiceMode != DiscordVoiceMode.Disabled;

		public new DiscordWSClientConfig Clone()
		{
			var config = MemberwiseClone() as DiscordWSClientConfig;
			config._isLocked = false;
			return config;
		}
	}
}