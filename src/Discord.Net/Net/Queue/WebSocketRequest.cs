using Discord.Net.WebSockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    internal class WebSocketRequest : IQueuedRequest
    {
        public IWebSocketClient Client { get; }
        public byte[] Data { get; }
        public int Offset { get; }
        public int Bytes { get; }
        public bool IsText { get; }
        public CancellationToken CancelToken { get; }
        public TaskCompletionSource<Stream> Promise { get; }

        public WebSocketRequest(byte[] data, bool isText, CancellationToken cancelToken) : this(data, 0, data.Length, isText, cancelToken) { }
        public WebSocketRequest(byte[] data, int offset, int length, bool isText, CancellationToken cancelToken)
        {
            Data = data;
            Offset = offset;
            Bytes = length;
            IsText = isText;
            Promise = new TaskCompletionSource<Stream>();
        }

        public async Task<Stream> Send()
        {
            await Client.Send(Data, Offset, Bytes, IsText).ConfigureAwait(false);
            return null;
        }
    }
}
