using Discord.API.Client.Rest;
using Discord.Logging;
using Discord.Net.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        //private readonly ConcurrentQueue<Task> _pendingTasks;
        private int _nextWarning;
        private int _count;

        /// <summary> Gets the current number of queued actions. </summary>
        public int Count => _count;

        internal MessageQueue(RestClient rest, Logger logger)
        {
            _rest = rest;
            _logger = logger;

            _nonceRand = new Random();
            _pendingSends = new ConcurrentQueue<Message>();
            _pendingEdits = new ConcurrentQueue<MessageEdit>();
            _pendingDeletes = new ConcurrentQueue<Message>();
            _pendingSendsByNonce = new ConcurrentDictionary<int, string>();
            //_pendingTasks = new ConcurrentQueue<Task>();
        }

        internal Message QueueSend(Channel channel, string text, bool isTTS)
        {
            Message msg = new Message(0, channel, channel.IsPrivate ? channel.Client.PrivateUser : channel.Server.CurrentUser);
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
            if (msg.State == MessageState.Queued && _pendingSendsByNonce.TryUpdate(msg.Nonce,  msgText, text))
                return;
            IncrementCount();
            _pendingEdits.Enqueue(new MessageEdit(msg, text));
        }
        internal void QueueDelete(Message msg)
        {
            string ignored;
            if (msg.State == MessageState.Queued && _pendingSendsByNonce.TryRemove(msg.Nonce, out ignored))
            {
                //Successfully stopped the message from being sent in the first place
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
                RunDeleteQueue(cancelToken),
                //RunTaskQueue(cancelToken)
            };
        }
        private Task RunSendQueue(CancellationToken cancelToken)
        {
            _nextWarning = WarningStart;
            return Task.Run((Func<Task>)(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        Message msg;
                        while (_pendingSends.TryDequeue(out msg))
                        {
                            string text;
                            if (_pendingSendsByNonce.TryRemove(msg.Nonce, out text)) //If it was delete from queue, this will fail
                            {
                                msg.RawText = text;
                                msg.Text = msg.Resolve(text);
                                await Send(msg).ConfigureAwait(false);
                            }
                        }

                        await Task.Delay((int)Discord.DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            }));
        }
        private Task RunEditQueue(CancellationToken cancelToken)
        {
            _nextWarning = WarningStart;
            return Task.Run((Func<Task>)(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        MessageEdit edit;
                        while (_pendingEdits.TryPeek(out edit) && edit.Message.State != MessageState.Queued)
                        {
                            _pendingEdits.TryDequeue(out edit);
                            if (edit.Message.State == MessageState.Normal)
                                await Edit(edit.Message, edit.NewText).ConfigureAwait(false);
                        }

                        await Task.Delay((int)Discord.DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            }));
        }
        private Task RunDeleteQueue(CancellationToken cancelToken)
        {
            _nextWarning = WarningStart;
            return Task.Run((Func<Task>)(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        Message msg;
                        while (_pendingDeletes.TryPeek(out msg) && msg.State != MessageState.Queued)
                        {
                            _pendingDeletes.TryDequeue(out msg);
                            if (msg.State == MessageState.Normal)
                                await Delete(msg).ConfigureAwait(false);
                        }

                        await Task.Delay((int)Discord.DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            }));
        }
        /*private Task RunTaskQueue(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                Task task;
                while (!cancelToken.IsCancellationRequested)
                {
                    while (_pendingTasks.TryPeek(out task) && task.IsCompleted)
                    {
                        _pendingTasks.TryDequeue(out task); //Should never fail
                        if (task.IsFaulted)
                            _logger.Warning("Error during Edit/Delete", task.Exception);
                    }
                    await Task.Delay((int)Discord.DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
                }

                //Wait for remaining tasks to complete
                while (_pendingTasks.TryDequeue(out task))
                {
                    if (!task.IsCompleted)
                        await task.ConfigureAwait(false);
                }
            });
        }*/

        internal async Task Send(Message msg)
        {
            try
            {
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
                _logger.Error("Failed to send message", ex);
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
                    await _rest.Send(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to edit message", ex);
                }
            }
        }
        internal async Task Delete(Message msg)
        {                
            if (msg.State == MessageState.Normal)
            {
                try
                {
                    var request = new DeleteMessageRequest(msg.Channel.Id, msg.Id);
                    await _rest.Send(request).ConfigureAwait(false);
                }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { } //Ignore
                catch (Exception ex)
                {
                    _logger.Error("Failed to delete message", ex);
                }
            }
        }

        private void IncrementCount()
        {
            int count = Interlocked.Increment(ref _count);
            if (count >= _nextWarning)
            {
                _nextWarning *= 2;
                _logger.Warning($"Queue is backed up, currently at {count} actions.");
            }
            else if (count < WarningStart) //Reset once the problem is solved
                _nextWarning = WarningStart;
        }

        /// <summary> Clears all queued message sends/edits/deletes. </summary>
        public void Clear()
        {
            Message msg;
            MessageEdit edit;

            while (_pendingSends.TryDequeue(out msg)) { }
            while (_pendingEdits.TryDequeue(out edit)) { }
            while (_pendingDeletes.TryDequeue(out msg)) { }
            _pendingSendsByNonce.Clear();
        }

        private int GenerateNonce()
        {
            lock (_nonceRand)
                return _nonceRand.Next(1, int.MaxValue);
        }
    }
}
