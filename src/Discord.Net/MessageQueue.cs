using Discord.API.Client.Rest;
using Discord.Logging;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net
{
    public class MessageQueue
    {
        private class MessageQueueItem
        {
            public readonly ulong Id, ChannelId;
            public readonly string Text;
            public readonly bool IsTTS;

            public MessageQueueItem(ulong id, ulong channelId, string text, bool isTTS)
            {
                Id = id;
                ChannelId = channelId;
                Text = text;
                IsTTS = isTTS;
            }
        }

        private const int WarningStart = 30;

        private readonly Random _nonceRand;
        private readonly DiscordClient _client;
        private readonly Logger _logger;
        private readonly ConcurrentQueue<MessageQueueItem> _pending;
        private int _nextWarning;

        public int Count { get; private set; }

        internal MessageQueue(DiscordClient client, Logger logger)
        {
            _client = client;
            _logger = logger;

            _nonceRand = new Random();
            _pending = new ConcurrentQueue<MessageQueueItem>();
        }

        internal void QueueSend(ulong channelId, string text, bool isTTS)
        {
            _pending.Enqueue(new MessageQueueItem(0, channelId, text, isTTS));
        }
        internal void QueueEdit(ulong channelId, ulong messageId, string text)
        {
            _pending.Enqueue(new MessageQueueItem(channelId, messageId, text, false));
        }

        internal Task Run(CancellationToken cancelToken, int interval)
        {
            _nextWarning = WarningStart;
            return Task.Run(async () =>
            {
                MessageQueueItem queuedMessage;

                while (!cancelToken.IsCancellationRequested)
                {
                    await Task.Delay(interval).ConfigureAwait(false);

                    Count = _pending.Count;
                    if (Count >= _nextWarning)
                    {
                        _nextWarning *= 2;
                        _logger.Warning($"Queue is backed up, currently at {Count} messages.");
                    }
                    else if (Count < WarningStart) //Reset once the problem is solved
                        _nextWarning = WarningStart;

                    while (_pending.TryDequeue(out queuedMessage))
                    {
                        try
                        {
                            if (queuedMessage.Id == 0)
                            {
                                var request = new SendMessageRequest(queuedMessage.ChannelId)
                                {
                                    Content = queuedMessage.Text,
                                    Nonce = GenerateNonce().ToIdString(),
                                    IsTTS = queuedMessage.IsTTS
                                };
                                await _client.ClientAPI.Send(request).ConfigureAwait(false);
                            }
                            else
                            {
                                var request = new UpdateMessageRequest(queuedMessage.ChannelId, queuedMessage.Id)
                                {
                                    Content = queuedMessage.Text
                                };
                                await _client.ClientAPI.Send(request).ConfigureAwait(false);
                            }
                        }
                        catch (WebException) { break; }
                        catch (HttpException) { /*msg.State = MessageState.Failed;*/ }
                        catch (Exception ex) { _logger.Error(ex); }
                    }
                }
            });
        }

        public void Clear()
        {
            MessageQueueItem ignored;
            while (_pending.TryDequeue(out ignored)) { }
        }

        private ulong GenerateNonce()
        {
            lock (_nonceRand)
                return (ulong)_nonceRand.Next(1, int.MaxValue);
        }
    }
}
