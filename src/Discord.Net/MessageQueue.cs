using Discord.API.Client.Rest;
using Discord.Logging;
using Discord.Net.Rest;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net
{
    /// <summary> Manages an outgoing message queue for DiscordClient. </summary>
    public class MessageQueue
    {
        private struct MessageEdit
        {
            public readonly Message Message;
            public readonly string NewText;

            public MessageEdit(Message message, string newText)
            {
                Message = message;
                NewText = newText;
            }
        }

        private const int WarningStart = 30;

        private readonly Random _nonceRand;
        private readonly RestClient _rest;
        private readonly Logger _logger;
        private readonly ConcurrentQueue<Message> _pendingSends;
        private readonly ConcurrentQueue<MessageEdit> _pendingEdits;
        private readonly ConcurrentQueue<Message> _pendingDeletes;
        private readonly ConcurrentDictionary<int, string> _pendingSendsByNonce;
        private int _count, _nextWarning;

        /// <summary> Gets the current number of queued actions. </summary>
        public int Count => _count;
        /// <summary> Gets the current number of queued sends. </summary>
        public int SendCount => _pendingSends.Count;
        /// <summary> Gets the current number of queued edits. </summary>
        public int EditCount => _pendingEdits.Count;
        /// <summary> Gets the current number of queued deletes. </summary>
        public int DeleteCount => _pendingDeletes.Count;

        internal MessageQueue(RestClient rest, Logger logger)
        {
            _rest = rest;
            _logger = logger;
            _nextWarning = WarningStart;

            _nonceRand = new Random();
            _pendingSends = new ConcurrentQueue<Message>();
            _pendingEdits = new ConcurrentQueue<MessageEdit>();
            _pendingDeletes = new ConcurrentQueue<Message>();
            _pendingSendsByNonce = new ConcurrentDictionary<int, string>();
        }

        internal Message QueueSend(Channel channel, string text, bool isTTS)
        {
            Message msg = new Message(0, channel, channel.IsPrivate ? channel.Client.PrivateUser : channel.Server.CurrentUser);
            msg.IsTTS = isTTS;
            msg.RawText = text;
            msg.Text = msg.Resolve(text);
            msg.Nonce = GenerateNonce();
            if (_pendingSendsByNonce.TryAdd(msg.Nonce, text))
            {
                msg.State = MessageState.Queued;
                IncrementCount();
                _pendingSends.Enqueue(msg);
            }
            else
                msg.State = MessageState.Failed;
            return msg;
        }
        internal void QueueEdit(Message msg, string text)
        {
            string msgText = msg.RawText;
            if (msg.State == MessageState.Queued && _pendingSendsByNonce.TryUpdate(msg.Nonce, text, msgText))
            {
                //Successfully edited the message before it was sent.
                return;
            }
            IncrementCount();
            _pendingEdits.Enqueue(new MessageEdit(msg, text));
        }
        internal void QueueDelete(Message msg)
        {
            string ignored;
            if (msg.State == MessageState.Queued && _pendingSendsByNonce.TryRemove(msg.Nonce, out ignored))
            {
                //Successfully stopped the message from being sent
                msg.State = MessageState.Aborted;
                return;
            }
            IncrementCount();
            _pendingDeletes.Enqueue(msg);
        }

        internal Task[] Run(CancellationToken cancelToken)
        {
            return new[]
            {
                RunSendQueue(cancelToken),
                RunEditQueue(cancelToken),
                RunDeleteQueue(cancelToken)
            };
        }
        private Task RunSendQueue(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        Message msg;
                        while (_pendingSends.TryDequeue(out msg))
                        {
                            DecrementCount();
                            string text;
                            if (_pendingSendsByNonce.TryRemove(msg.Nonce, out text)) //If it was deleted from queue, this will fail
                            {
                                try
                                {
                                    msg.RawText = text;
                                    msg.Text = msg.Resolve(text);
                                    var request = new SendMessageRequest(msg.Channel.Id)
                                    {
                                        Content = msg.RawText,
                                        Nonce = msg.Nonce.ToString(),
                                        IsTTS = msg.IsTTS
                                    };
                                    var response = await _rest.Send(request).ConfigureAwait(false);
                                    msg.State = MessageState.Normal;
                                    msg.Id = response.Id;
                                    msg.Update(response);
                                }
                                catch (Exception ex)
                                {
                                    msg.State = MessageState.Failed;
                                    _logger.Error($"Failed to send message to {msg.Channel.Path}", ex);
                                }
                            }
                        }
                        await Task.Delay(Discord.DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }
        private Task RunEditQueue(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        MessageEdit edit;
                        while (_pendingEdits.TryPeek(out edit) && edit.Message.State != MessageState.Queued)
                        {
                            if (_pendingEdits.TryDequeue(out edit))
                            {
                                DecrementCount();
                                if (edit.Message.State == MessageState.Normal)
                                {
                                    try
                                    {
                                        var request = new UpdateMessageRequest(edit.Message.Channel.Id, edit.Message.Id)
                                        {
                                            Content = edit.NewText
                                        };
                                        await _rest.Send(request).ConfigureAwait(false);
                                    }
                                    catch (Exception ex) { _logger.Error($"Failed to edit message {edit.Message.Path}", ex); }
                                }
                            }
                        }
                        await Task.Delay(Discord.DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }
        private Task RunDeleteQueue(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        Message msg;
                        while (_pendingDeletes.TryPeek(out msg) && msg.State != MessageState.Queued)
                        {
                            if (_pendingDeletes.TryDequeue(out msg))
                            {
                                DecrementCount();
                                if (msg.State == MessageState.Normal)
                                {
                                    try
                                    {
                                        var request = new DeleteMessageRequest(msg.Channel.Id, msg.Id);
                                        await _rest.Send(request).ConfigureAwait(false);
                                    }
                                    catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { } //Ignore
                                    catch (Exception ex) { _logger.Error($"Failed to delete message {msg.Path}", ex); }
                                }
                            }
                        }

                        await Task.Delay(Discord.DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        private void IncrementCount()
        {
            int count = Interlocked.Increment(ref _count);
            if (count >= _nextWarning)
            {
                _nextWarning <<= 1;
                int sendCount = _pendingSends.Count;
                int editCount = _pendingEdits.Count;
                int deleteCount = _pendingDeletes.Count;
                count = sendCount + editCount + deleteCount; //May not add up due to async
                _logger.Warning($"Queue is backed up, currently at {count} actions ({sendCount} sends, {editCount} edits, {deleteCount} deletes).");
            }
            else if (count < WarningStart) //Reset once the problem is solved
                _nextWarning = WarningStart;
        }
        private void DecrementCount()
        {
            int count = Interlocked.Decrement(ref _count);
            if (count < (WarningStart / 2)) //Reset once the problem is solved
                _nextWarning = WarningStart;
        }

        /// <summary> Clears all queued message sends/edits/deletes. </summary>
        public void Clear()
        {
            Message msg;
            MessageEdit edit;

            while (_pendingSends.TryDequeue(out msg))
                DecrementCount();
            while (_pendingEdits.TryDequeue(out edit))
                DecrementCount();
            while (_pendingDeletes.TryDequeue(out msg))
                DecrementCount();
            _pendingSendsByNonce.Clear();
        }

        private int GenerateNonce()
        {
            lock (_nonceRand)
                return _nonceRand.Next(1, int.MaxValue);
        }
    }
}
