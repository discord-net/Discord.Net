using Discord.Net.WebSockets;
using System;
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
        public int? TimeoutTick { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public CancellationToken CancelToken { get; set; }

        public WebSocketRequest(IWebSocketClient client, byte[] data, bool isText, RequestOptions options) : this(client, data, 0, data.Length, isText, options) { }
        public WebSocketRequest(IWebSocketClient client, byte[] data, int index, int count, bool isText, RequestOptions options)
        {
            Client = client;
            Data = data;
            DataIndex = index;
            DataCount = count;
            IsText = isText;
            if (options != null)
                TimeoutTick = unchecked(Environment.TickCount + options.Timeout.Value);
            else
                TimeoutTick = null;
            Promise = new TaskCompletionSource<Stream>();
        }

        public async Task<Stream> Send()
        {
            await Client.Send(Data, DataIndex, DataCount, IsText).ConfigureAwait(false);
            return null;
        }
    }
}
