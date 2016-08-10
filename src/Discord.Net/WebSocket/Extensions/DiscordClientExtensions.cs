using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket
{
    public static class DiscordClientExtensions
    {
        public static IPrivateChannel GetPrivateChannel(this DiscordSocketClient client, ulong id)
            => client.GetChannel(id) as IPrivateChannel;

        public static IDMChannel GetDMChannel(this DiscordSocketClient client, ulong id)
            => client.GetPrivateChannelAsync(id) as IDMChannel;
        public static IEnumerable<IDMChannel> GetDMChannels(this DiscordSocketClient client)
            => client.GetPrivateChannels().Select(x => x as IDMChannel).Where(x => x != null);

        public static IGroupChannel GetGroupChannel(this DiscordSocketClient client, ulong id)
            => client.GetPrivateChannel(id) as IGroupChannel;
        public static IEnumerable<IGroupChannel> GetGroupChannels(this DiscordSocketClient client)
            => client.GetPrivateChannels().Select(x => x as IGroupChannel).Where(x => x != null);
        
        public static IVoiceRegion GetVoiceRegion(this DiscordSocketClient client, string id)
            => client.VoiceRegions.FirstOrDefault(r => r.Id == id);
        public static IReadOnlyCollection<IVoiceRegion> GetVoiceRegions(this DiscordSocketClient client) =>
            client.VoiceRegions;
        public static IVoiceRegion GetOptimalVoiceRegion(this DiscordSocketClient client)
            => client.VoiceRegions.FirstOrDefault(x => x.IsOptimal);

        public static IGuild GetGuild(this DiscordSocketClient client, ulong id) =>
            client.DataStore.GetGuild(id);
        public static GuildEmbed? GetGuildEmbed(this DiscordSocketClient client, ulong id)
        {
            var guild = client.DataStore.GetGuild(id);
            if (guild != null)
                return new GuildEmbed(guild.IsEmbeddable, guild.EmbedChannelId);
            return null;
        }        
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
