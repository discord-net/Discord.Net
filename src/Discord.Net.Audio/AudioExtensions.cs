namespace Discord.Audio
{
    public static class AudioExtensions
    {
		public static AudioService Audio(this DiscordClient client, bool required = true)
			=> client.Services.Get<AudioService>(required);
    }
}
