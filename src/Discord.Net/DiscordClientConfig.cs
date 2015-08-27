namespace Discord
{
    public class DiscordClientConfig
	{
#if !DNXCORE50
		/// <summary> Enables the voice websocket and UDP client (Experimental!). </summary>
		/// <remarks> This option requires the opus .dll or .so be in the local lib/ folder. </remarks>
		public bool EnableVoice { get; set; } = false;
#endif
		/// <summary> Enables the verbose DebugMessage event handler. May hinder performance but should help debug any issues. </summary>
		public bool EnableDebug { get; set; } = false;

		/// <summary> Max time in milliseconds to wait for the web socket to connect. </summary>
		public int ConnectionTimeout { get; set; } = 5000;
		/// <summary> Max time in milliseconds to wait for the voice web socket to connect. </summary>
		public int VoiceConnectionTimeout { get; set; } = 10000;
		/// <summary> Gets or sets the time (in milliseconds) to wait after an unexpected disconnect before reconnecting. </summary>
		public int ReconnectDelay { get; set; } = 1000;
		/// <summary> Gets or sets the time (in milliseconds) to wait after an reconnect fails before retrying. </summary>
		public int FailedReconnectDelay { get; set; } = 10000;
		/// <summary> Gets or sets the time (in milliseconds) to wait when the websocket's message queue is empty before checking again. </summary>
		public int WebSocketInterval { get; set; } = 100;
		/// <summary> Enables or disables the internal message queue. This will allow SendMessage to return immediately and handle messages internally. Messages will set the IsQueued and HasFailed properties to show their progress. </summary>
		public bool UseMessageQueue { get; set; } = false;
		/// <summary> Gets or sets the time (in milliseconds) to wait when the message queue is empty before checking again. </summary>
		public int MessageQueueInterval { get; set; } = 100;

		public DiscordClientConfig() { }
    }
}
