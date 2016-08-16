using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket
{
    // Todo: Docstrings
    public static class GuildExtensions
    {
        // Channels
        public static IGuildChannel GetChannel(this IGuild guild, ulong id) =>
            GetSocketGuild(guild).GetChannel(id);
        public static IReadOnlyCollection<IGuildChannel> GetChannels(this IGuild guild) =>
            GetSocketGuild(guild).Channels;

        public static ITextChannel GetTextChannel(this IGuild guild, ulong id) =>
            GetSocketGuild(guild).GetChannel(id) as ITextChannel;
        public static IEnumerable<ITextChannel> GetTextChannels(this IGuild guild) =>
            GetSocketGuild(guild).Channels.Select(c => c as ITextChannel).Where(c => c != null);


        public static IVoiceChannel GetVoiceChannel(this IGuild guild, ulong id) =>
            GetSocketGuild(guild).GetChannel(id) as IVoiceChannel;
        public static IEnumerable<IVoiceChannel> GetVoiceChannels(this IGuild guild) =>
            GetSocketGuild(guild).Channels.Select(c => c as IVoiceChannel).Where(c => c != null);

        // Users
        public static IGuildUser GetCurrentUser(this IGuild guild) =>
            GetSocketGuild(guild).CurrentUser;
        public static IGuildUser GetUser(this IGuild guild, ulong id) =>
            GetSocketGuild(guild).GetUser(id);

        public static IReadOnlyCollection<IGuildUser> GetUsers(this IGuild guild) =>
            GetSocketGuild(guild).Members;

        public static int GetUserCount(this IGuild guild) =>
            GetSocketGuild(guild).MemberCount;
        public static int GetCachedUserCount(this IGuild guild) =>
            GetSocketGuild(guild).DownloadedMemberCount;

        //Helpers
        internal static SocketGuild GetSocketGuild(IGuild guild)
        {
            Preconditions.NotNull(guild, nameof(guild));
            var socketGuild = guild as SocketGuild;
            if (socketGuild == null)
                throw new InvalidOperationException("This extension method is only valid on WebSocket Entities");
            return socketGuild;
        }
    }
}
