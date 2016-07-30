using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket.Extensions
{
    // TODO: Docstrings
    public static class GuildExtensions
    {
        // Channels

        public static IGuildChannel GetChannel(this IGuild guild, ulong id) =>
            (guild as SocketGuild).GetChannel(id);

        public static ITextChannel GetTextChannel(this IGuild guild, ulong id) =>
            (guild as SocketGuild).GetChannel(id) as ITextChannel;

        public static IEnumerable<ITextChannel> GetTextChannels(this IGuild guild) =>
            (guild as SocketGuild).Channels.Select(c => c as ITextChannel).Where(c => c != null);


        public static IVoiceChannel GetVoiceChannel(this IGuild guild, ulong id) =>
            (guild as SocketGuild).GetChannel(id) as IVoiceChannel;

        public static IEnumerable<IVoiceChannel> GetVoiceChannels(this IGuild guild) =>
            (guild as SocketGuild).Channels.Select(c => c as IVoiceChannel).Where(c => c != null);

        // Users

        public static IGuildUser GetCurrentUser(this IGuild guild) =>
            (guild as SocketGuild).CurrentUser;

        public static IGuildUser GetUser(this IGuild guild, ulong id) =>
            (guild as SocketGuild).GetUser(id);

        public static IEnumerable<IGuildUser> GetUsers(this IGuild guild) =>
            (guild as SocketGuild).Members;

        public static int GetUserCount(this IGuild guild) =>
            (guild as SocketGuild).MemberCount;

        public static int GetCachedUserCount(this IGuild guild) =>
            (guild as SocketGuild).DownloadedMemberCount;
    }
}
