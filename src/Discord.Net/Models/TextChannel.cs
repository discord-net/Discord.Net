using Discord.API.Client.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using APIChannel = Discord.API.Client.Channel;

namespace Discord
{
    public class TextChannel : PublicChannel, IPublicChannel, ITextChannel
    {
        private readonly static Action<TextChannel, TextChannel> _cloner = DynamicIL.CreateCopyMethod<TextChannel>();

        private readonly MessageManager _messages;

        /// <summary> Gets or sets the topic of this channel. </summary>
        public string Topic { get; set; }

        public override ChannelType Type => ChannelType.Text;
        /// <summary> Gets a collection of all messages the client has seen posted in this channel. This collection does not guarantee any ordering. </summary>
        public IEnumerable<Message> Messages => _messages != null ? _messages : Enumerable.Empty<Message>();
        /// <summary> Gets a collection of all users with read access to this channel. </summary>
        public override IEnumerable<User> Users
        {
            get
            {
                if (Client.Config.UsePermissionsCache)
                    return _permissions.Users.Where(x => x.Permissions.ReadMessages == true).Select(x => x.User);
                else
                {
                    ChannelPermissions perms = new ChannelPermissions();
                    return Server.Users.Where(x =>
                    {
                        _permissions.ResolvePermissions(x, ref perms);
                        return perms.ReadMessages == true;
                    });
                }
            }
        }

        internal override MessageManager MessageManager => _messages;

        internal TextChannel(APIChannel model, Server server)
            : base(model, server)
        {
            if (Client.Config.MessageCacheSize > 0)
                _messages = new MessageManager(this, (int)(Client.Config.MessageCacheSize * 1.05));
        }
        private TextChannel(ulong id, Server server)
            : base(id, server)
        {
        }

        internal override void Update(APIChannel model)
        {
            base.Update(model);
            if (model.Topic != null) Topic = model.Topic;
        }
        /// <summary> Save all changes to this channel. </summary>
        public override async Task Save()
        {
            var request = new UpdateChannelRequest(Id)
            {
                Name = Name,
                Topic = Topic,
                Position = Position
            };
            await Client.ClientAPI.Send(request).ConfigureAwait(false);
        }

        public Message GetMessage(ulong id) => _messages.Get(id);
        public Task<Message[]> DownloadMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before)
            => _messages.Download(limit, relativeMessageId, relativeDir);

        public Task<Message> SendMessage(string text, bool isTTS = false) => _messages.Send(text, isTTS);
        public Task<Message> SendFile(string filePath) => _messages.SendFile(filePath);
        public Task<Message> SendFile(string filename, Stream stream) => _messages.SendFile(filename, stream);
        public Task SendIsTyping() => Client.ClientAPI.Send(new SendIsTypingRequest(Id));
        
        internal override Channel Clone()
        {
            var result = new TextChannel(Id, Server);
            _cloner(this, result);
            return result;
        }
    }
}
