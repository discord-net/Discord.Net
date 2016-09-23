using Discord.Net.WebSockets;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class WebSocketRequest : IQueuedRequest
    {
        public IWebSocketClient Client { get; }
        public byte[] Data { get; }
        public bool IsText { get; }
        public int? TimeoutTick { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public CancellationToken CancelToken { get; set; }
        
        public WebSocketRequest(IWebSocketClient client, byte[] data, bool isText, RequestOptions options)
        {
            if (options == null)
                options = RequestOptions.Default;

            Client = client;
            Data = data;
            IsText = isText;
            TimeoutTick = options.Timeout.HasValue ? (int?)unchecked(Environment.TickCount + options.Timeout.Value) : null;
            Promise = new TaskCompletionSource<Stream>();
        }

        public async Task<Stream> SendAsync()
        {
            await Client.SendAsync(Data, 0, Data.Length, IsText).ConfigureAwait(false);
            return null;
        }
    }
}
