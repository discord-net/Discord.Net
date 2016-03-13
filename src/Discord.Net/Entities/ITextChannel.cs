using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public interface ITextChannel : IChannel
    {
        Message GetMessage(ulong id);
        Task<Message[]> DownloadMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before);

        Task<Message> SendMessage(string text, bool isTTS = false);
        Task<Message> SendFile(string filePath);
        Task<Message> SendFile(string filename, Stream stream);

        Task SendIsTyping();
    }
}
