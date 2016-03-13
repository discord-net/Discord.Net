using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public class PrivateChannel : ITextChannel, IChannel
    {
        /// <inheritdoc />
        public DiscordClient Discord { get; }
        /// <inheritdoc />
        public EntityState State { get; }
        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public PrivateUser Recipient { get; }
        /// <inheritdoc />
        public PrivateUser CurrentUser { get; }

        /// <inheritdoc />
        ChannelType IChannel.Type => ChannelType.Private | ChannelType.Text;
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Task<PrivateUser> GetUser(ulong id) => null;
        /// <inheritdoc />
        Task<IUser> IChannel.GetUser(ulong id) => null;
        /// <inheritdoc />
        public Task<IEnumerable<PrivateUser>> GetUsers() => null;
        /// <inheritdoc />
        Task<IEnumerable<IUser>> IChannel.GetUsers() => null;
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
