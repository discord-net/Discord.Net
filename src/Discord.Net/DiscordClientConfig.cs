namespace Discord
{
	public class DiscordClientConfig : DiscordWebSocketClientConfig
	{		
		/// <summary> Gets or sets the time (in milliseconds) to wait when the message queue is empty before checking again. </summary>
		public int MessageQueueInterval { get { return _messageQueueInterval; } set { SetValue(ref _messageQueueInterval, value); } }
		private int _messageQueueInterval = 100;

		//Experimental Features
		/// <summary> (Experimental) Enables the client to be simultaneously connected to multiple channels at once (Discord still limits you to one channel per server). </summary>
		public bool EnableVoiceMultiserver { get { return _enableVoiceMultiserver; } set { SetValue(ref _enableVoiceMultiserver, value); } }
		private bool _enableVoiceMultiserver = false;
		/// <summary> (Experimental) Enables or disables the internal message queue. This will allow SendMessage to return immediately and handle messages internally. Messages will set the IsQueued and HasFailed properties to show their progress. </summary>
		public bool UseMessageQueue { get { return _useMessageQueue; } set { SetValue(ref _useMessageQueue, value); } }
		private bool _useMessageQueue = false;
		/// <summary> (Experimental) Maintains the LastActivity property for users, showing when they last made an action (sent message, joined server, typed, etc). </summary>
		public bool TrackActivity { get { return _trackActivity; } set { SetValue(ref _trackActivity, value); } }
		private bool _trackActivity = true;
		/// <summary> (Experimental) Acknowledges all incoming messages so that they appear read. </summary>
		public bool AckMessages { get { return _ackMessages; } set { SetValue(ref _ackMessages, value); } }
		private bool _ackMessages = false;

		//Internal
		internal override bool EnableVoice => base.EnableVoice && !EnableVoiceMultiserver;

		public new DiscordClientConfig Clone()
		{
			var config = MemberwiseClone() as DiscordClientConfig;
			config._isLocked = false;
			return config;
        }
	}
}
