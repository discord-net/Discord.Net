using System.IO;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public interface IAudioClient
    {
        ConnectionState State { get; }
        Channel Channel { get; }
        Server Server { get; }
        Stream OutputStream { get; }

        Task Join(Channel channel);
        Task Disconnect();

        /// <summary> Sends a PCM frame to the voice server. Will block until space frees up in the outgoing buffer. </summary>
        /// <param name="data">PCM frame to send. This must be a single or collection of uncompressed 48Kz monochannel 20ms PCM frames. </param>
        /// <param name="offset">Offset . </param>
        /// <param name="count">Number of bytes in this frame. </param>
        void Send(byte[] data, int offset, int count);
        /// <summary> Clears the PCM buffer. </summary>
        void Clear();
        /// <summary> Blocks until the voice output buffer is empty. </summary>
        void Wait();
    }
}
