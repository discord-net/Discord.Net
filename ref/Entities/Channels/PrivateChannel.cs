using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public class PrivateChannel : ITextChannel, IPrivateChannel
    {
        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public DiscordClient Discord { get; }
        /// <inheritdoc />
        public EntityState State { get; }
        /// <inheritdoc />
        public ChannelType Type { get; }
        /// <inheritdoc />
        public bool IsPrivate => true;
        /// <inheritdoc />
        public bool IsPublic => false;
        /// <inheritdoc />
        public bool IsText => true;
        /// <inheritdoc />
        public bool IsVoice => false;
        
        /// <inheritdoc />
        public User Recipient { get; }

        /// <inheritdoc />
        public Task<IEnumerable<User>> GetUsers() => null;
        /// <inheritdoc />
        public Task<Message> GetMessage(ulong id) => null;
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100) => null;
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before) => null;

        /// <inheritdoc />
        public Task<Message> SendMessage(string text, bool isTTS = false) => null;
        /// <inheritdoc />
        public Task<Message> SendFile(string filePath, string text = null, bool isTTS = false) => null;
        /// <inheritdoc />
        public Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false) => null;

        /// <inheritdoc />
        public Task SendIsTyping() => null;

        /// <inheritdoc />
        public Task Update() => null;
        /// <inheritdoc />
        public Task Delete() => null;
    }
}
