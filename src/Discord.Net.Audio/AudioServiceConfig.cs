
using System;

namespace Discord.Audio
{
	public enum AudioMode : byte
	{
		Outgoing = 1,
		Incoming = 2,
		Both = Outgoing | Incoming
	}

	public class AudioServiceConfig
	{
		/// <summary> Max time in milliseconds to wait for DiscordAudioClient to connect and initialize. </summary>
		public int ConnectionTimeout { get { return _connectionTimeout; } set { SetValue(ref _connectionTimeout, value); } }
		private int _connectionTimeout = 30000;

		//Experimental Features
		/// <summary> (Experimental) Enables the voice websocket and UDP client and specifies how it will be used. </summary>
		public AudioMode Mode { get { return _mode; } set { SetValue(ref _mode, value); } }
		private AudioMode _mode = AudioMode.Outgoing;

		/// <summary> (Experimental) Enables the voice websocket and UDP client. This option requires the libsodium .dll or .so be in the local or system folder. </summary>
		public bool EnableEncryption { get { return _enableEncryption; } set { SetValue(ref _enableEncryption, value); } }
		private bool _enableEncryption = true;

		/// <summary> (Experimental) Enables the client to be simultaneously connected to multiple channels at once (Discord still limits you to one channel per server). </summary>
		public bool EnableMultiserver { get { return _enableMultiserver; } set { SetValue(ref _enableMultiserver, value); } }
		private bool _enableMultiserver = false;

		/// <summary> Gets or sets the buffer length (in milliseconds) for outgoing voice packets. </summary>
		public int BufferLength { get { return _bufferLength; } set { SetValue(ref _bufferLength, value); } }
		private int _bufferLength = 1000;

		/// <summary> Gets or sets the bitrate used (in kbit/s, between 1 and 512 inclusively) for outgoing voice packets. A null value will use default Opus settings. </summary>
		public int? Bitrate { get { return _bitrate; } set { SetValue(ref _bitrate, value); } }
		private int? _bitrate = null;

        /// <summary> Gets or sets the number of channels (1 or 2) used for outgoing audio. </summary>
        public int Channels { get { return _channels; } set { SetValue(ref _channels, value); } }
        private int _channels = 1;

        //Lock
        protected bool _isLocked;
		internal void Lock() { _isLocked = true; }
		protected void SetValue<T>(ref T storage, T value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to modify a service's configuration after it has been created.");
			storage = value;
		}

		public AudioServiceConfig Clone()
		{
			var config = MemberwiseClone() as AudioServiceConfig;
			config._isLocked = false;
			return config;
		}
	}
}
