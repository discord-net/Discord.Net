using System;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public static class AudioExtensions
    {
        public static DiscordClient UsingAudio(this DiscordClient client, AudioServiceConfig config = null)
        {
            client.Services.Add(new AudioService(config));
            return client;
        }
        public static DiscordClient UsingAudio(this DiscordClient client, Action<AudioServiceConfigBuilder> configFunc = null)
        {
            var builder = new AudioServiceConfigBuilder();
            configFunc(builder);
            client.Services.Add(new AudioService(builder));
            return client;
        }

        public static Task<IAudioClient> JoinAudio(this Channel channel) => channel.Client.Services.Get<AudioService>().Join(channel);
        public static Task LeaveAudio(this Channel channel) => channel.Client.Services.Get<AudioService>().Leave(channel);
        public static Task LeaveAudio(this Server server) => server.Client.Services.Get<AudioService>().Leave(server);
        public static IAudioClient GetAudioClient(Server server) => server.Client.Services.Get<AudioService>().GetClient(server);
    }
}
