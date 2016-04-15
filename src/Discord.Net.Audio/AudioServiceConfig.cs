namespace Discord.Audio
{
	public class AudioServiceConfigBuilder
	{
		/// <summary> Enables the voice websocket and UDP client and specifies how it will be used. </summary>
		public AudioMode Mode { get; set; } = AudioMode.Outgoing;

		/// <summary> Enables the voice websocket and UDP client. This option requires the libsodium .dll or .so be in the local or system folder. </summary>
		public bool EnableEncryption { get; set; } = true;

		/// <summary> Gets or sets the buffer length (in milliseconds) for outgoing voice packets. </summary>
		public int BufferLength { get; set; } = 1000;
        /// <summary> Gets or sets the bitrate used (in kbit/s, between 1 and MaxBitrate inclusively) for outgoing voice packets. A null value will use default Opus settings. </summary>
        public int? Bitrate { get; set; } = null;
        /// <summary> Gets or sets the number of channels (1 or 2) used in both input provided to IAudioClient and output send to Discord. Defaults to 2 (stereo). </summary>
        public int Channels { get; set; } = 2;

        public AudioServiceConfig Build() => new AudioServiceConfig(this);
    }

    public class AudioServiceConfig
    {
        public const int MaxBitrate = 128;

        public AudioMode Mode { get; }

        public bool EnableEncryption { get; }

        public int BufferLength { get; }        
        public int? Bitrate { get; }
        public int Channels { get; }

        internal AudioServiceConfig(AudioServiceConfigBuilder builder)
        {
            Mode = builder.Mode;

            EnableEncryption = builder.EnableEncryption;

            BufferLength = builder.BufferLength;
            Bitrate = builder.Bitrate;
            Channels = builder.Channels;
        }
    }
}
