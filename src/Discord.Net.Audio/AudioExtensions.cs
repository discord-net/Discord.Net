using System;

namespace Discord.Audio
{
    public static class AudioExtensions
    {
        public static DiscordClient UsingAudio(this DiscordClient client, AudioServiceConfig config = null)
        {
            client.Services.Add(new AudioService(config));
            return client;
        }
        public static DiscordClient UsingAudio(this DiscordClient client, Action<AudioServiceConfig> configFunc = null)
        {
            var config = new AudioServiceConfig();
            configFunc(config);
            client.Services.Add(new AudioService(config));
            return client;
        }
        public static AudioService Audio(this DiscordClient client, bool required = true)
			=> client.Services.Get<AudioService>(required);
    }
}
