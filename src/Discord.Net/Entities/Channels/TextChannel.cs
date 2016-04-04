using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    public class TextChannel : GuildChannel, IMessageChannel, IMentionable
    {
        private readonly MessageManager _messages;
        
        /// <inheritdoc />
        public string Topic { get; private set; }

        /// <inheritdoc />
        public override ChannelType Type => ChannelType.Text;
        /// <inheritdoc />
        public string Mention => MentionHelper.Mention(this);

        internal TextChannel(ulong id, Guild guild, int messageCacheSize, bool usePermissionsCache)
            : base(id, guild, usePermissionsCache)
        {
            _messages = new MessageManager(this, messageCacheSize);
        }

        internal override void Update(Model model)
        {
            Topic = model.Topic;
            base.Update(model);
        }
        
        /// <inheritdoc />
        public Task<Message> GetMessage(ulong id) 
            => _messages.Get(id);
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100) 
            =>  _messages.GetMany(limit);
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before)
            => _messages.GetMany(limit, relativeMessageId, relativeDir);

        /// <inheritdoc />
        public Task<Message> SendMessage(string text, bool isTTS = false)
            => _messages.Send(text, isTTS);
        /// <inheritdoc />
        public Task<Message> SendFile(string filePath, string text = null, bool isTTS = false)
            => _messages.SendFile(filePath, text, isTTS);
        /// <inheritdoc />
        public Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false)
            => _messages.SendFile(stream, filename, text, isTTS);

        /// <inheritdoc />
        public Task TriggerTyping()
            => _messages.TriggerTyping();
        
        public async Task Modify(Action<ModifyTextChannelRequest> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var req = new ModifyTextChannelRequest(Id);
            func(req);
            await Discord.RestClient.Send(req).ConfigureAwait(false);
        }
    }
}
