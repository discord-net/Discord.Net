using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public class TextChannel : ITextChannel, IPublicChannel, IMentionable, IModifiable<TextChannel.Properties>
    {
        public sealed class Properties
        {
            public string Name { get; }
            public string Topic { get; }
            public int Position { get; }
        }

        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public DiscordClient Discord { get; }
        /// <inheritdoc />
        public EntityState State { get; }
        /// <inheritdoc />
        public ChannelType Type => ChannelType.Public | ChannelType.Text;
        /// <inheritdoc />
        public bool IsPrivate => false;
        /// <inheritdoc />
        public bool IsPublic => true;
        /// <inheritdoc />
        public bool IsText => true;
        /// <inheritdoc />
        public bool IsVoice => false;

        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public string Topic { get; }
        /// <inheritdoc />
        public int Position { get; }
        /// <inheritdoc />
        public string Mention { get; }
        /// <inheritdoc />
        public Server Server { get; }
        /// <inheritdoc />
        public IEnumerable<PermissionOverwriteEntry> PermissionOverwrites { get; }
        /// <inheritdoc />
        public IEnumerable<User> Users { get; }

        /// <inheritdoc />
        public OverwritePermissions? GetPermissionOverwrite(User user) => null;
        /// <inheritdoc />
        public OverwritePermissions? GetPermissionOverwrite(Role role) => null;
        /// <inheritdoc />
        public Task<IEnumerable<User>> GetUsers() => null;
        /// <inheritdoc />
        public Task<Message> GetMessage(ulong id) => null;
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100) => null;
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before) => null;
        /// <inheritdoc />
        public Task<IEnumerable<Invite>> GetInvites() => null;
        
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(User user, OverwritePermissions permissions) => null;
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(Role role, OverwritePermissions permissions) => null;
        /// <inheritdoc />
        public Task RemovePermissionOverwrite(User user) => null;
        /// <inheritdoc />
        public Task RemovePermissionOverwrite(Role role) => null;

        /// <inheritdoc />
        public Task<Message> SendMessage(string text, bool isTTS = false) => null;
        /// <inheritdoc />
        public Task<Message> SendFile(string filePath, string text = null, bool isTTS = false) => null;
        /// <inheritdoc />
        public Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false) => null;

        /// <inheritdoc />
        public Task SendIsTyping() => null;

        /// <inheritdoc />
        public Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false) => null;

        /// <inheritdoc />
        public Task Update() => null;
        /// <inheritdoc />
        public Task Modify(Action<Properties> func) => null;
        /// <inheritdoc />
        public Task Delete() => null;
    }
}
