using Discord.API.Rest;
using Discord.Logging;
using Discord.Net;
using Discord.Net.Rest;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


namespace Discord
{
    /// <summary> Manages an outgoing message queue for DiscordClient. </summary>
    public class MessageQueue : IDisposable
    {
        private struct MessageSend
        {
            public readonly TaskCompletionSource<Message> Promise;
            public readonly IMessageChannel Channel;
            public readonly bool IsTTS;
            public readonly string Text;

            public MessageSend(TaskCompletionSource<Message> promise, IMessageChannel channel, bool isTTS, string text)
            {
                Promise = promise;
                Channel = channel;
                IsTTS = isTTS;
                Text = text;
            }
        }
        private struct MessageEdit
        {
            public readonly TaskCompletionSource<object> Promise;
            public readonly Message Message;
            public readonly string NewText;

            public MessageEdit(TaskCompletionSource<object> promise, Message message, string newText)
            {
                Promise = promise;
                Message = message;
                NewText = newText;
            }
        }
        private struct MessageDelete
        {
            public readonly TaskCompletionSource<object> Promise;
            public readonly Message Message;

            public MessageDelete(TaskCompletionSource<object> promise, Message message)
            {
                Promise = promise;
                Message = message;
            }
        }

        private const int WarningStart = 30;
        
        private readonly RestClient _rest;
        private readonly Logger _logger;
        private readonly ConcurrentQueue<MessageSend> _pendingSends;
        private readonly ConcurrentQueue<MessageEdit> _pendingEdits;
        private readonly ConcurrentQueue<MessageDelete> _pendingDeletes;
        private readonly SemaphoreSlim _connectionLock;
        private int _count, _nextWarning;
        private Task[] _tasks;
        private bool _isDisposed = false;

        /// <summary> Gets the current number of queued actions. </summary>
        public int Count => _count;

        internal MessageQueue(RestClient rest, Logger logger)
        {
            _rest = rest;
            _logger = logger;
            _nextWarning = WarningStart;
            
            _connectionLock = new SemaphoreSlim(1, 1);
            _pendingSends = new ConcurrentQueue<MessageSend>();
            _pendingEdits = new ConcurrentQueue<MessageEdit>();
            _pendingDeletes = new ConcurrentQueue<MessageDelete>();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    _connectionLock.Dispose();
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);

        internal async Task Start(CancellationToken cancelToken)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await StartInternal(cancelToken).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        internal async Task StartInternal(CancellationToken cancelToken)
        {
            await StopInternal().ConfigureAwait(false);

            _tasks = new Task[]
            {
                RunSendQueue(cancelToken),
                RunEditQueue(cancelToken),
                RunDeleteQueue(cancelToken)
            };
        }
        internal async Task Stop()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await StopInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task StopInternal()
        {
            if (_tasks != null)
            {
                await Task.WhenAll(_tasks).ConfigureAwait(false);
                _tasks = null;
            }
        }

        internal Task<Message> QueueSend(IMessageChannel channel, string text, bool isTTS)
        {
            var promise = new TaskCompletionSource<Message>();
            IncrementCount();
            _pendingSends.Enqueue(new MessageSend(promise, channel, isTTS, text));
            return promise.Task;
        }
        internal Task QueueEdit(Message msg, string text)
        {
            var promise = new TaskCompletionSource<object>();
            IncrementCount();
            _pendingEdits.Enqueue(new MessageEdit(promise, msg, text));
            return promise.Task;
        }
        internal Task QueueDelete(Message msg)
        {
            var promise = new TaskCompletionSource<object>();
            IncrementCount();
            _pendingDeletes.Enqueue(new MessageDelete(promise, msg));
            return promise.Task;
        }
        
        private Task RunSendQueue(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        MessageSend item;
                        while (_pendingSends.TryDequeue(out item))
                        {
                            try
                            {
                                var request = new CreateMessageRequest(item.Channel.Id)
                                {
                                    Content = item.Text,
                                    IsTTS = item.IsTTS
                                };
                                var response = await _rest.Send(request).ConfigureAwait(false);
                                item.Promise.SetResult(item.Channel.Discord.CreateMessage(item.Channel, item.Channel.GetCurrentUser(), response));
                            }
                            catch (Exception ex) { item.Promise.SetException(ex); }
                            DecrementCount();
                        }
                        await Task.Delay(DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
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
                        MessageEdit item;
                        while (_pendingEdits.TryDequeue(out item))
                        {
                            var msg = item.Message;
                            //if (msg.State != EntityState.Deleted)
                            //{
                                try
                                {
                                    var request = new ModifyMessageRequest(msg.Channel.Id, msg.Id)
                                    {
                                        Content = item.NewText
                                    };
                                    await _rest.Send(request).ConfigureAwait(false);
                                    item.Promise.SetResult(null);
                                }
                                catch (Exception ex) { item.Promise.SetException(ex); }
                            //}
                            DecrementCount();
                        }
                        await Task.Delay(DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
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
                        MessageDelete item;
                        while (_pendingDeletes.TryDequeue(out item))
                        {
                            var msg = item.Message;
                            //if (msg.State != EntityState.Deleted)
                            //{
                                try
                                {
                                    var request = new DeleteMessageRequest(msg.Channel.Id, msg.Id);
                                    await _rest.Send(request).ConfigureAwait(false);
                                    item.Promise.SetResult(null);
                                }
                                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { } //Ignore
                                catch (Exception ex) { item.Promise.SetException(ex); }
                            //}
                            DecrementCount();
                        }

                        await Task.Delay(DiscordConfig.MessageQueueInterval).ConfigureAwait(false);
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
            MessageSend send;
            MessageEdit edit;
            MessageDelete delete;

            while (_pendingSends.TryDequeue(out send))
                DecrementCount();
            while (_pendingEdits.TryDequeue(out edit))
                DecrementCount();
            while (_pendingDeletes.TryDequeue(out delete))
                DecrementCount();
        }
    }
}
