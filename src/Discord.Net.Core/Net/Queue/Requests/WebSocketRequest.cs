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
        public RequestOptions Options { get; }
        public CancellationToken CancelToken { get; internal set; }

        public WebSocketRequest(IWebSocketClient client, byte[] data, bool isText, RequestOptions options)
        {
            Preconditions.NotNull(options, nameof(options));

            Client = client;
            Data = data;
            IsText = isText;
            Options = options;
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
