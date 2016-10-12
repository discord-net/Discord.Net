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

        Stream CreateOpusStream(int samplesPerFrame, int bufferSize = 4000);
        Stream CreatePCMStream(int samplesPerFrame, int? bitrate = null, OpusApplication application = OpusApplication.MusicOrMixed, int bufferSize = 4000);
    }
}
