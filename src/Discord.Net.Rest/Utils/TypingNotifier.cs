using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal class TypingNotifier : IDisposable
    {
        private readonly BaseDiscordClient _client;
        private readonly CancellationTokenSource _cancelToken;
        private readonly ulong _channelId;
        private readonly RequestOptions _options;

        public TypingNotifier(BaseDiscordClient discord, IChannel channel, RequestOptions options)
        {
            _client = discord;
            _cancelToken = new CancellationTokenSource();
            _channelId = channel.Id;
            _options = options;
            var _ = Run();
        }

        private async Task Run()
        {
            try
            {
                var token = _cancelToken.Token;
                while (!_cancelToken.IsCancellationRequested)
                {
                    try
                    {
                        await _client.ApiClient.TriggerTypingIndicatorAsync(_channelId);
                    }
                    catch { }
                    await Task.Delay(9750, token);
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            _cancelToken.Cancel();
        }
    }
}
