using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public interface ITextChannel : IChannel
    {
        Message GetMessage(ulong id);
        Task<IEnumerable<Message>> DownloadMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before);

        Task<Message> SendMessage(string text, bool isTTS = false);
        Task<Message> SendFile(string filePath, string text = null, bool isTTS = false);
        Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false);

        Task SendIsTyping();
    }
}
