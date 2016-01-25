using Discord.Net.Rest;
using Discord.Net.WebSockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public interface IAudioClient
    {
        /// <summary> Gets the unique identifier for this client. </summary>
        int Id { get; }
        /// <summary> Gets the session id for the current connection. </summary>
        string SessionId { get; }
        /// <summary> Gets the current state of this client. </summary>
        ConnectionState State { get; }
        /// <summary> Gets the channel this client is currently a member of. </summary>
        Channel Channel { get; }
        /// <summary> Gets the server this client is bound to. </summary>
        Server Server { get; }
        /// <summary> Gets a stream object that wraps the Send() function. </summary>
        Stream OutputStream { get; }
        /// <summary> Gets a cancellation token that triggers when the client is manually disconnected. </summary>
        CancellationToken CancelToken { get; }

        /// <summary> Gets the internal RestClient for the Client API endpoint. </summary>
        RestClient ClientAPI { get; }
        /// <summary> Gets the internal WebSocket for the Gateway event stream. </summary>
        GatewaySocket GatewaySocket { get; }
        /// <summary> Gets the internal WebSocket for the Voice control stream. </summary>
        VoiceSocket VoiceSocket { get; }

        /// <summary> Moves the client to another channel on the same server. </summary>
        Task Join(Channel channel);
        /// <summary> Disconnects from the Discord server, canceling any pending requests. </summary>
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
