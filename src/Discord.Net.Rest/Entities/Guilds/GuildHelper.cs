using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using WidgetModel = Discord.API.GuildWidget;
using Model = Discord.API.Guild;
using RoleModel = Discord.API.Role;
using ImageModel = Discord.API.Image;
using System.IO;

namespace Discord.Rest
{
    internal static class GuildHelper
    {
        #region General
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
        public static async Task<Model> ModifyAsync(IGuild guild, BaseDiscordClient client,
            Action<GuildProperties> func, RequestOptions options)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var args = new GuildProperties();
            func(args);

            var apiArgs = new API.Rest.ModifyGuildParams
            {
                AfkChannelId = args.AfkChannelId,
                AfkTimeout = args.AfkTimeout,
                SystemChannelId = args.SystemChannelId,
                DefaultMessageNotifications = args.DefaultMessageNotifications,
                Icon = args.Icon.IsSpecified ? args.Icon.Value?.ToModel() : Optional.Create<ImageModel?>(),
                Name = args.Name,
                Splash = args.Splash.IsSpecified ? args.Splash.Value?.ToModel() : Optional.Create<ImageModel?>(),
                Banner = args.Banner.IsSpecified ? args.Banner.Value?.ToModel() : Optional.Create<ImageModel?>(),
                VerificationLevel = args.VerificationLevel,
                ExplicitContentFilter = args.ExplicitContentFilter,
                SystemChannelFlags = args.SystemChannelFlags,
                IsBoostProgressBarEnabled = args.IsBoostProgressBarEnabled
            };

            if (apiArgs.Banner.IsSpecified)
                guild.Features.EnsureFeature(GuildFeature.Banner);

            if (apiArgs.Splash.IsSpecified)
                guild.Features.EnsureFeature(GuildFeature.InviteSplash);

            if (args.AfkChannel.IsSpecified)
                apiArgs.AfkChannelId = args.AfkChannel.Value.Id;
            else if (args.AfkChannelId.IsSpecified)
                apiArgs.AfkChannelId = args.AfkChannelId.Value;

            if (args.SystemChannel.IsSpecified)
                apiArgs.SystemChannelId = args.SystemChannel.Value.Id;
            else if (args.SystemChannelId.IsSpecified)
                apiArgs.SystemChannelId = args.SystemChannelId.Value;

            if (args.Owner.IsSpecified)
                apiArgs.OwnerId = args.Owner.Value.Id;
            else if (args.OwnerId.IsSpecified)
                apiArgs.OwnerId = args.OwnerId.Value;

            if (args.Region.IsSpecified)
                apiArgs.RegionId = args.Region.Value.Id;
            else if (args.RegionId.IsSpecified)
                apiArgs.RegionId = args.RegionId.Value;

            if (!apiArgs.Banner.IsSpecified && guild.BannerId != null)
                apiArgs.Banner = new ImageModel(guild.BannerId);
            if (!apiArgs.Splash.IsSpecified && guild.SplashId != null)
                apiArgs.Splash = new ImageModel(guild.SplashId);
            if (!apiArgs.Icon.IsSpecified && guild.IconId != null)
                apiArgs.Icon = new ImageModel(guild.IconId);

            if (args.ExplicitContentFilter.IsSpecified)
                apiArgs.ExplicitContentFilter = args.ExplicitContentFilter.Value;

            if (args.SystemChannelFlags.IsSpecified)
                apiArgs.SystemChannelFlags = args.SystemChannelFlags.Value;

            // PreferredLocale takes precedence over PreferredCulture
            if (args.PreferredLocale.IsSpecified)
                apiArgs.PreferredLocale = args.PreferredLocale.Value;
            else if (args.PreferredCulture.IsSpecified)
                apiArgs.PreferredLocale = args.PreferredCulture.Value.Name;

            return await client.ApiClient.ModifyGuildAsync(guild.Id, apiArgs, options).ConfigureAwait(false);
        }
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
        public static async Task<WidgetModel> ModifyWidgetAsync(IGuild guild, BaseDiscordClient client,
            Action<GuildWidgetProperties> func, RequestOptions options)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var args = new GuildWidgetProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyGuildWidgetParams
            {
                Enabled = args.Enabled
            };

            if (args.Channel.IsSpecified)
                apiArgs.ChannelId = args.Channel.Value?.Id;
            else if (args.ChannelId.IsSpecified)
                apiArgs.ChannelId = args.ChannelId.Value;

            return await client.ApiClient.ModifyGuildWidgetAsync(guild.Id, apiArgs, options).ConfigureAwait(false);
        }
        public static async Task ReorderChannelsAsync(IGuild guild, BaseDiscordClient client,
            IEnumerable<ReorderChannelProperties> args, RequestOptions options)
        {
            var apiArgs = args.Select(x => new API.Rest.ModifyGuildChannelsParams(x.Id, x.Position));
            await client.ApiClient.ModifyGuildChannelsAsync(guild.Id, apiArgs, options).ConfigureAwait(false);
        }
        public static async Task<IReadOnlyCollection<RoleModel>> ReorderRolesAsync(IGuild guild, BaseDiscordClient client,
            IEnumerable<ReorderRoleProperties> args, RequestOptions options)
        {
            var apiArgs = args.Select(x => new API.Rest.ModifyGuildRolesParams(x.Id, x.Position));
            return await client.ApiClient.ModifyGuildRolesAsync(guild.Id, apiArgs, options).ConfigureAwait(false);
        }
        public static async Task LeaveAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.LeaveGuildAsync(guild.Id, options).ConfigureAwait(false);
        }
        public static async Task DeleteAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.DeleteGuildAsync(guild.Id, options).ConfigureAwait(false);
        }
        public static ulong GetUploadLimit(IGuild guild)
        {
            var tierFactor = guild.PremiumTier switch
            {
                PremiumTier.Tier2 => 50,
                PremiumTier.Tier3 => 100,
                _ => 8
            };

            var mebibyte = Math.Pow(2, 20);
            return (ulong) (tierFactor * mebibyte);
        }
        #endregion

        #region Bans
        public static IAsyncEnumerable<IReadOnlyCollection<RestBan>> GetBansAsync(IGuild guild, BaseDiscordClient client,
            ulong? fromUserId, Direction dir, int limit, RequestOptions options)
        {
            if (dir == Direction.Around && limit > DiscordConfig.MaxBansPerBatch)
            {
                int around = limit / 2;
                if (fromUserId.HasValue)
                    return GetBansAsync(guild, client, fromUserId.Value + 1, Direction.Before, around + 1, options)
                        .Concat(GetBansAsync(guild, client, fromUserId.Value, Direction.After, around, options));
                else
                    return GetBansAsync(guild, client, null, Direction.Before, around + 1, options);
            }

            return new PagedAsyncEnumerable<RestBan>(
                DiscordConfig.MaxBansPerBatch,
                async (info, ct) =>
                {
                    var args = new GetGuildBansParams
                    {
                        RelativeDirection = dir,
                        Limit = info.PageSize
                    };
                    if (info.Position != null)
                        args.RelativeUserId = info.Position.Value;

                    var models = await client.ApiClient.GetGuildBansAsync(guild.Id, args, options).ConfigureAwait(false);
                    var builder = ImmutableArray.CreateBuilder<RestBan>();

                    foreach (var model in models)
                        builder.Add(RestBan.Create(client, model));

                    return builder.ToImmutable();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxMessagesPerBatch)
                        return false;
                    if (dir == Direction.Before)
                        info.Position = lastPage.Min(x => x.User.Id);
                    else
                        info.Position = lastPage.Max(x => x.User.Id);
                    return true;
                },
                start: fromUserId,
                count: limit
                );
        }

        public static async Task<RestBan> GetBanAsync(IGuild guild, BaseDiscordClient client, ulong userId, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildBanAsync(guild.Id, userId, options).ConfigureAwait(false);
            return model == null ? null : RestBan.Create(client, model);
        }

        public static async Task AddBanAsync(IGuild guild, BaseDiscordClient client,
            ulong userId, int pruneDays, string reason, RequestOptions options)
        {
            var args = new CreateGuildBanParams { DeleteMessageDays = pruneDays, Reason = reason };
            await client.ApiClient.CreateGuildBanAsync(guild.Id, userId, args, options).ConfigureAwait(false);
        }
        public static async Task RemoveBanAsync(IGuild guild, BaseDiscordClient client,
            ulong userId, RequestOptions options)
        {
            await client.ApiClient.RemoveGuildBanAsync(guild.Id, userId, options).ConfigureAwait(false);
        }
        #endregion

        #region Channels
        public static async Task<RestGuildChannel> GetChannelAsync(IGuild guild, BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetChannelAsync(guild.Id, id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildChannel.Create(client, guild, model);
            return null;
        }
        public static async Task<IReadOnlyCollection<RestGuildChannel>> GetChannelsAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildChannelsAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestGuildChannel.Create(client, guild, x)).ToImmutableArray();
        }
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public static async Task<RestTextChannel> CreateTextChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<TextChannelProperties> func = null)
        {
            if (name == null) throw new ArgumentNullException(paramName: nameof(name));

            var props = new TextChannelProperties();
            func?.Invoke(props);

            var args = new CreateGuildChannelParams(name, ChannelType.Text)
            {
                CategoryId = props.CategoryId,
                Topic = props.Topic,
                IsNsfw = props.IsNsfw,
                Position = props.Position,
                SlowModeInterval = props.SlowModeInterval,
                Overwrites = props.PermissionOverwrites.IsSpecified
                    ? props.PermissionOverwrites.Value.Select(overwrite => new API.Overwrite
                    {
                        TargetId = overwrite.TargetId,
                        TargetType = overwrite.TargetType,
                        Allow = overwrite.Permissions.AllowValue.ToString(),
                        Deny = overwrite.Permissions.DenyValue.ToString()
                    }).ToArray()
                    : Optional.Create<API.Overwrite[]>(),
            };
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestTextChannel.Create(client, guild, model);
        }
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public static async Task<RestVoiceChannel> CreateVoiceChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<VoiceChannelProperties> func = null)
        {
            if (name == null) throw new ArgumentNullException(paramName: nameof(name));

            var props = new VoiceChannelProperties();
            func?.Invoke(props);

            var args = new CreateGuildChannelParams(name, ChannelType.Voice)
            {
                CategoryId = props.CategoryId,
                Bitrate = props.Bitrate,
                UserLimit = props.UserLimit,
                Position = props.Position,
                Overwrites = props.PermissionOverwrites.IsSpecified
                    ? props.PermissionOverwrites.Value.Select(overwrite => new API.Overwrite
                    {
                        TargetId = overwrite.TargetId,
                        TargetType = overwrite.TargetType,
                        Allow = overwrite.Permissions.AllowValue.ToString(),
                        Deny = overwrite.Permissions.DenyValue.ToString()
                    }).ToArray()
                    : Optional.Create<API.Overwrite[]>(),
            };
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestVoiceChannel.Create(client, guild, model);
        }
        public static async Task<RestStageChannel> CreateStageChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<VoiceChannelProperties> func = null)
        {
            if (name == null)
                throw new ArgumentNullException(paramName: nameof(name));

            var props = new VoiceChannelProperties();
            func?.Invoke(props);

            var args = new CreateGuildChannelParams(name, ChannelType.Stage)
            {
                CategoryId = props.CategoryId,
                Bitrate = props.Bitrate,
                UserLimit = props.UserLimit,
                Position = props.Position,
                Overwrites = props.PermissionOverwrites.IsSpecified
                    ? props.PermissionOverwrites.Value.Select(overwrite => new API.Overwrite
                    {
                        TargetId = overwrite.TargetId,
                        TargetType = overwrite.TargetType,
                        Allow = overwrite.Permissions.AllowValue.ToString(),
                        Deny = overwrite.Permissions.DenyValue.ToString()
                    }).ToArray()
                    : Optional.Create<API.Overwrite[]>(),
            };
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestStageChannel.Create(client, guild, model);
        }
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public static async Task<RestCategoryChannel> CreateCategoryChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<GuildChannelProperties> func = null)
        {
            if (name == null) throw new ArgumentNullException(paramName: nameof(name));

            var props = new GuildChannelProperties();
            func?.Invoke(props);

            var args = new CreateGuildChannelParams(name, ChannelType.Category)
            {
                Position = props.Position,
                Overwrites = props.PermissionOverwrites.IsSpecified
                    ? props.PermissionOverwrites.Value.Select(overwrite => new API.Overwrite
                    {
                        TargetId = overwrite.TargetId,
                        TargetType = overwrite.TargetType,
                        Allow = overwrite.Permissions.AllowValue.ToString(),
                        Deny = overwrite.Permissions.DenyValue.ToString()
                    }).ToArray()
                    : Optional.Create<API.Overwrite[]>(),
            };

            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestCategoryChannel.Create(client, guild, model);
        }
        #endregion

        #region Voice Regions
        public static async Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildVoiceRegionsAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestVoiceRegion.Create(client, x)).ToImmutableArray();
        }
        #endregion

        #region Integrations
        public static async Task<IReadOnlyCollection<RestIntegration>> GetIntegrationsAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetIntegrationsAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestIntegration.Create(client, guild, x)).ToImmutableArray();
        }
        public static async Task DeleteIntegrationAsync(IGuild guild, BaseDiscordClient client, ulong id,
            RequestOptions options) =>
                await client.ApiClient.DeleteIntegrationAsync(guild.Id, id, options).ConfigureAwait(false);
        #endregion

        #region Interactions
        public static async Task<IReadOnlyCollection<RestGuildCommand>> GetSlashCommandsAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildApplicationCommandsAsync(guild.Id, options);
            return models.Select(x => RestGuildCommand.Create(client, x, guild.Id)).ToImmutableArray();
        }
        public static async Task<RestGuildCommand> GetSlashCommandAsync(IGuild guild, ulong id, BaseDiscordClient client,
            RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildApplicationCommandAsync(guild.Id, id, options);
            return RestGuildCommand.Create(client, model, guild.Id);
        }
        #endregion

        #region Invites
        public static async Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildInvitesAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestInviteMetadata.Create(client, guild, null, x)).ToImmutableArray();
        }
        public static async Task<RestInviteMetadata> GetVanityInviteAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var vanityModel = await client.ApiClient.GetVanityInviteAsync(guild.Id, options).ConfigureAwait(false);
            if (vanityModel == null) throw new InvalidOperationException("This guild does not have a vanity URL.");
            var inviteModel = await client.ApiClient.GetInviteAsync(vanityModel.Code, options).ConfigureAwait(false);
            inviteModel.Uses = vanityModel.Uses;
            return RestInviteMetadata.Create(client, guild, null, inviteModel);
        }
        #endregion

        #region Roles
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public static async Task<RestRole> CreateRoleAsync(IGuild guild, BaseDiscordClient client,
            string name, GuildPermissions? permissions, Color? color, bool isHoisted, bool isMentionable, RequestOptions options)
        {
            if (name == null) throw new ArgumentNullException(paramName: nameof(name));

            var createGuildRoleParams = new API.Rest.ModifyGuildRoleParams
            {
                Color = color?.RawValue ?? Optional.Create<uint>(),
                Hoist = isHoisted,
                Mentionable = isMentionable,
                Name = name,
                Permissions = permissions?.RawValue.ToString() ?? Optional.Create<string>()
            };

            var model = await client.ApiClient.CreateGuildRoleAsync(guild.Id, createGuildRoleParams, options).ConfigureAwait(false);

            return RestRole.Create(client, guild, model);
        }
        #endregion

        #region Users
        public static async Task<RestGuildUser> AddGuildUserAsync(IGuild guild, BaseDiscordClient client, ulong userId, string accessToken,
            Action<AddGuildUserProperties> func, RequestOptions options)
        {
            var args = new AddGuildUserProperties();
            func?.Invoke(args);

            if (args.Roles.IsSpecified)
            {
                var ids = args.Roles.Value.Select(r => r.Id);

                if (args.RoleIds.IsSpecified)
                    args.RoleIds.Value.Concat(ids);
                else
                    args.RoleIds = Optional.Create(ids);
            }
            var apiArgs = new AddGuildMemberParams
            {
                AccessToken = accessToken,
                Nickname = args.Nickname,
                IsDeafened = args.Deaf,
                IsMuted = args.Mute,
                RoleIds = args.RoleIds.IsSpecified ? args.RoleIds.Value.Distinct().ToArray() : Optional.Create<ulong[]>()
            };

            var model = await client.ApiClient.AddGuildMemberAsync(guild.Id, userId, apiArgs, options);

            return model is null ? null : RestGuildUser.Create(client, guild, model);
        }

        public static async Task AddGuildUserAsync(ulong guildId, BaseDiscordClient client, ulong userId, string accessToken,
            Action<AddGuildUserProperties> func, RequestOptions options)
        {
            var args = new AddGuildUserProperties();
            func?.Invoke(args);

            if (args.Roles.IsSpecified)
            {
                var ids = args.Roles.Value.Select(r => r.Id);

                if (args.RoleIds.IsSpecified)
                    args.RoleIds.Value.Concat(ids);
                else
                    args.RoleIds = Optional.Create(ids);
            }
            var apiArgs = new AddGuildMemberParams
            {
                AccessToken = accessToken,
                Nickname = args.Nickname,
                IsDeafened = args.Deaf,
                IsMuted = args.Mute,
                RoleIds = args.RoleIds.IsSpecified ? args.RoleIds.Value.Distinct().ToArray() : Optional.Create<ulong[]>()
            };

            await client.ApiClient.AddGuildMemberAsync(guildId, userId, apiArgs, options);
        }

        public static async Task<RestGuildUser> GetUserAsync(IGuild guild, BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildMemberAsync(guild.Id, id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildUser.Create(client, guild, model);
            return null;
        }
        public static async Task<RestGuildUser> GetCurrentUserAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            return await GetUserAsync(guild, client, client.CurrentUser.Id, options).ConfigureAwait(false);
        }
        public static IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(IGuild guild, BaseDiscordClient client,
            ulong? fromUserId, int? limit, RequestOptions options)
        {
            return new PagedAsyncEnumerable<RestGuildUser>(
                DiscordConfig.MaxUsersPerBatch,
                async (info, ct) =>
                {
                    var args = new GetGuildMembersParams
                    {
                        Limit = info.PageSize
                    };
                    if (info.Position != null)
                        args.AfterUserId = info.Position.Value;
                    var models = await client.ApiClient.GetGuildMembersAsync(guild.Id, args, options).ConfigureAwait(false);
                    return models.Select(x => RestGuildUser.Create(client, guild, x)).ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxUsersPerBatch)
                        return false;
                    info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                start: fromUserId,
                count: limit
            );
        }
        public static async Task<int> PruneUsersAsync(IGuild guild, BaseDiscordClient client,
            int days, bool simulate, RequestOptions options, IEnumerable<ulong> includeRoleIds)
        {
            var args = new GuildPruneParams(days, includeRoleIds?.ToArray());
            GetGuildPruneCountResponse model;
            if (simulate)
                model = await client.ApiClient.GetGuildPruneCountAsync(guild.Id, args, options).ConfigureAwait(false);
            else
                model = await client.ApiClient.BeginGuildPruneAsync(guild.Id, args, options).ConfigureAwait(false);
            return model.Pruned;
        }
        public static async Task<IReadOnlyCollection<RestGuildUser>> SearchUsersAsync(IGuild guild, BaseDiscordClient client,
            string query, int? limit, RequestOptions options)
        {
            var apiArgs = new SearchGuildMembersParams
            {
                Query = query,
                Limit = limit ?? Optional.Create<int>()
            };
            var models = await client.ApiClient.SearchGuildMembersAsync(guild.Id, apiArgs, options).ConfigureAwait(false);
            return models.Select(x => RestGuildUser.Create(client, guild, x)).ToImmutableArray();
        }
        #endregion

        #region Audit logs
        public static IAsyncEnumerable<IReadOnlyCollection<RestAuditLogEntry>> GetAuditLogsAsync(IGuild guild, BaseDiscordClient client,
            ulong? from, int? limit, RequestOptions options, ulong? userId = null, ActionType? actionType = null)
        {
            return new PagedAsyncEnumerable<RestAuditLogEntry>(
                DiscordConfig.MaxAuditLogEntriesPerBatch,
                async (info, ct) =>
                {
                    var args = new GetAuditLogsParams
                    {
                        Limit = info.PageSize
                    };
                    if (info.Position != null)
                        args.BeforeEntryId = info.Position.Value;
                    if (userId.HasValue)
                        args.UserId = userId.Value;
                    if (actionType.HasValue)
                        args.ActionType = (int)actionType.Value;
                    var model = await client.ApiClient.GetAuditLogsAsync(guild.Id, args, options);
                    return model.Entries.Select((x) => RestAuditLogEntry.Create(client, model, x)).ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxAuditLogEntriesPerBatch)
                        return false;
                    info.Position = lastPage.Min(x => x.Id);
                    return true;
                },
                start: from,
                count: limit
            );
        }
        #endregion

        #region Webhooks
        public static async Task<RestWebhook> GetWebhookAsync(IGuild guild, BaseDiscordClient client, ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetWebhookAsync(id, options: options).ConfigureAwait(false);
            if (model == null)
                return null;
            return RestWebhook.Create(client, guild, model);
        }
        public static async Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(IGuild guild, BaseDiscordClient client, RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildWebhooksAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestWebhook.Create(client, guild, x)).ToImmutableArray();
        }
        #endregion

        #region Emotes
        public static async Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(IGuild guild, BaseDiscordClient client, RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildEmotesAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => x.ToEntity()).ToImmutableArray();
        }
        public static async Task<GuildEmote> GetEmoteAsync(IGuild guild, BaseDiscordClient client, ulong id, RequestOptions options)
        {
            var emote = await client.ApiClient.GetGuildEmoteAsync(guild.Id, id, options).ConfigureAwait(false);
            return emote.ToEntity();
        }
        public static async Task<GuildEmote> CreateEmoteAsync(IGuild guild, BaseDiscordClient client, string name, Image image, Optional<IEnumerable<IRole>> roles,
            RequestOptions options)
        {
            var apiargs = new CreateGuildEmoteParams
            {
                Name = name,
                Image = image.ToModel()
            };
            if (roles.IsSpecified)
                apiargs.RoleIds = roles.Value?.Select(xr => xr.Id).ToArray();

            var emote = await client.ApiClient.CreateGuildEmoteAsync(guild.Id, apiargs, options).ConfigureAwait(false);
            return emote.ToEntity();
        }
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
        public static async Task<GuildEmote> ModifyEmoteAsync(IGuild guild, BaseDiscordClient client, ulong id, Action<EmoteProperties> func,
            RequestOptions options)
        {
            if (func == null) throw new ArgumentNullException(paramName: nameof(func));

            var props = new EmoteProperties();
            func(props);

            var apiargs = new ModifyGuildEmoteParams
            {
                Name = props.Name
            };
            if (props.Roles.IsSpecified)
                apiargs.RoleIds = props.Roles.Value?.Select(xr => xr.Id).ToArray();

            var emote = await client.ApiClient.ModifyGuildEmoteAsync(guild.Id, id, apiargs, options).ConfigureAwait(false);
            return emote.ToEntity();
        }
        public static Task DeleteEmoteAsync(IGuild guild, BaseDiscordClient client, ulong id, RequestOptions options)
            => client.ApiClient.DeleteGuildEmoteAsync(guild.Id, id, options);

        public static async Task<API.Sticker> CreateStickerAsync(BaseDiscordClient client, IGuild guild, string name, string description, IEnumerable<string> tags,
            Image image, RequestOptions options = null)
        {
            Preconditions.NotNull(name, nameof(name));
            Preconditions.NotNull(description, nameof(description));

            Preconditions.AtLeast(name.Length, 2, nameof(name));
            Preconditions.AtLeast(description.Length, 2, nameof(description));

            Preconditions.AtMost(name.Length, 30, nameof(name));
            Preconditions.AtMost(description.Length, 100, nameof(name));

            var apiArgs = new CreateStickerParams()
            {
                Name = name,
                Description = description,
                File = image.Stream,
                Tags = string.Join(", ", tags)
            };

            return await client.ApiClient.CreateGuildStickerAsync(apiArgs, guild.Id, options).ConfigureAwait(false);
        }

        public static async Task<API.Sticker> CreateStickerAsync(BaseDiscordClient client, IGuild guild, string name, string description, IEnumerable<string> tags,
            Stream file, string filename, RequestOptions options = null)
        {
            Preconditions.NotNull(name, nameof(name));
            Preconditions.NotNull(description, nameof(description));
            Preconditions.NotNull(file, nameof(file));
            Preconditions.NotNull(filename, nameof(filename));

            Preconditions.AtLeast(name.Length, 2, nameof(name));
            Preconditions.AtLeast(description.Length, 2, nameof(description));

            Preconditions.AtMost(name.Length, 30, nameof(name));
            Preconditions.AtMost(description.Length, 100, nameof(name));

            var apiArgs = new CreateStickerParams()
            {
                Name = name,
                Description = description,
                File = file,
                Tags = string.Join(", ", tags),
                FileName = filename
            };

            return await client.ApiClient.CreateGuildStickerAsync(apiArgs, guild.Id, options).ConfigureAwait(false);
        }

        public static async Task<API.Sticker> ModifyStickerAsync(BaseDiscordClient client, ulong guildId, ISticker sticker, Action<StickerProperties> func,
            RequestOptions options = null)
        {
            if (func == null)
                throw new ArgumentNullException(paramName: nameof(func));

            var props = new StickerProperties();
            func(props);

            var apiArgs = new ModifyStickerParams()
            {
                Description = props.Description,
                Name = props.Name,
                Tags = props.Tags.IsSpecified ?
                    string.Join(", ", props.Tags.Value) :
                    Optional<string>.Unspecified
            };

            return await client.ApiClient.ModifyStickerAsync(apiArgs, guildId, sticker.Id, options).ConfigureAwait(false);
        }

        public static async Task DeleteStickerAsync(BaseDiscordClient client, ulong guildId, ISticker sticker, RequestOptions options = null)
            => await client.ApiClient.DeleteStickerAsync(guildId, sticker.Id, options).ConfigureAwait(false);
        #endregion

        #region Events

        public static async Task<IReadOnlyCollection<RestUser>> GetEventUsersAsync(BaseDiscordClient client, IGuildScheduledEvent guildEvent, int limit = 100, RequestOptions options = null)
        {
            var models = await client.ApiClient.GetGuildScheduledEventUsersAsync(guildEvent.Id, guildEvent.Guild.Id, limit, options).ConfigureAwait(false);

            return models.Select(x => RestUser.Create(client, guildEvent.Guild, x)).ToImmutableArray();
        }

        public static IAsyncEnumerable<IReadOnlyCollection<RestUser>> GetEventUsersAsync(BaseDiscordClient client, IGuildScheduledEvent guildEvent,
           ulong? fromUserId, int? limit, RequestOptions options)
        {
            return new PagedAsyncEnumerable<RestUser>(
                DiscordConfig.MaxGuildEventUsersPerBatch,
                async (info, ct) =>
                {
                    var args = new GetEventUsersParams
                    {
                        Limit = info.PageSize,
                        RelativeDirection = Direction.After,
                    };
                    if (info.Position != null)
                        args.RelativeUserId = info.Position.Value;
                    var models = await client.ApiClient.GetGuildScheduledEventUsersAsync(guildEvent.Id, guildEvent.Guild.Id, args, options).ConfigureAwait(false);
                    return models
                        .Select(x => RestUser.Create(client, guildEvent.Guild, x))
                        .ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxGuildEventUsersPerBatch)
                        return false;
                    info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                start: fromUserId,
                count: limit
            );
        }

        public static IAsyncEnumerable<IReadOnlyCollection<RestUser>> GetEventUsersAsync(BaseDiscordClient client, IGuildScheduledEvent guildEvent,
            ulong? fromUserId, Direction dir, int limit, RequestOptions options = null)
        {
            if (dir == Direction.Around && limit > DiscordConfig.MaxMessagesPerBatch)
            {
                int around = limit / 2;
                if (fromUserId.HasValue)
                    return GetEventUsersAsync(client, guildEvent, fromUserId.Value + 1, Direction.Before, around + 1, options) //Need to include the message itself
                        .Concat(GetEventUsersAsync(client, guildEvent, fromUserId, Direction.After, around, options));
                else //Shouldn't happen since there's no public overload for ulong? and Direction
                    return GetEventUsersAsync(client, guildEvent, null, Direction.Before, around + 1, options);
            }

            return new PagedAsyncEnumerable<RestUser>(
                DiscordConfig.MaxGuildEventUsersPerBatch,
                async (info, ct) =>
                {
                    var args = new GetEventUsersParams
                    {
                        RelativeDirection = dir,
                        Limit = info.PageSize
                    };
                    if (info.Position != null)
                        args.RelativeUserId = info.Position.Value;

                    var models = await client.ApiClient.GetGuildScheduledEventUsersAsync(guildEvent.Id, guildEvent.Guild.Id, args, options).ConfigureAwait(false);
                    var builder = ImmutableArray.CreateBuilder<RestUser>();
                    foreach (var model in models)
                    {
                        builder.Add(RestUser.Create(client, guildEvent.Guild, model));
                    }
                    return builder.ToImmutable();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxGuildEventUsersPerBatch)
                        return false;
                    if (dir == Direction.Before)
                        info.Position = lastPage.Min(x => x.Id);
                    else
                        info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                start: fromUserId,
                count: limit
            );
        }

        public static async Task<API.GuildScheduledEvent> ModifyGuildEventAsync(BaseDiscordClient client, Action<GuildScheduledEventsProperties> func,
            IGuildScheduledEvent guildEvent, RequestOptions options = null)
        {
            var args = new GuildScheduledEventsProperties();

            func(args);

            if (args.Status.IsSpecified)
            {
                switch (args.Status.Value)
                {
                    case GuildScheduledEventStatus.Active    when guildEvent.Status != GuildScheduledEventStatus.Scheduled:
                    case GuildScheduledEventStatus.Completed when guildEvent.Status != GuildScheduledEventStatus.Active:
                    case GuildScheduledEventStatus.Cancelled when guildEvent.Status != GuildScheduledEventStatus.Scheduled:
                        throw new ArgumentException($"Cannot set event to {args.Status.Value} when events status is {guildEvent.Status}");
                }
            }

            if (args.Type.IsSpecified)
            {
                // taken from https://discord.com/developers/docs/resources/guild-scheduled-event#modify-guild-scheduled-event
                switch (args.Type.Value)
                {
                    case GuildScheduledEventType.External:
                        if (!args.Location.IsSpecified)
                            throw new ArgumentException("Location must be specified for external events.");
                        if (!args.EndTime.IsSpecified)
                            throw new ArgumentException("End time must be specified for external events.");
                        if (!args.ChannelId.IsSpecified)
                            throw new ArgumentException("Channel id must be set to null!");
                        if (args.ChannelId.Value != null)
                            throw new ArgumentException("Channel id must be set to null!");
                        break;
                }
            }

            var apiArgs = new ModifyGuildScheduledEventParams()
            {
                ChannelId = args.ChannelId,
                Description = args.Description,
                EndTime = args.EndTime,
                Name = args.Name,
                PrivacyLevel = args.PrivacyLevel,
                StartTime = args.StartTime,
                Status = args.Status,
                Type = args.Type,
                Image = args.CoverImage.IsSpecified
                    ? args.CoverImage.Value.HasValue
                        ? args.CoverImage.Value.Value.ToModel()
                        : null
                    : Optional<ImageModel?>.Unspecified
            };

            if(args.Location.IsSpecified)
            {
                apiArgs.EntityMetadata = new API.GuildScheduledEventEntityMetadata()
                {
                    Location = args.Location,
                };
            }

            return await client.ApiClient.ModifyGuildScheduledEventAsync(apiArgs, guildEvent.Id, guildEvent.Guild.Id, options).ConfigureAwait(false);
        }

        public static async Task<RestGuildEvent> GetGuildEventAsync(BaseDiscordClient client, ulong id, IGuild guild, RequestOptions options = null)
        {
            var model = await client.ApiClient.GetGuildScheduledEventAsync(id, guild.Id, options).ConfigureAwait(false);

            if (model == null)
                return null;

            return RestGuildEvent.Create(client, guild, model);
        }

        public static async Task<IReadOnlyCollection<RestGuildEvent>> GetGuildEventsAsync(BaseDiscordClient client, IGuild guild, RequestOptions options = null)
        {
            var models = await client.ApiClient.ListGuildScheduledEventsAsync(guild.Id, options).ConfigureAwait(false);

            return models.Select(x => RestGuildEvent.Create(client, guild, x)).ToImmutableArray();
        }

        public static async Task<RestGuildEvent> CreateGuildEventAsync(BaseDiscordClient client, IGuild guild,
            string name,
            GuildScheduledEventPrivacyLevel privacyLevel,
            DateTimeOffset startTime,
            GuildScheduledEventType type,
            string description = null,
            DateTimeOffset? endTime = null,
            ulong? channelId = null,
            string location = null,
            Image? bannerImage = null,
            RequestOptions options = null)
        {
            if(location != null)
            {
                Preconditions.AtMost(location.Length, 100, nameof(location));
            }

            switch (type)
            {
                case GuildScheduledEventType.Stage or GuildScheduledEventType.Voice when channelId == null:
                    throw new ArgumentException($"{nameof(channelId)} must not be null when type is {type}", nameof(channelId));
                case GuildScheduledEventType.External when channelId != null:
                    throw new ArgumentException($"{nameof(channelId)} must be null when using external event type", nameof(channelId));
                case GuildScheduledEventType.External when location == null:
                    throw new ArgumentException($"{nameof(location)} must not be null when using external event type", nameof(location));
                case GuildScheduledEventType.External when endTime == null:
                    throw new ArgumentException($"{nameof(endTime)} must not be null when using external event type", nameof(endTime));
            }

            if (startTime <= DateTimeOffset.Now)
                throw new ArgumentOutOfRangeException(nameof(startTime), "The start time for an event cannot be in the past");

            if (endTime != null && endTime <= startTime)
                throw new ArgumentOutOfRangeException(nameof(endTime), $"{nameof(endTime)} cannot be before the start time");


            var apiArgs = new CreateGuildScheduledEventParams()
            {
                ChannelId = channelId ?? Optional<ulong>.Unspecified,
                Description = description ?? Optional<string>.Unspecified,
                EndTime = endTime ?? Optional<DateTimeOffset>.Unspecified,
                Name = name,
                PrivacyLevel = privacyLevel,
                StartTime = startTime,
                Type = type,
                Image = bannerImage.HasValue ? bannerImage.Value.ToModel() : Optional<ImageModel>.Unspecified
            };

            if(location != null)
            {
                apiArgs.EntityMetadata = new API.GuildScheduledEventEntityMetadata()
                {
                    Location = location
                };
            }

            var model = await client.ApiClient.CreateGuildScheduledEventAsync(apiArgs, guild.Id, options).ConfigureAwait(false);

            return RestGuildEvent.Create(client, guild, client.CurrentUser, model);
        }

        public static async Task DeleteEventAsync(BaseDiscordClient client, IGuildScheduledEvent guildEvent, RequestOptions options = null)
        {
            await client.ApiClient.DeleteGuildScheduledEventAsync(guildEvent.Id, guildEvent.Guild.Id, options).ConfigureAwait(false);
        }

        #endregion
    }
}
