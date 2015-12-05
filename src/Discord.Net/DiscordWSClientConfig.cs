using System;
using System.Reflection;

namespace Discord
{
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

		//Experimental Features
		/// <summary> (Experimental) Instructs Discord to not send send information about offline users, for servers with more than 50 users. </summary>
		public bool UseLargeThreshold { get { return _useLargeThreshold; } set { SetValue(ref _useLargeThreshold, value); } }
		private bool _useLargeThreshold = false;

		public new DiscordWSClientConfig Clone()
		{
			var config = MemberwiseClone() as DiscordWSClientConfig;
			config._isLocked = false;
			return config;
		}
	}
}