using System;
using System.Net;

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

	public class DiscordAPIClientConfig
	{
		/// <summary> Specifies the minimum log level severity that will be sent to the LogMessage event. Warning: setting this to debug will really hurt performance but should help investigate any internal issues. </summary>
		public LogSeverity LogLevel { get { return _logLevel; } set { SetValue(ref _logLevel, value); } }
		private LogSeverity _logLevel = LogSeverity.Info;
		
		/// <summary> Max time (in milliseconds) to wait for an API request to complete. </summary>
		public int APITimeout { get { return _apiTimeout; } set { SetValue(ref _apiTimeout, value); } }
		private int _apiTimeout = 10000;

		/// <summary> The proxy to user for API and WebSocket connections. </summary>
		public string ProxyUrl { get { return _proxyUrl; } set { SetValue(ref _proxyUrl, value); } }
		private string _proxyUrl = null;
		/// <summary> The credentials to use for this proxy. </summary>
		public NetworkCredential ProxyCredentials { get { return _proxyCredentials; } set { SetValue(ref _proxyCredentials, value); } }
		private NetworkCredential _proxyCredentials = null;

		//Internals
		internal static readonly string UserAgent = $"Discord.Net/{DiscordClient.Version} (https://github.com/RogueException/Discord.Net)";

		//Lock
		protected bool _isLocked;
		internal void Lock() { _isLocked = true; }
		protected void SetValue<T>(ref T storage, T value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to modify a discord client's configuration after it has been created.");
			storage = value;
		}

		public DiscordAPIClientConfig Clone()
		{
			var config = MemberwiseClone() as DiscordAPIClientConfig;
			config._isLocked = false;
			return config;
		}
	}
}