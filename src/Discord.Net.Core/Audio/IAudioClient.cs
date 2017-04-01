using System;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public interface IAudioClient : IDisposable
    {
        event Func<Task> Connected;
        event Func<Exception, Task> Disconnected;
        event Func<int, int, Task> LatencyUpdated;
        event Func<ulong, AudioInStream, Task> StreamCreated;
        event Func<ulong, Task> StreamDestroyed;
        event Func<ulong, bool, Task> SpeakingUpdated;

        /// <summary> Gets the current connection state of this client. </summary>
        ConnectionState ConnectionState { get; }
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        int Latency { get; }

        Task StopAsync();

        /// <summary>
        /// Creates a new outgoing stream accepting Opus-encoded data.
        /// </summary>
        /// <param name="samplesPerFrame">Samples per frame. Must be 120, 240, 480, 960, 1920 or 2880, representing 2.5, 5, 10, 20, 40 or 60 milliseconds respectively.</param>
        /// <returns></returns>
        AudioOutStream CreateOpusStream(int samplesPerFrame, int bufferMillis = 1000);
        /// <summary>
        /// Creates a new outgoing stream accepting Opus-encoded data. This is a direct stream with no internal timer.
        /// </summary>
        /// <param name="samplesPerFrame">Samples per frame. Must be 120, 240, 480, 960, 1920 or 2880, representing 2.5, 5, 10, 20, 40 or 60 milliseconds respectively.</param>
        /// <returns></returns>
        AudioOutStream CreateDirectOpusStream(int samplesPerFrame);
        /// <summary>
        /// Creates a new outgoing stream accepting PCM (raw) data.
        /// </summary>
        /// <param name="samplesPerFrame">Samples per frame. Must be 120, 240, 480, 960, 1920 or 2880, representing 2.5, 5, 10, 20, 40 or 60 milliseconds respectively.</param>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        AudioOutStream CreatePCMStream(AudioApplication application, int samplesPerFrame, int channels = 2, int? bitrate = null, int bufferMillis = 1000);
        /// <summary>
        /// Creates a new direct outgoing stream accepting PCM (raw) data. This is a direct stream with no internal timer.
        /// </summary>
        /// <param name="samplesPerFrame">Samples per frame. Must be 120, 240, 480, 960, 1920 or 2880, representing 2.5, 5, 10, 20, 40 or 60 milliseconds respectively.</param>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        AudioOutStream CreateDirectPCMStream(AudioApplication application, int samplesPerFrame, int channels = 2, int? bitrate = null);
    }
}
