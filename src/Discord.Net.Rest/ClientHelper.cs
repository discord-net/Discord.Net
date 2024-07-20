using Discord.API;
using Discord.API.Rest;

using System;
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

        public static async Task<RestApplication> GetCurrentBotApplicationAsync(BaseDiscordClient client, RequestOptions options)
        {
            var model = await client.ApiClient.GetCurrentBotApplicationAsync(options).ConfigureAwait(false);
            return RestApplication.Create(client, model);
        }

        public static Task<API.Application> ModifyCurrentBotApplicationAsync(BaseDiscordClient client, Action<ModifyApplicationProperties> func, RequestOptions options)
        {
            var args = new ModifyApplicationProperties();
            func(args);

            if (args.Tags.IsSpecified)
            {
                Preconditions.AtMost(args.Tags.Value.Length, DiscordConfig.MaxApplicationTagCount, nameof(args.Tags), $"An application can have a maximum of {DiscordConfig.MaxApplicationTagCount} applied.");
                foreach (var tag in args.Tags.Value)
                    Preconditions.AtMost(tag.Length, DiscordConfig.MaxApplicationTagLength, nameof(args.Tags), $"An application tag must have length less or equal to {DiscordConfig.MaxApplicationTagLength}");
            }

            if (args.Description.IsSpecified)
                Preconditions.AtMost(args.Description.Value.Length, DiscordConfig.MaxApplicationDescriptionLength, nameof(args.Description), $"An application description tag mus have length less or equal to {DiscordConfig.MaxApplicationDescriptionLength}");

            return client.ApiClient.ModifyCurrentBotApplicationAsync(new()
            {
                Description = args.Description,
                Tags = args.Tags,
                Icon = args.Icon.IsSpecified ? args.Icon.Value?.ToModel() : Optional<API.Image?>.Unspecified,
                InteractionsEndpointUrl = args.InteractionsEndpointUrl,
                RoleConnectionsEndpointUrl = args.RoleConnectionsEndpointUrl,
                Flags = args.Flags,
                CoverImage = args.CoverImage.IsSpecified ? args.CoverImage.Value?.ToModel() : Optional<API.Image?>.Unspecified,
                CustomInstallUrl = args.CustomInstallUrl,
                InstallParams = args.InstallParams.IsSpecified
                    ? args.InstallParams.Value is null
                        ? null
                        : new InstallParams
                        {
                            Permission = args.InstallParams.Value.Permission,
                            Scopes = args.InstallParams.Value.Scopes.ToArray()
                        }
                    : Optional<InstallParams>.Unspecified,
                IntegrationTypesConfig = args.IntegrationTypesConfig.IsSpecified
                    ? args.IntegrationTypesConfig.Value?.ToDictionary(x => x.Key, x => new InstallParams
                    {
                        Permission = x.Value.Permission,
                        Scopes = x.Value.Scopes.ToArray()
                    })
                    : Optional<Dictionary<ApplicationIntegrationType, InstallParams>>.Unspecified
            }, options);
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
            return models.Select(model => RestConnection.Create(client, model)).ToImmutableArray();
        }

        public static async Task<RestInviteMetadata> GetInviteAsync(BaseDiscordClient client, string inviteId, RequestOptions options, ulong? scheduledEventId = null)
        {
            var model = await client.ApiClient.GetInviteAsync(inviteId, options, scheduledEventId).ConfigureAwait(false);
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

        public static async Task<IReadOnlyCollection<RestGlobalCommand>> GetGlobalApplicationCommandsAsync(BaseDiscordClient client, bool withLocalizations = false,
            string locale = null, RequestOptions options = null)
        {
            var response = await client.ApiClient.GetGlobalApplicationCommandsAsync(withLocalizations, locale, options).ConfigureAwait(false);

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

        public static async Task<IReadOnlyCollection<RestGuildCommand>> GetGuildApplicationCommandsAsync(BaseDiscordClient client, ulong guildId, bool withLocalizations = false,
            string locale = null, RequestOptions options = null)
        {
            var response = await client.ApiClient.GetGuildApplicationCommandsAsync(guildId, withLocalizations, locale, options).ConfigureAwait(false);

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

        #region Role Connection Metadata

        public static async Task<IReadOnlyCollection<RoleConnectionMetadata>> GetRoleConnectionMetadataRecordsAsync(BaseDiscordClient client, RequestOptions options = null)
            => (await client.ApiClient.GetApplicationRoleConnectionMetadataRecordsAsync(options))
                .Select(model
                    => new RoleConnectionMetadata(
                        model.Type,
                        model.Key,
                        model.Name,
                        model.Description,
                        model.NameLocalizations.IsSpecified
                            ? model.NameLocalizations.Value?.ToImmutableDictionary()
                            : null,
                        model.DescriptionLocalizations.IsSpecified
                            ? model.DescriptionLocalizations.Value?.ToImmutableDictionary()
                            : null))
                .ToImmutableArray();

        public static async Task<IReadOnlyCollection<RoleConnectionMetadata>> ModifyRoleConnectionMetadataRecordsAsync(ICollection<RoleConnectionMetadataProperties> metadata, BaseDiscordClient client, RequestOptions options = null)
            => (await client.ApiClient.UpdateApplicationRoleConnectionMetadataRecordsAsync(metadata
                .Select(x => new API.RoleConnectionMetadata
                {
                    Name = x.Name,
                    Description = x.Description,
                    Key = x.Key,
                    Type = x.Type,
                    NameLocalizations = x.NameLocalizations?.ToDictionary(),
                    DescriptionLocalizations = x.DescriptionLocalizations?.ToDictionary()
                }).ToArray()))
                .Select(model
                    => new RoleConnectionMetadata(
                        model.Type,
                        model.Key,
                        model.Name,
                        model.Description,
                        model.NameLocalizations.IsSpecified
                            ? model.NameLocalizations.Value?.ToImmutableDictionary()
                            : null,
                        model.DescriptionLocalizations.IsSpecified
                            ? model.DescriptionLocalizations.Value?.ToImmutableDictionary()
                            : null))
                .ToImmutableArray();

        public static async Task<RoleConnection> GetUserRoleConnectionAsync(ulong applicationId, BaseDiscordClient client, RequestOptions options = null)
        {
            var roleConnection = await client.ApiClient.GetUserApplicationRoleConnectionAsync(applicationId, options);

            return new RoleConnection(roleConnection.PlatformName.GetValueOrDefault(null),
                roleConnection.PlatformUsername.GetValueOrDefault(null),
                roleConnection.Metadata.GetValueOrDefault());
        }

        public static async Task<RoleConnection> ModifyUserRoleConnectionAsync(ulong applicationId, RoleConnectionProperties roleConnection, BaseDiscordClient client, RequestOptions options = null)
        {
            var updatedConnection = await client.ApiClient.ModifyUserApplicationRoleConnectionAsync(applicationId,
                new API.RoleConnection
                {
                    PlatformName = roleConnection.PlatformName,
                    PlatformUsername = roleConnection.PlatformUsername,
                    Metadata = roleConnection.Metadata
                }, options);

            return new RoleConnection(
                updatedConnection.PlatformName.GetValueOrDefault(null),
                updatedConnection.PlatformUsername.GetValueOrDefault(null),
                updatedConnection.Metadata.GetValueOrDefault()?.ToImmutableDictionary()
                );
        }


        #endregion

        #region App Subscriptions

        public static async Task<RestEntitlement> CreateTestEntitlementAsync(BaseDiscordClient client, ulong skuId, ulong ownerId, SubscriptionOwnerType ownerType,
            RequestOptions options = null)
        {
            var model = await client.ApiClient.CreateEntitlementAsync(new CreateEntitlementParams
            {
                Type = ownerType,
                OwnerId = ownerId,
                SkuId = skuId
            }, options);

            return RestEntitlement.Create(client, model);
        }

        public static IAsyncEnumerable<IReadOnlyCollection<RestEntitlement>> ListEntitlementsAsync(BaseDiscordClient client, int? limit = 100,
            ulong? afterId = null, ulong? beforeId = null, bool excludeEnded = false, ulong? guildId = null, ulong? userId = null,
             ulong[] skuIds = null, RequestOptions options = null)
        {
            return new PagedAsyncEnumerable<RestEntitlement>(
                DiscordConfig.MaxEntitlementsPerBatch,
                async (info, ct) =>
                {
                    var args = new ListEntitlementsParams()
                    {
                        Limit = info.PageSize,
                        BeforeId = beforeId ?? Optional<ulong>.Unspecified,
                        ExcludeEnded = excludeEnded,
                        GuildId = guildId ?? Optional<ulong>.Unspecified,
                        UserId = userId ?? Optional<ulong>.Unspecified,
                        SkuIds = skuIds ?? Optional<ulong[]>.Unspecified,
                    };
                    if (info.Position != null)
                        args.AfterId = info.Position.Value;
                    var models = await client.ApiClient.ListEntitlementAsync(args, options).ConfigureAwait(false);
                    return models
                        .Select(x => RestEntitlement.Create(client, x))
                        .ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxEntitlementsPerBatch)
                        return false;
                    info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                start: afterId,
                count: limit
            );
        }

        public static async Task<IReadOnlyCollection<SKU>> ListSKUsAsync(BaseDiscordClient client, RequestOptions options = null)
        {
            var models = await client.ApiClient.ListSKUsAsync(options).ConfigureAwait(false);

            return models.Select(x => new SKU(x.Id, x.Type, x.ApplicationId, x.Name, x.Slug)).ToImmutableArray();
        }

        public static Task ConsumeEntitlementAsync(BaseDiscordClient client, ulong entitlementId, RequestOptions options = null)
            => client.ApiClient.ConsumeEntitlementAsync(entitlementId, options);

        #endregion
    }
}
