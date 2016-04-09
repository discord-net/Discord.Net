using Discord.API.Rest;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord
{
    internal class MessageManager : IEnumerable<Message>
    {
        private readonly IMessageChannel _channel;
        private readonly ConcurrentDictionary<ulong, Message> _messages;
        private readonly ConcurrentQueue<Message> _orderedMessages;
        private readonly int _size;

        public MessageManager(IMessageChannel channel, int size = 0)
        {
            _channel = channel;
            _size = (int)(size * 1.05);
            if (size > 0)
            {
                _messages = new ConcurrentDictionary<ulong, Message>(2, size);
                _orderedMessages = new ConcurrentQueue<Message>();
            }
        }

        internal Message Add(Model model, IUser user)
            => Add(_channel.Discord.CreateMessage(_channel, user, model));
        private Message Add(Message message)
        {
            if (_size > 0)
            {
                if (_messages.TryAdd(message.Id, message))
                {
                    _orderedMessages.Enqueue(message);

                    Message msg;
                    while (_orderedMessages.Count > _size && _orderedMessages.TryDequeue(out msg))
                        _messages.TryRemove(msg.Id, out msg);
                }
            }
            return message;
        }
        internal void Remove(ulong id)
        {
            if (_size > 0)
            {
                Message msg;
                _messages.TryRemove(id, out msg);
            }
        }
        
        public Task<Message> Get(ulong id)
        {
            if (_messages != null)
            {
                Message result;
                if (_messages.TryGetValue(id, out result))
                    return Task.FromResult(result);
            }
            else
                throw new NotSupportedException(); //TODO: Not supported yet

            return Task.FromResult<Message>(null);
        }
        
        public async Task<IEnumerable<Message>> GetMany(int limit = DiscordConfig.MaxMessagesPerBatch, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return ImmutableArray<Message>.Empty;

            if (_messages != null)
            {
                ImmutableArray<Message> cachedMessages;
                if (relativeMessageId == null)
                    cachedMessages = _orderedMessages.ToImmutableArray();
                else if (relativeDir == Relative.Before)
                    cachedMessages = _orderedMessages.Where(x => x.Id < relativeMessageId.Value).ToImmutableArray();
                else
                    cachedMessages = _orderedMessages.Where(x => x.Id > relativeMessageId.Value).ToImmutableArray();

                if (cachedMessages.Length == limit)
                    return cachedMessages;
                else if (cachedMessages.Length > limit)
                    return cachedMessages.Skip(cachedMessages.Length - limit);
                else
                {
                    var missingMessages = await Download(limit - cachedMessages.Length, cachedMessages[0].Id, Relative.Before).ConfigureAwait(false);
                    return missingMessages.SelectMany(x => x).Concat(cachedMessages);
                }
            }
            return (await Download(limit, relativeMessageId, relativeDir).ConfigureAwait(false)).SelectMany(x => x);
        }
        private async Task<IEnumerable<IEnumerable<Message>>> Download(int limit = DiscordConfig.MaxMessagesPerBatch, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before)
        {
            var request = new GetChannelMessagesRequest(_channel.Id)
            {
                Limit = limit,
                RelativeDir = relativeDir,
                RelativeId = relativeMessageId ?? 0
            };
            var guild = (_channel as GuildChannel)?.Guild;
            var recipient = (_channel as DMChannel)?.Recipient;

            int runs = limit / DiscordConfig.MaxMessagesPerBatch;
            int lastRunCount = limit - runs * DiscordConfig.MaxMessagesPerBatch;
            var result = new Message[runs][];

            int i = 0;
            for (; i < runs; i++)
            {
                request.Limit = (i == runs - 1) ? lastRunCount : DiscordConfig.MaxMessagesPerBatch;

                Model[] models = await _channel.Discord.RestClient.Send(request).ConfigureAwait(false);

                //Was this an empty batch?
                if (models.Length == 0) break;

                Message[] msgs = new Message[models.Length];
                for (int j = 0; j < models.Length; j++)
                {
                    var model = models[j];
                    var user = _channel.GetUser(model.Author.Id);
                    msgs[j] = _channel.Discord.CreateMessage(_channel, user, model);
                }
                result[i] = msgs;
                
                request.RelativeId = relativeDir == Relative.Before ? msgs[0].Id : msgs[msgs.Length - 1].Id;

                //Was this an incomplete (the last) batch?
                if (models.Length != DiscordConfig.MaxMessagesPerBatch) { i++; break; }
            }

            //Dont return nulls if we didnt get all the requested messages
            for (; i < runs; i++)
                result[i] = Array.Empty<Message>();

            return result;
        }

        public Task<Message> Send(string text, bool isTTS)
        {
            if (text == "") throw new ArgumentException("Value cannot be blank", nameof(text));
            if (text.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {DiscordConfig.MaxMessageSize} characters or less.");
            return _channel.Discord.MessageQueue.QueueSend(_channel, text, isTTS);
        }
        public async Task<Message> SendFile(string filePath, string text = null, bool isTTS = false)
        {
            using (var stream = File.OpenRead(filePath))
                return await SendFile(stream, Path.GetFileName(filePath), text, isTTS).ConfigureAwait(false);
        }
        public async Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false)
        {
            var request = new UploadFileRequest(_channel.Id)
            {
                Filename = filename,
                Stream = stream
            };
            var response = await _channel.Discord.RestClient.Send(request).ConfigureAwait(false);

            return _channel.Discord.CreateMessage(_channel, _channel.GetCurrentUser(), response);
        }
        public Task TriggerTyping() => _channel.Discord.RestClient.Send(new TriggerTypingIndicatorRequest(_channel.Id));

        public IEnumerator<Message> GetEnumerator() => _orderedMessages.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _orderedMessages.GetEnumerator();
    }
}
