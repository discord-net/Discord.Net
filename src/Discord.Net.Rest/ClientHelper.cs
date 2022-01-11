using System;
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
        #region Applications
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
        /// <exception cref="InvalidOperationException">Unexpected channel type.</exception>
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
            return models.Select(RestConnection.Create).ToImmutableArray();
        }

        public static async Task<RestInviteMetadata> GetInviteAsync(BaseDiscordClient client,
            string inviteId, RequestOptions options)
        {
            var model = await client.ApiClient.GetInviteAsync(inviteId, options).ConfigureAwait(false);
            if (model != null)
                return RestInviteMetadata.Create(client, null, null, model);
            return null;
        }

        public static async Task<RestGuild> GetGuildAsync(BaseDiscordClient client,
            ulong id, bool withCounts, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildAsync(id, withCounts, options).ConfigureAwait(false);
            if (model != null)
                return RestGuild.Create(client, model);
            return null;
        }
        public static async Task<RestGuildWidget?> GetGuildWidgetAsync(BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildWidgetAsync(id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildWidget.Create(model);
            return null;
        }
        public static IAsyncEnumerable<IReadOnlyCollection<RestUserGuild>> GetGuildSummariesAsync(BaseDiscordClient client,
            ulong? fromGuildId, int? limit, RequestOptions options)
        {
            return new PagedAsyncEnumerable<RestUserGuild>(
                DiscordConfig.MaxGuildsPerBatch,
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
        public static async Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(BaseDiscordClient client, bool withCounts, RequestOptions options)
        {
            var summaryModels = await GetGuildSummariesAsync(client, null, null, options).FlattenAsync().ConfigureAwait(false);
            var guilds = ImmutableArray.CreateBuilder<RestGuild>();
            foreach (var summaryModel in summaryModels)
            {
                var guildModel = await client.ApiClient.GetGuildAsync(summaryModel.Id, withCounts).ConfigureAwait(false);
                if (guildModel != null)
                    guilds.Add(RestGuild.Create(client, guildModel));
            }
            return guilds.ToImmutable();
        }
        public static async Task<RestGuild> CreateGuildAsync(BaseDiscordClient client,
            string name, IVoiceRegion region, Stream jpegIcon, RequestOptions options)
        {
            var args = new CreateGuildParams(name, region.Id);
            if (jpegIcon != null)
                args.Icon = new API.Image(jpegIcon);

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
            var guild = await GetGuildAsync(client, guildId, false, options).ConfigureAwait(false);
            if (guild == null)
                return null;

            var model = await client.ApiClient.GetGuildMemberAsync(guildId, id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildUser.Create(client, guild, model);

            return null;
        }

        public static async Task<RestWebhook> GetWebhookAsync(BaseDiscordClient client, ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetWebhookAsync(id).ConfigureAwait(false);
            if (model != null)
                return RestWebhook.Create(client, (IGuild)null, model);
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
            return models.Select(x => RestVoiceRegion.Create(client, x)).FirstOrDefault(x => x.Id == id);
        }

        public static async Task<int> GetRecommendShardCountAsync(BaseDiscordClient client, RequestOptions options)
        {
            var response = await client.ApiClient.GetBotGatewayAsync(options).ConfigureAwait(false);
            return response.Shards;
        }

        public static async Task<BotGateway> GetBotGatewayAsync(BaseDiscordClient client, RequestOptions options)
        {
            var response = await client.ApiClient.GetBotGatewayAsync(options).ConfigureAwait(false);
            return new BotGateway
            {
                Url = response.Url,
                Shards = response.Shards,
                SessionStartLimit = new SessionStartLimit
                {
                    Total = response.SessionStartLimit.Total,
                    Remaining = response.SessionStartLimit.Remaining,
                    ResetAfter = response.SessionStartLimit.ResetAfter,
                    MaxConcurrency = response.SessionStartLimit.MaxConcurrency
                }
            };
        }

        public static async Task<IReadOnlyCollection<RestGlobalCommand>> GetGlobalApplicationCommandsAsync(BaseDiscordClient client,
            RequestOptions options = null)
        {
            var response = await client.ApiClient.GetGlobalApplicationCommandsAsync(options).ConfigureAwait(false);

            if (!response.Any())
                return Array.Empty<RestGlobalCommand>();

            return response.Select(x => RestGlobalCommand.Create(client, x)).ToArray();
        }
        public static async Task<RestGlobalCommand> GetGlobalApplicationCommandAsync(BaseDiscordClient client, ulong id,
            RequestOptions options = null)
        {
            var model = await client.ApiClient.GetGlobalApplicationCommandAsync(id, options);

            return model != null ? RestGlobalCommand.Create(client, model) : null;
        }

        public static async Task<IReadOnlyCollection<RestGuildCommand>> GetGuildApplicationCommandsAsync(BaseDiscordClient client, ulong guildId,
            RequestOptions options = null)
        {
            var response = await client.ApiClient.GetGuildApplicationCommandsAsync(guildId, options).ConfigureAwait(false);

            if (!response.Any())
                return ImmutableArray.Create<RestGuildCommand>();

            return response.Select(x => RestGuildCommand.Create(client, x, guildId)).ToImmutableArray();
        }
        public static async Task<RestGuildCommand> GetGuildApplicationCommandAsync(BaseDiscordClient client, ulong id, ulong guildId,
            RequestOptions options = null)
        {
            var model = await client.ApiClient.GetGuildApplicationCommandAsync(guildId, id, options);

            return model != null ? RestGuildCommand.Create(client, model, guildId) : null;
        }
        public static async Task<RestGuildCommand> CreateGuildApplicationCommandAsync(BaseDiscordClient client, ulong guildId, ApplicationCommandProperties properties,
            RequestOptions options = null)
        {
            var model = await InteractionHelper.CreateGuildCommandAsync(client, guildId, properties, options);

            return RestGuildCommand.Create(client, model, guildId);
        }
        public static async Task<RestGlobalCommand> CreateGlobalApplicationCommandAsync(BaseDiscordClient client, ApplicationCommandProperties properties,
            RequestOptions options = null)
        {
            var model = await InteractionHelper.CreateGlobalCommandAsync(client, properties, options);

            return RestGlobalCommand.Create(client, model);
        }
        public static async Task<IReadOnlyCollection<RestGlobalCommand>> BulkOverwriteGlobalApplicationCommandAsync(BaseDiscordClient client, ApplicationCommandProperties[] properties,
            RequestOptions options = null)
        {
            var models = await InteractionHelper.BulkOverwriteGlobalCommandsAsync(client, properties, options);

            return models.Select(x => RestGlobalCommand.Create(client, x)).ToImmutableArray();
        }
        public static async Task<IReadOnlyCollection<RestGuildCommand>> BulkOverwriteGuildApplicationCommandAsync(BaseDiscordClient client, ulong guildId,
            ApplicationCommandProperties[] properties, RequestOptions options = null)
        {
            var models = await InteractionHelper.BulkOverwriteGuildCommandsAsync(client, guildId, properties, options);

            return models.Select(x => RestGuildCommand.Create(client, x, guildId)).ToImmutableArray();
        }

        public static Task AddRoleAsync(BaseDiscordClient client, ulong guildId, ulong userId, ulong roleId, RequestOptions options = null)
            => client.ApiClient.AddRoleAsync(guildId, userId, roleId, options);

        public static Task RemoveRoleAsync(BaseDiscordClient client, ulong guildId, ulong userId, ulong roleId, RequestOptions options = null)
            => client.ApiClient.RemoveRoleAsync(guildId, userId, roleId, options);
        #endregion
    }
}
