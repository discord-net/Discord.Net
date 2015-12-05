namespace Discord
{
	public class DiscordClientConfig : DiscordWSClientConfig
	{		
		/// <summary> Gets or sets the time (in milliseconds) to wait when the message queue is empty before checking again. </summary>
		public int MessageQueueInterval { get { return _messageQueueInterval; } set { SetValue(ref _messageQueueInterval, value); } }
		private int _messageQueueInterval = 100;
		/// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
		public int MessageCacheLength { get { return _messageCacheLength; } set { SetValue(ref _messageCacheLength, value); } }
		private int _messageCacheLength = 100;

		//Experimental Features
		/// <summary> (Experimental) Enables or disables the internal message queue. This will allow SendMessage to return immediately and handle messages internally. Messages will set the IsQueued and HasFailed properties to show their progress. </summary>
		public bool UseMessageQueue { get { return _useMessageQueue; } set { SetValue(ref _useMessageQueue, value); } }
		private bool _useMessageQueue = false;
		/// <summary> (Experimental) Maintains the LastActivity property for users, showing when they last made an action (sent message, joined server, typed, etc). </summary>
		public bool TrackActivity { get { return _trackActivity; } set { SetValue(ref _trackActivity, value); } }
		private bool _trackActivity = true;
		/// <summary> (Experimental) Acknowledges all incoming messages so that they appear read. </summary>
		public bool AckMessages { get { return _ackMessages; } set { SetValue(ref _ackMessages, value); } }
		private bool _ackMessages = false;

		public new DiscordClientConfig Clone()
		{
			var config = MemberwiseClone() as DiscordClientConfig;
			config._isLocked = false;
			return config;
        }
	}
}
