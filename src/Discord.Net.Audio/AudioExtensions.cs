namespace Discord.Audio
{
    public static class AudioExtensions
    {
        public static DiscordClient UsingAudio(this DiscordClient client, AudioServiceConfig config = null)
        {
            client.Services.Add(new AudioService(config));
            return client;
        }
		public static AudioService Audio(this DiscordClient client, bool required = true)
			=> client.Services.Get<AudioService>(required);
    }
}
