using APIChannel = Discord.API.Client.Channel;
using Discord.API.Client.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public class PrivateChannel : Channel, IPrivateChannel, ITextChannel
    {
        private readonly static Action<PrivateChannel, PrivateChannel> _cloner = DynamicIL.CreateCopyMethod<PrivateChannel>();

        private readonly MessageManager _messages;
        
        /// <summary> Gets the target user, if this is a private chat. </summary>
        public User Recipient { get; }

        public override DiscordClient Client => Recipient.Client;

        public override ChannelType Type => ChannelType.Private;
        public override User CurrentUser => Client.PrivateUser;
        public override IEnumerable<User> Users => new User[] { Client.PrivateUser, Recipient };

        internal override MessageManager MessageManager => _messages;
        internal override PermissionManager PermissionManager => null;

        internal PrivateChannel(ulong id, User recipient, APIChannel model)
            : this(id, recipient)
        {
            _messages = new MessageManager(this, Client.Config.MessageCacheSize);
            Update(model);
        }
        private PrivateChannel(ulong id, User recipient)
            :base(id)
        {
            Recipient = recipient;
        }

        internal override User GetUser(ulong id)
        {
            if (id == Recipient.Id) return Recipient;
            else if (id == Client.CurrentUser.Id) return Client.PrivateUser;
            else return null;
        }

        public Message GetMessage(ulong id) => _messages.Get(id);
        public Task<Message[]> DownloadMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before)
            => _messages.Download(limit, relativeMessageId, relativeDir);

        public Task<Message> SendMessage(string text, bool isTTS = false) =>  _messages.Send(text, isTTS);
        public Task<Message> SendFile(string filePath) => _messages.SendFile(filePath);
        public Task<Message> SendFile(string filename, Stream stream) => _messages.SendFile(filename, stream);
        public Task SendIsTyping() => Client.ClientAPI.Send(new SendIsTypingRequest(Id));

        public override string ToString() => $"@{Recipient.Name}";

        internal override void Update(APIChannel model) { }
        internal override Channel Clone()
        {
            var result = new PrivateChannel(Id, Recipient);
            _cloner(this, result);
            return result;
        }
    }
}
