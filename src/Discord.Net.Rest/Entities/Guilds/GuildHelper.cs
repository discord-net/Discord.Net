using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EmbedModel = Discord.API.GuildEmbed;
using Model = Discord.API.Guild;
using RoleModel = Discord.API.Role;
using ImageModel = Discord.API.Image;

namespace Discord.Rest
{
    internal static class GuildHelper
    {
        //General
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
                VerificationLevel = args.VerificationLevel,
                ExplicitContentFilter = args.ExplicitContentFilter
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

            if (!apiArgs.Splash.IsSpecified && guild.SplashId != null)
                apiArgs.Splash = new ImageModel(guild.SplashId);
            if (!apiArgs.Icon.IsSpecified && guild.IconId != null)
                apiArgs.Icon = new ImageModel(guild.IconId);

            if (args.ExplicitContentFilter.IsSpecified)
                apiArgs.ExplicitContentFilter = args.ExplicitContentFilter.Value;

            return await client.ApiClient.ModifyGuildAsync(guild.Id, apiArgs, options).ConfigureAwait(false);
        }
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
        public static async Task<EmbedModel> ModifyEmbedAsync(IGuild guild, BaseDiscordClient client,
            Action<GuildEmbedProperties> func, RequestOptions options)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var args = new GuildEmbedProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyGuildEmbedParams
            {
                Enabled = args.Enabled
            };

            if (args.Channel.IsSpecified)
                apiArgs.ChannelId = args.Channel.Value?.Id;
            else if (args.ChannelId.IsSpecified)
                apiArgs.ChannelId = args.ChannelId.Value;

            return await client.ApiClient.ModifyGuildEmbedAsync(guild.Id, apiArgs, options).ConfigureAwait(false);
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

        //Bans
        public static async Task<IReadOnlyCollection<RestBan>> GetBansAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildBansAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestBan.Create(client, x)).ToImmutableArray();
        }
        public static async Task<RestBan> GetBanAsync(IGuild guild, BaseDiscordClient client, ulong userId, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildBanAsync(guild.Id, userId, options).ConfigureAwait(false);
            return RestBan.Create(client, model);
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

        //Channels
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
                UserLimit = props.UserLimit
            };
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestVoiceChannel.Create(client, guild, model);
        }
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public static async Task<RestCategoryChannel> CreateCategoryChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options)
        {
            if (name == null) throw new ArgumentNullException(paramName: nameof(name));

            var args = new CreateGuildChannelParams(name, ChannelType.Category);
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestCategoryChannel.Create(client, guild, model);
        }

        //Voice Regions
        public static async Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(IGuild guild, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildVoiceRegionsAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestVoiceRegion.Create(client, x)).ToImmutableArray();
        }

        //Integrations
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

        //Invites
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
            return RestInviteMetadata.Create(client, guild, null, inviteModel);
        }

        //Roles
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public static async Task<RestRole> CreateRoleAsync(IGuild guild, BaseDiscordClient client,
            string name, GuildPermissions? permissions, Color? color, bool isHoisted, RequestOptions options)
        {
            if (name == null) throw new ArgumentNullException(paramName: nameof(name));

            var model = await client.ApiClient.CreateGuildRoleAsync(guild.Id, options).ConfigureAwait(false);
            var role = RestRole.Create(client, guild, model);

            await role.ModifyAsync(x =>
            {
                x.Name = name;
                x.Permissions = (permissions ?? role.Permissions);
                x.Color = (color ?? Color.Default);
                x.Hoist = isHoisted;
            }, options).ConfigureAwait(false);

            return role;
        }

        //Users
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
                DiscordConfig.MaxMessagesPerBatch,
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
                    if (lastPage.Count != DiscordConfig.MaxMessagesPerBatch)
                        return false;
                    info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                start: fromUserId,
                count: limit
            );
        }
        public static async Task<int> PruneUsersAsync(IGuild guild, BaseDiscordClient client,
            int days, bool simulate, RequestOptions options)
        {
            var args = new GuildPruneParams(days);
            GetGuildPruneCountResponse model;
            if (simulate)
                model = await client.ApiClient.GetGuildPruneCountAsync(guild.Id, args, options).ConfigureAwait(false);
            else
                model = await client.ApiClient.BeginGuildPruneAsync(guild.Id, args, options).ConfigureAwait(false);
            return model.Pruned;
        }

        // Audit logs
        public static IAsyncEnumerable<IReadOnlyCollection<RestAuditLogEntry>> GetAuditLogsAsync(IGuild guild, BaseDiscordClient client,
            ulong? from, int? limit, RequestOptions options)
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

        //Webhooks
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

        //Emotes
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
    }
}
