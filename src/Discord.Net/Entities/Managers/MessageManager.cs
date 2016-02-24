using APIMessage = Discord.API.Client.Message;
using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
    internal class MessageManager : IEnumerable<Message>
    {
        private readonly ITextChannel _channel;
        private readonly int _size;
        private readonly ConcurrentDictionary<ulong, Message> _messages;
        private readonly ConcurrentQueue<ulong> _orderedMessages;

        public MessageManager(ITextChannel channel, int size = 0)
        {
            _channel = channel;
            _size = size;
            if (size > 0)
            {
                _messages = new ConcurrentDictionary<ulong, Message>(2, size);
                _orderedMessages = new ConcurrentQueue<ulong>();
            }
        }

        internal Message Add(APIMessage model, User user) => Add(new Message(model, _channel, user));
        internal Message Add(ulong id, User user) => Add(new Message(id, _channel, user));
        private Message Add(Message message)
        {
            message.State = MessageState.Normal;
            if (_size > 0)
            {
                if (_messages.TryAdd(message.Id, message))
                {
                    _orderedMessages.Enqueue(message.Id);

                    ulong msgId;
                    while (_orderedMessages.Count > _size && _orderedMessages.TryDequeue(out msgId))
                    {
                        Message msg;
                        if (_messages.TryRemove(msgId, out msg))
                            msg.State = MessageState.Detached;
                    }
                }
            }
            return message;
        }
        internal Message Remove(ulong id)
        {
            if (_size > 0)
            {
                Message msg;
                if (_messages.TryRemove(id, out msg))
                    return msg;
            }
            return new Message(id, _channel, null) { State = MessageState.Deleted };
        }
        
        public Message Get(ulong id, ulong? userId = null)
        {
            if (_messages != null)
            {
                Message result;
                if (_messages.TryGetValue(id, out result))
                    return result;
            }
            return new Message(id, _channel, userId != null ? (_channel as Channel).GetUser(userId.Value) : null) { State = MessageState.Detached };
        }

        public async Task<Message[]> Download(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return new Message[0];

            try
            {
                var request = new GetMessagesRequest(_channel.Id)
                {
                    Limit = limit,
                    RelativeDir = relativeMessageId.HasValue ? relativeDir == Relative.Before ? "before" : "after" : null,
                    RelativeId = relativeMessageId ?? 0
                };
                var msgs = await _channel.Client.ClientAPI.Send(request).ConfigureAwait(false);
                var server = (_channel as PublicChannel)?.Server;

                return msgs.Select(x =>
                {
                    Message msg = null;
                    ulong id = x.Author.Id;
                    var user = server?.GetUser(id) ?? (_channel as Channel).GetUser(id);
                    msg = new Message(x, _channel, user);
                    return msg;
                }).ToArray();
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                return new Message[0];
            }
        }

        public Task<Message> Send(string text, bool isTTS)
        {
            if (text == "") throw new ArgumentException("Value cannot be blank", nameof(text));
            if (text.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {DiscordConfig.MaxMessageSize} characters or less.");
            return Task.FromResult(_channel.Client.MessageQueue.QueueSend(_channel, text, isTTS));
        }
        public async Task<Message> SendFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
                return await SendFile(Path.GetFileName(filePath), stream).ConfigureAwait(false);
        }
        public async Task<Message> SendFile(string filename, Stream stream)
        {
            var request = new SendFileRequest(_channel.Id)
            {
                Filename = filename,
                Stream = stream
            };
            var response = await _channel.Client.ClientAPI.Send(request).ConfigureAwait(false);

            return Add(response, (_channel as Channel).CurrentUser);
        }

        public IEnumerator<Message> GetEnumerator()
        {
            return _orderedMessages
                .Select(x =>
                {
                    Message msg;
                    if (_messages.TryGetValue(x, out msg))
                        return msg;
                    return null;
                })
                .Where(x => x != null).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
