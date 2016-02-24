using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public class TextChannel : Channel, ITextChannel, IPublicChannel, IMentionable
    {
        public Server Server { get; }
        public string Mention { get; }
        public IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }
        public IEnumerable<Message> Messages { get; }

        public string Topic { get; set; }
        public bool IsTyping { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }

        public override DiscordClient Client => null;
        public override ChannelType Type => default(ChannelType);
        public override User CurrentUser => null;
        public override IEnumerable<User> Users => null;

        public Message GetMessage(ulong id) => null;
        public PermissionOverwrite? GetPermissionsRule(User user) => null;
        public PermissionOverwrite? GetPermissionsRule(Role role) => null;

        public Task<IEnumerable<Message>> DownloadMessages(int limit) => null;
        public Task<IEnumerable<Message>> DownloadMessages(int limit, ulong? relativeMessageId, Relative relativeDir) => null;
        public Task<IEnumerable<Invite>> DownloadInvites() => null;

        public Task<Message> SendMessage(string text, bool isTTS = false) => null;
        public Task<Message> SendFile(string path, string text = null, bool isTTS = false) => null;
        public Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false) => null;

        public Task SendIsTyping() => null;

        public Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = default(int?), bool tempMembership = false, bool withXkcd = false) => null;

        public Task AddPermissionsRule(User user, ChannelPermissions allow, ChannelPermissions deny) => null;
        public Task AddPermissionsRule(User user, TriStateChannelPermissions permissions) => null;
        public Task AddPermissionsRule(Role role, ChannelPermissions allow, ChannelPermissions deny) => null;
        public Task AddPermissionsRule(Role role, TriStateChannelPermissions permissions) => null;
        public Task RemovePermissionsRule(User user) => null;
        public Task RemovePermissionsRule(Role role) => null;

        public Task Delete() => null;
        public override Task Save() => null;
    }
}
