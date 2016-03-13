using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public class TextChannel : ITextChannel, IMentionable, IModifiable<TextChannel.Properties>
    {
        public sealed class Properties
        {
            public string Name { get; }
            public string Topic { get; }
            public int Position { get; }
        }

        /// <inheritdoc />
        public EntityState State { get; }
        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public Server Server { get; }

        /// <inheritdoc />
        public DiscordClient Discord { get; }
        /// <inheritdoc />
        public ChannelType Type => ChannelType.Public | ChannelType.Text;

        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public string Topic { get; }
        /// <inheritdoc />
        public int Position { get; }

        /// <inheritdoc />
        public string Mention { get; }
        /// <inheritdoc />
        public IEnumerable<PermissionOverwriteEntry> PermissionOverwrites { get; }

        /// <inheritdoc />
        public OverwritePermissions? GetPermissionOverwrite(ServerUser user) => null;
        /// <inheritdoc />
        public OverwritePermissions? GetPermissionOverwrite(Role role) => null;
        /// <inheritdoc />
        public Task<ServerUser> GetUser(ulong id) => null;
        /// <inheritdoc />
        Task<IUser> IChannel.GetUser(ulong id) => null;
        /// <inheritdoc />
        public Task<IEnumerable<ServerUser>> GetUsers() => null;
        /// <inheritdoc />
        Task<IEnumerable<IUser>> IChannel.GetUsers() => null;
        /// <inheritdoc />
        public Task<Message> GetMessage(ulong id) => null;
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100) => null;
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before) => null;
        /// <inheritdoc />
        public Task<IEnumerable<Invite>> GetInvites() => null;
        
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(ServerUser user, OverwritePermissions permissions) => null;
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(Role role, OverwritePermissions permissions) => null;
        /// <inheritdoc />
        public Task RemovePermissionOverwrite(ServerUser user) => null;
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
