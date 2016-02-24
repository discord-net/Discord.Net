using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public class PrivateChannel : Channel, ITextChannel, IPrivateChannel
    {        
        public User Recipient { get; }
        public IEnumerable<Message> Messages { get; }

        public override DiscordClient Client => null;
        public override ChannelType Type => default(ChannelType);

        public override User CurrentUser => null;
        public override IEnumerable<User> Users => null;

        public Message GetMessage(ulong id) => null;
        public Task<IEnumerable<Message>> DownloadMessages(int limit) => null;
        public Task<IEnumerable<Message>> DownloadMessages(int limit, ulong? relativeMessageId, Relative relativeDir) => null;

        public Task<Message> SendMessage(string text, bool isTTS = false) => null;
        public Task<Message> SendFile(string path, string text = null, bool isTTS = false) => null;
        public Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false) => null;

        public Task SendIsTyping() => null;

        public override Task Save() => null;
    }
}
