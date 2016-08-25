using System;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public interface IAudioClient
    {
        /// <summary> Fired when the client connects to Discord. </summary>
        event Func<Task> Connected;
        /// <summary> Fired when the client disconnects from Discord. </summary>
        event Func<Exception, Task> Disconnected;
        /// <summary> Fired in response to a heartbeat, providing the old and new latency. </summary>
        event Func<int, int, Task> LatencyUpdated;

        /// <summary> Gets the API client used for communicating with Discord. </summary>
        DiscordVoiceAPIClient ApiClient { get; }
        /// <summary> Gets the current connection state of this client. </summary>
        ConnectionState ConnectionState { get; }
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        int Latency { get; }

        /// <summary> Disconnects the current client from Discord. </summary>
        Task DisconnectAsync();

        /// <summary> Creates an Opus stream for sending raw Opus-encoded data. </summary>
        RTPWriteStream CreateOpusStream(int samplesPerFrame, int bufferSize = 4000);
        /// <summary> Creates a PCM stream for sending unencoded PCM data. </summary>
        OpusEncodeStream CreatePCMStream(int samplesPerFrame, int? bitrate = null, OpusApplication application = OpusApplication.MusicOrMixed, int bufferSize = 4000);
    }
}
