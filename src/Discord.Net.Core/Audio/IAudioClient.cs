using System;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public interface IAudioClient
    {
        event Func<Task> Connected;
        event Func<Exception, Task> Disconnected;
        event Func<int, int, Task> LatencyUpdated;
        
        /// <summary> Gets the current connection state of this client. </summary>
        ConnectionState ConnectionState { get; }
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        int Latency { get; }

        Task DisconnectAsync();

        /// <summary>
        /// Creates a new outgoing stream accepting Opus-encoded data.
        /// </summary>
        /// <param name="samplesPerFrame">Samples per frame. Must be 120, 240, 480, 960, 1920 or 2880, representing 2.5, 5, 10, 20, 40 or 60 milliseconds respectively.</param>
        /// <returns></returns>
        Stream CreateOpusStream(int samplesPerFrame);
        /// <summary>
        /// Creates a new outgoing stream accepting Opus-encoded data. This is a direct stream with no internal timer.
        /// </summary>
        /// <param name="samplesPerFrame">Samples per frame. Must be 120, 240, 480, 960, 1920 or 2880, representing 2.5, 5, 10, 20, 40 or 60 milliseconds respectively.</param>
        /// <returns></returns>
        Stream CreateDirectOpusStream(int samplesPerFrame);
        /// <summary>
        /// Creates a new outgoing stream accepting PCM (raw) data.
        /// </summary>
        /// <param name="samplesPerFrame">Samples per frame. Must be 120, 240, 480, 960, 1920 or 2880, representing 2.5, 5, 10, 20, 40 or 60 milliseconds respectively.</param>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        Stream CreatePCMStream(int samplesPerFrame, int channels = 2, int? bitrate = null);
        /// <summary>
        /// Creates a new direct outgoing stream accepting PCM (raw) data. This is a direct stream with no internal timer.
        /// </summary>
        /// <param name="samplesPerFrame">Samples per frame. Must be 120, 240, 480, 960, 1920 or 2880, representing 2.5, 5, 10, 20, 40 or 60 milliseconds respectively.</param>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        Stream CreateDirectPCMStream(int samplesPerFrame, int channels = 2, int? bitrate = null);
    }
}
