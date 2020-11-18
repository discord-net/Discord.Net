using Discord.Net.WebSockets;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class WebSocketRequest : IRequest
    {
        public IWebSocketClient Client { get; }
        public byte[] Data { get; }
        public bool IsText { get; }
        public bool IgnoreLimit { get; }
        public DateTimeOffset? TimeoutAt { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public RequestOptions Options { get; }
        public CancellationToken CancelToken { get; internal set; }

        public WebSocketRequest(IWebSocketClient client, byte[] data, bool isText, bool ignoreLimit, RequestOptions options)
        {
            Preconditions.NotNull(options, nameof(options));

            Client = client;
            Data = data;
            IsText = isText;
            IgnoreLimit = ignoreLimit;
            Options = options;
            TimeoutAt = options.Timeout.HasValue ? DateTimeOffset.UtcNow.AddMilliseconds(options.Timeout.Value) : (DateTimeOffset?)null;
            Promise = new TaskCompletionSource<Stream>();
        }

        public async Task SendAsync()
        {
            await Client.SendAsync(Data, 0, Data.Length, IsText).ConfigureAwait(false);
        }
    }
}
