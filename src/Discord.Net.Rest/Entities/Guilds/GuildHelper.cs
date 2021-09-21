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
                SystemChannelFlags = args.SystemChannelFlags
            };

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
        #endregion

        #region Bans
        public static async Task<IReadOnlyCollection<RestBan>> GetBansAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildBansAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestBan.Create(client, x)).ToImmutableArray();
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
        public static async Task<IReadOnlyCollection<RestGuildIntegration>> GetIntegrationsAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildIntegrationsAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestGuildIntegration.Create(client, guild, x)).ToImmutableArray();
        }
        public static async Task<RestGuildIntegration> CreateIntegrationAsync(IGuild guild, BaseDiscordClient client,
            ulong id, string type, RequestOptions options)
        {
            var args = new CreateGuildIntegrationParams(id, type);
            var model = await client.ApiClient.CreateGuildIntegrationAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestGuildIntegration.Create(client, guild, model);
        }
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
    }
}
