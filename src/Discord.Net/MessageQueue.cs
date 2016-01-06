using Discord.API.Client.Rest;
using Discord.Logging;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net
{
    /// <summary> Manages an outgoing message queue for DiscordClient. </summary>
    public sealed class MessageQueue
    {
        private interface IQueuedAction
        {
            Task Do(MessageQueue queue);
        }

        private struct SendAction : IQueuedAction
        {
            private readonly Message _msg;

            public SendAction(Message msg)
            {
                _msg = msg;
            }
            Task IQueuedAction.Do(MessageQueue queue) => queue.Send(_msg);
        }
        private struct EditAction : IQueuedAction
        {
            private readonly Message _msg;
            private readonly string _text;

            public EditAction(Message msg, string text)
            {
                _msg = msg;
                _text = text;
            }
            Task IQueuedAction.Do(MessageQueue queue) => queue.Edit(_msg, _text);
        }
        private struct DeleteAction : IQueuedAction
        {
            private readonly Message _msg;

            public DeleteAction(Message msg)
            {
                _msg = msg;
            }
            Task IQueuedAction.Do(MessageQueue queue) => queue.Delete(_msg);
        }

        private const int WarningStart = 30;

        private readonly Random _nonceRand;
        private readonly DiscordClient _client;
        private readonly Logger _logger;
        private readonly ConcurrentQueue<IQueuedAction> _pendingActions;
        private readonly ConcurrentDictionary<int, Message> _pendingSends;
        private int _nextWarning;

        /// <summary> Gets the current number of queued actions. </summary>
        public int Count { get; private set; }

        internal MessageQueue(DiscordClient client, Logger logger)
        {
            _client = client;
            _logger = logger;

            _nonceRand = new Random();
            _pendingActions = new ConcurrentQueue<IQueuedAction>();
            _pendingSends = new ConcurrentDictionary<int, Message>();
        }

        internal Message QueueSend(Channel channel, string text, bool isTTS)
        {
            Message msg = new Message(0, channel, channel.IsPrivate ? _client.PrivateUser : channel.Server.CurrentUser);
            msg.RawText = text;
            msg.Text = msg.Resolve(text);
            msg.Nonce = GenerateNonce();
            msg.State = MessageState.Queued;
            _pendingSends.TryAdd(msg.Nonce, msg);
            _pendingActions.Enqueue(new SendAction(msg));
            return msg;
        }
        internal void QueueEdit(Message msg, string text)
        {
            _pendingActions.Enqueue(new EditAction(msg, text));
        }
        internal void QueueDelete(Message msg)
        {
            Message ignored;
            if (msg.State == MessageState.Queued && _pendingSends.TryRemove(msg.Nonce, out ignored))
            {
                msg.State = MessageState.Aborted;
                return;
            }

            _pendingActions.Enqueue(new DeleteAction(msg));
        }

        internal Task Run(CancellationToken cancelToken, int interval)
        {
            _nextWarning = WarningStart;
            return Task.Run(async () =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    Count = _pendingActions.Count;
                    if (Count >= _nextWarning)
                    {
                        _nextWarning *= 2;
                        _logger.Warning($"Queue is backed up, currently at {Count} actions.");
                    }
                    else if (Count < WarningStart) //Reset once the problem is solved
                        _nextWarning = WarningStart;

                    IQueuedAction queuedAction;
                    while (_pendingActions.TryDequeue(out queuedAction))
                        await queuedAction.Do(this);

                    await Task.Delay(interval).ConfigureAwait(false);
                }
            });
        }

        internal async Task Send(Message msg)
        {
            if (_pendingSends.TryRemove(msg.Nonce, out msg)) //Remove it from pending
            {
                try
                {
                    var request = new SendMessageRequest(msg.Channel.Id)
                    {
                        Content = msg.RawText,
                        Nonce = msg.Nonce.ToString(),
                        IsTTS = msg.IsTTS
                    };
                    var response = await _client.ClientAPI.Send(request).ConfigureAwait(false);
                    msg.Update(response);
                    msg.State = MessageState.Normal;
                }
                catch (Exception ex) { msg.State = MessageState.Failed; _logger.Error("Failed to send message", ex); }
            }
        }
        internal async Task Edit(Message msg, string text)
        {
            if (msg.State == MessageState.Normal)
            {
                try
                {
                    var request = new UpdateMessageRequest(msg.Channel.Id, msg.Id)
                    {
                        Content = text
                    };
                    await _client.ClientAPI.Send(request).ConfigureAwait(false);
                }
                catch (Exception ex) { msg.State = MessageState.Failed; _logger.Error("Failed to edit message", ex); }
            }
        }
        internal async Task Delete(Message msg)
        {                
            if (msg.State == MessageState.Normal)
            {
                try
                {
                    var request = new DeleteMessageRequest(msg.Channel.Id, msg.Id);
                    await _client.ClientAPI.Send(request).ConfigureAwait(false);
                }
                catch (Exception ex) { msg.State = MessageState.Failed; _logger.Error("Failed to delete message", ex); }
            }
        }

        /// <summary> Clears all queued message sends/edits/deletes </summary>
        public void Clear()
        {
            IQueuedAction ignored;
            while (_pendingActions.TryDequeue(out ignored)) { }
        }

        private int GenerateNonce()
        {
            lock (_nonceRand)
                return _nonceRand.Next(1, int.MaxValue);
        }
    }
}
