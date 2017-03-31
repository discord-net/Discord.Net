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
        public static async Task<RestApplication> GetApplicationInfoAsync(BaseDiscordClient client, RequestOptions options)
        {
            var model = await client.ApiClient.GetMyApplicationAsync(options).ConfigureAwait(false);
            return RestApplication.Create(client, model);
        }

        public static async Task<RestChannel> GetChannelAsync(BaseDiscordClient client, 
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetChannelAsync(id, options).ConfigureAwait(false);
            if (model != null)
                return RestChannel.Create(client, model);
            return null;
        }
        public static async Task<IReadOnlyCollection<IRestPrivateChannel>> GetPrivateChannelsAsync(BaseDiscordClient client, RequestOptions options)
        {
            var models = await client.ApiClient.GetMyPrivateChannelsAsync(options).ConfigureAwait(false);
            return models.Select(x => RestChannel.CreatePrivate(client, x)).ToImmutableArray();
        }
        public static async Task<IReadOnlyCollection<RestDMChannel>> GetDMChannelsAsync(BaseDiscordClient client, RequestOptions options)
        {
            var models = await client.ApiClient.GetMyPrivateChannelsAsync(options).ConfigureAwait(false);
            return models
                .Where(x => x.Type == ChannelType.DM)
                .Select(x => RestDMChannel.Create(client, x)).ToImmutableArray();
        }
        public static async Task<IReadOnlyCollection<RestGroupChannel>> GetGroupChannelsAsync(BaseDiscordClient client, RequestOptions options)
        {
            var models = await client.ApiClient.GetMyPrivateChannelsAsync(options).ConfigureAwait(false);
            return models
                .Where(x => x.Type == ChannelType.Group)
                .Select(x => RestGroupChannel.Create(client, x)).ToImmutableArray();
        }
        
        public static async Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync(BaseDiscordClient client, RequestOptions options)
        {
            var models = await client.ApiClient.GetMyConnectionsAsync(options).ConfigureAwait(false);
            return models.Select(x => RestConnection.Create(x)).ToImmutableArray();
        }
        
        public static async Task<RestInvite> GetInviteAsync(BaseDiscordClient client,
            string inviteId, RequestOptions options)
        {
            var model = await client.ApiClient.GetInviteAsync(inviteId, options).ConfigureAwait(false);
            if (model != null)
                return RestInvite.Create(client, null, null, model);
            return null;
        }
        
        public static async Task<RestGuild> GetGuildAsync(BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildAsync(id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuild.Create(client, model);
            return null;
        }
        public static async Task<RestGuildEmbed?> GetGuildEmbedAsync(BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildEmbedAsync(id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildEmbed.Create(model);
            return null;
        }
        public static IAsyncEnumerable<IReadOnlyCollection<RestUserGuild>> GetGuildSummariesAsync(BaseDiscordClient client, 
            ulong? fromGuildId, int? limit, RequestOptions options)
        {
            return new PagedAsyncEnumerable<RestUserGuild>(
                DiscordConfig.MaxUsersPerBatch,
                async (info, ct) =>
                {
                    var args = new GetGuildSummariesParams
                    {
                        Limit = info.PageSize
                    };
                    if (info.Position != null)
                        args.AfterGuildId = info.Position.Value;
                    var models = await client.ApiClient.GetMyGuildsAsync(args, options).ConfigureAwait(false);
                    return models
                        .Select(x => RestUserGuild.Create(client, x))
                        .ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxMessagesPerBatch)
                        return false;
                    info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                start: fromGuildId,
                count: limit
            );
        }
        public static async Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(BaseDiscordClient client, RequestOptions options)
        {
            var summaryModels = await GetGuildSummariesAsync(client, null, null, options).Flatten();
            var guilds = ImmutableArray.CreateBuilder<RestGuild>();
            foreach (var summaryModel in summaryModels)
            {
                var guildModel = await client.ApiClient.GetGuildAsync(summaryModel.Id).ConfigureAwait(false);
                if (guildModel != null)
                    guilds.Add(RestGuild.Create(client, guildModel));
            }
            return guilds.ToImmutable();
        }
        public static async Task<RestGuild> CreateGuildAsync(BaseDiscordClient client,
            string name, IVoiceRegion region, Stream jpegIcon, RequestOptions options)
        {
            var args = new CreateGuildParams(name, region.Id);
            var model = await client.ApiClient.CreateGuildAsync(args, options).ConfigureAwait(false);
            return RestGuild.Create(client, model);
        }
        
        public static async Task<RestUser> GetUserAsync(BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetUserAsync(id, options).ConfigureAwait(false);
            if (model != null)
                return RestUser.Create(client, model);
            return null;
        }
        public static async Task<RestGuildUser> GetGuildUserAsync(BaseDiscordClient client,
            ulong guildId, ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildMemberAsync(guildId, id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildUser.Create(client, new RestGuild(client, guildId), model);
            return null;
        }

        public static async Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(BaseDiscordClient client, RequestOptions options)
        {
            var models = await client.ApiClient.GetVoiceRegionsAsync(options).ConfigureAwait(false);
            return models.Select(x => RestVoiceRegion.Create(client, x)).ToImmutableArray();
        }
        public static async Task<RestVoiceRegion> GetVoiceRegionAsync(BaseDiscordClient client,
            string id, RequestOptions options)
        {
            var models = await client.ApiClient.GetVoiceRegionsAsync(options).ConfigureAwait(false);
            return models.Select(x => RestVoiceRegion.Create(client, x)).Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
