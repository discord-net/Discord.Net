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
        public int DataIndex { get; }
        public int DataCount { get; }
        public bool IsText { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public CancellationToken CancelToken { get; set; }

        public WebSocketRequest(IWebSocketClient client, byte[] data, bool isText) : this(client, data, 0, data.Length, isText) { }
        public WebSocketRequest(IWebSocketClient client, byte[] data, int index, int count, bool isText)
        {
            Client = client;
            Data = data;
            DataIndex = index;
            DataCount = count;
            IsText = isText;
            Promise = new TaskCompletionSource<Stream>();
        }

        public async Task<Stream> Send()
        {
            await Client.Send(Data, DataIndex, DataCount, IsText).ConfigureAwait(false);
            return null;
        }
    }
}
