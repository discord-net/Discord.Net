using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket
{
    public static class SocketClientExtensions
    {
        public static IVoiceRegion GetVoiceRegion(this DiscordSocketClient client, string id)
        {
            var region = client.VoiceRegions.FirstOrDefault(r => r.Id == id);
            return region;
        }

        public static IReadOnlyCollection<IVoiceRegion> GetVoiceRegions(this DiscordSocketClient client) =>
            client.VoiceRegions;

        public static IGuild GetGuild(this DiscordSocketClient client, ulong id) =>
            client.DataStore.GetGuild(id);

        public static GuildEmbed? GetGuildEmbed(this DiscordSocketClient client, ulong id)
        {
            var guild = client.DataStore.GetGuild(id);
            if (guild != null)
                return new GuildEmbed(guild.IsEmbeddable, guild.EmbedChannelId);
            return null;
        }

        public static IReadOnlyCollection<IUserGuild> GetGuildSummaries(this DiscordSocketClient client) =>
            client.Guilds;
        public static IReadOnlyCollection<IGuild> GetGuilds(this DiscordSocketClient client) =>
            client.Guilds;

        public static IChannel GetChannel(this DiscordSocketClient client, ulong id) =>
            client.DataStore.GetChannel(id);

        public static IReadOnlyCollection<IPrivateChannel> GetPrivateChannels(this DiscordSocketClient client) =>
            client.DataStore.PrivateChannels;

        public static IUser GetUser(this DiscordSocketClient client, ulong id) =>
            client.DataStore.GetUser(id);

        public static IUser GetUser(this DiscordSocketClient client, string username, string discriminator) =>
            client.DataStore.Users.Where(x => x.Discriminator == discriminator && x.Username == username).FirstOrDefault();

        public static ISelfUser GetCurrentUser(this DiscordSocketClient client) =>
            client.CurrentUser;

    }
}
