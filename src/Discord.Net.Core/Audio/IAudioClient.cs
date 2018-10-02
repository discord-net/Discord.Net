using System;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public interface IAudioClient : IDisposable
    {
        event Func<Task> Connected;
        event Func<Exception, Task> Disconnected;
        event Func<int, int, Task> LatencyUpdated;
        event Func<int, int, Task> UdpLatencyUpdated;
        event Func<ulong, AudioInStream, Task> StreamCreated;
        event Func<ulong, Task> StreamDestroyed;
        event Func<ulong, bool, Task> SpeakingUpdated;

        /// <summary> Gets the current connection state of this client. </summary>
        ConnectionState ConnectionState { get; }
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the voice WebSocket server. </summary>
        int Latency { get; }
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the voice UDP server. </summary>
        int UdpLatency { get; }

        Task StopAsync();
        Task SetSpeakingAsync(bool value);

        /// <summary>Creates a new outgoing stream accepting Opus-encoded data.</summary>
        AudioOutStream CreateOpusStream(int bufferMillis = 1000);
        /// <summary>Creates a new outgoing stream accepting Opus-encoded data. This is a direct stream with no internal timer.</summary>
        AudioOutStream CreateDirectOpusStream();
        /// <summary>Creates a new outgoing stream accepting PCM (raw) data.</summary>
        AudioOutStream CreatePCMStream(AudioApplication application, int? bitrate = null, int bufferMillis = 1000, int packetLoss = 30);
        /// <summary>Creates a new direct outgoing stream accepting PCM (raw) data. This is a direct stream with no internal timer.</summary>
        AudioOutStream CreateDirectPCMStream(AudioApplication application, int? bitrate = null, int packetLoss = 30);
    }
}
