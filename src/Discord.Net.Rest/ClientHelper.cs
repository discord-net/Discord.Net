using Discord.API.Rest;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class ClientHelper
    {
        //Applications
        public static async Task<RestApplication> GetApplicationInfoAsync(BaseDiscordClient client)
        {
            var model = await client.ApiClient.GetMyApplicationAsync().ConfigureAwait(false);
            return RestApplication.Create(client, model);
        }

        public static async Task<RestChannel> GetChannelAsync(BaseDiscordClient client, 
            ulong id)
        {
            var model = await client.ApiClient.GetChannelAsync(id).ConfigureAwait(false);
            if (model != null)
                return RestChannel.Create(client, model);
            return null;
        }
        public static async Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync(BaseDiscordClient client)
        {
            var models = await client.ApiClient.GetMyPrivateChannelsAsync().ConfigureAwait(false);
            return models.Select(x => RestDMChannel.Create(client, x)).ToImmutableArray();
        }
        
        public static async Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync(BaseDiscordClient client)
        {
            var models = await client.ApiClient.GetMyConnectionsAsync().ConfigureAwait(false);
            return models.Select(x => RestConnection.Create(x)).ToImmutableArray();
        }
        
        public static async Task<RestInvite> GetInviteAsync(BaseDiscordClient client,
            string inviteId)
        {
            var model = await client.ApiClient.GetInviteAsync(inviteId).ConfigureAwait(false);
            if (model != null)
                return RestInvite.Create(client, null, null, model);
            return null;
        }
        
        public static async Task<RestGuild> GetGuildAsync(BaseDiscordClient client,
            ulong id)
        {
            var model = await client.ApiClient.GetGuildAsync(id).ConfigureAwait(false);
            if (model != null)
                return RestGuild.Create(client, model);
            return null;
        }
        public static async Task<RestGuildEmbed?> GetGuildEmbedAsync(BaseDiscordClient client,
            ulong id)
        {
            var model = await client.ApiClient.GetGuildEmbedAsync(id).ConfigureAwait(false);
            if (model != null)
                return RestGuildEmbed.Create(model);
            return null;
        }
        public static async Task<IReadOnlyCollection<RestUserGuild>> GetGuildSummariesAsync(BaseDiscordClient client)
        {
            var models = await client.ApiClient.GetMyGuildsAsync().ConfigureAwait(false);
            return models.Select(x => RestUserGuild.Create(client, x)).ToImmutableArray();
        }
        public static async Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(BaseDiscordClient client)
        {
            var summaryModels = await client.ApiClient.GetMyGuildsAsync().ConfigureAwait(false);
            var guilds = ImmutableArray.CreateBuilder<RestGuild>(summaryModels.Count);
            foreach (var summaryModel in summaryModels)
            {
                var guildModel = await client.ApiClient.GetGuildAsync(summaryModel.Id).ConfigureAwait(false);
                if (guildModel != null)
                    guilds.Add(RestGuild.Create(client, guildModel));
            }
            return guilds.ToImmutable();
        }
        public static async Task<RestGuild> CreateGuildAsync(BaseDiscordClient client,
            string name, IVoiceRegion region, Stream jpegIcon = null)
        {
            var args = new CreateGuildParams(name, region.Id);
            var model = await client.ApiClient.CreateGuildAsync(args).ConfigureAwait(false);
            return RestGuild.Create(client, model);
        }
        
        public static async Task<RestUser> GetUserAsync(BaseDiscordClient client,
            ulong id)
        {
            var model = await client.ApiClient.GetUserAsync(id).ConfigureAwait(false);
            if (model != null)
                return RestUser.Create(client, model);
            return null;
        }
        public static async Task<RestGuildUser> GetGuildUserAsync(BaseDiscordClient client,
            ulong guildId, ulong id)
        {
            var model = await client.ApiClient.GetGuildMemberAsync(guildId, id).ConfigureAwait(false);
            if (model != null)
                return RestGuildUser.Create(client, new RestGuild(client, guildId), model);
            return null;
        }

        public static async Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(BaseDiscordClient client)
        {
            var models = await client.ApiClient.GetVoiceRegionsAsync().ConfigureAwait(false);
            return models.Select(x => RestVoiceRegion.Create(client, x)).ToImmutableArray();
        }
        public static async Task<RestVoiceRegion> GetVoiceRegionAsync(BaseDiscordClient client,
            string id)
        {
            var models = await client.ApiClient.GetVoiceRegionsAsync().ConfigureAwait(false);
            return models.Select(x => RestVoiceRegion.Create(client, x)).Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
