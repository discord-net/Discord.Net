using Discord.API.Client.Rest;
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
            public readonly ulong[] MentionedUsers;
            public readonly bool IsTTS;

            public MessageQueueItem(ulong id, ulong channelId, string text, ulong[] userIds, bool isTTS)
            {
                Id = id;
                ChannelId = channelId;
                Text = text;
                MentionedUsers = userIds;
                IsTTS = isTTS;
            }
        }

        private readonly Random _nonceRand;
        private readonly DiscordClient _client;
        private readonly ConcurrentQueue<MessageQueueItem> _pending;

        internal MessageQueue(DiscordClient client)
        {
            _client = client;
            _nonceRand = new Random();
            _pending = new ConcurrentQueue<MessageQueueItem>();
        }

        public void QueueSend(ulong channelId, string text, ulong[] userIds, bool isTTS)
        {
            _pending.Enqueue(new MessageQueueItem(0, channelId, text, userIds, isTTS));
        }
        public void QueueEdit(ulong channelId, ulong messageId, string text, ulong[] userIds)
        {
            _pending.Enqueue(new MessageQueueItem(channelId, messageId, text, userIds, false));
        }

        internal Task Run(CancellationToken cancelToken, int interval)
        {
            return Task.Run(async () =>
            {
                MessageQueueItem queuedMessage;

                while (!cancelToken.IsCancellationRequested)
                {
                    await Task.Delay(interval).ConfigureAwait(false);
                    while (_pending.TryDequeue(out queuedMessage))
                    {
                        try
                        {
                            if (queuedMessage.Id == 0)
                            {
                                var request = new SendMessageRequest(queuedMessage.ChannelId)
                                {
                                    Content = queuedMessage.Text,
                                    MentionedUserIds = queuedMessage.MentionedUsers,
                                    Nonce = GenerateNonce().ToIdString(),
                                    IsTTS = queuedMessage.IsTTS
                                };
                                await _client.ClientAPI.Send(request).ConfigureAwait(false);
                            }
                            else
                            {
                                var request = new UpdateMessageRequest(queuedMessage.ChannelId, queuedMessage.Id)
                                {
                                    Content = queuedMessage.Text,
                                    MentionedUserIds = queuedMessage.MentionedUsers
                                };
                                await _client.ClientAPI.Send(request).ConfigureAwait(false);
                            }
                        }
                        catch (WebException) { break; }
                        catch (HttpException) { /*msg.State = MessageState.Failed;*/ }
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
