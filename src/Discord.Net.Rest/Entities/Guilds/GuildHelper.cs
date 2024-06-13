using Discord.API;
using Discord.API.Rest;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ImageModel = Discord.API.Image;
using Model = Discord.API.Guild;
using RoleModel = Discord.API.Role;
using WidgetModel = Discord.API.GuildWidget;

namespace Discord.Rest
{
    internal static class GuildHelper
    {
        #region General
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null" />.</exception>
        public static Task<Model> ModifyAsync(IGuild guild, BaseDiscordClient client,
            Action<GuildProperties> func, RequestOptions options)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

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
                IsBoostProgressBarEnabled = args.IsBoostProgressBarEnabled,
                GuildFeatures = args.Features.IsSpecified ? new GuildFeatures(args.Features.Value, Array.Empty<string>()) : Optional.Create<GuildFeatures>(),
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

            return client.ApiClient.ModifyGuildAsync(guild.Id, apiArgs, options);
        }

        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null" />.</exception>
        public static Task<WidgetModel> ModifyWidgetAsync(IGuild guild, BaseDiscordClient client,
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

            return client.ApiClient.ModifyGuildWidgetAsync(guild.Id, apiArgs, options);
        }

        public static Task ReorderChannelsAsync(IGuild guild, BaseDiscordClient client,
            IEnumerable<ReorderChannelProperties> args, RequestOptions options)
        {
            var apiArgs = args.Select(x => new API.Rest.ModifyGuildChannelsParams(x.Id, x.Position));
            return client.ApiClient.ModifyGuildChannelsAsync(guild.Id, apiArgs, options);
        }

        public static Task<IReadOnlyCollection<RoleModel>> ReorderRolesAsync(IGuild guild, BaseDiscordClient client,
            IEnumerable<ReorderRoleProperties> args, RequestOptions options)
        {
            var apiArgs = args.Select(x => new API.Rest.ModifyGuildRolesParams(x.Id, x.Position));
            return client.ApiClient.ModifyGuildRolesAsync(guild.Id, apiArgs, options);
        }

        public static Task LeaveAsync(IGuild guild, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.LeaveGuildAsync(guild.Id, options);

        public static Task DeleteAsync(IGuild guild, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.DeleteGuildAsync(guild.Id, options);

        public static int GetMaxBitrate(PremiumTier premiumTier)
        {
            return premiumTier switch
            {
                PremiumTier.Tier1 => 128000,
                PremiumTier.Tier2 => 256000,
                PremiumTier.Tier3 => 384000,
                _ => 96000,
            };
        }

        public static ulong GetUploadLimit(PremiumTier premiumTier)
        {
            ulong tierFactor = premiumTier switch
            {
                PremiumTier.Tier2 => 50,
                PremiumTier.Tier3 => 100,
                _ => 25
            };

            // 1 << 20 = 2 pow 20
            var mebibyte = 1UL << 20;
            return tierFactor * mebibyte;
        }

        public static async Task<GuildIncidentsData> ModifyGuildIncidentActionsAsync(IGuild guild, BaseDiscordClient client, Action<GuildIncidentsDataProperties> func, RequestOptions options = null)
        {
            var props = new GuildIncidentsDataProperties();
            func(props);

            var args = props.DmsDisabledUntil.IsSpecified || props.InvitesDisabledUntil.IsSpecified
                ? new ModifyGuildIncidentsDataParams { DmsDisabledUntil = props.DmsDisabledUntil, InvitesDisabledUntil = props.InvitesDisabledUntil }
                : null;

            var model = await client.ApiClient.ModifyGuildIncidentActionsAsync(guild.Id, args, options);

            return new GuildIncidentsData
            {
                DmsDisabledUntil = model.DmsDisabledUntil,
                InvitesDisabledUntil = model.InvitesDisabledUntil
            };
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
                    if (lastPage.Count != DiscordConfig.MaxBansPerBatch)
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

        public static Task AddBanAsync(IGuild guild, BaseDiscordClient client, ulong userId, int pruneDays, string reason, RequestOptions options)
        {
            Preconditions.AtLeast(pruneDays, 0, nameof(pruneDays), "Prune length must be within [0, 7]");
            return client.ApiClient.CreateGuildBanAsync(guild.Id, userId, (uint)pruneDays * 86400, reason, options);
        }

        public static Task AddBanAsync(IGuild guild, BaseDiscordClient client, ulong userId, uint pruneSeconds, RequestOptions options)
        {
            return client.ApiClient.CreateGuildBanAsync(guild.Id, userId, pruneSeconds, null, options);
        }

        public static Task RemoveBanAsync(IGuild guild, BaseDiscordClient client, ulong userId, RequestOptions options)
            => client.ApiClient.RemoveGuildBanAsync(guild.Id, userId, options);

        public static async Task<BulkBanResult> BulkBanAsync(IGuild guild, BaseDiscordClient client, ulong[] userIds, int? deleteMessageSeconds, RequestOptions options)
        {
            var pos = 0;
            var banned = new List<ulong>(userIds.Length);
            var failed = new List<ulong>();
            while (pos * DiscordConfig.MaxBansPerBulkBatch < userIds.Length)
            {
                var toBan = userIds
                    .Skip(pos * DiscordConfig.MaxBansPerBulkBatch)
                    .Take(DiscordConfig.MaxBansPerBulkBatch);
                pos++;
                var model = await client.ApiClient.BulkBanAsync(guild.Id, toBan.ToArray(), deleteMessageSeconds, options);
                banned.AddRange(model.BannedUsers ?? []);
                failed.AddRange(model.FailedUsers ?? []);
            }
            return new(banned.ToImmutableArray(), failed.ToImmutableArray());
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
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public static async Task<RestTextChannel> CreateTextChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<TextChannelProperties> func = null)
        {
            if (name == null)
                throw new ArgumentNullException(paramName: nameof(name));

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
                DefaultAutoArchiveDuration = props.AutoArchiveDuration
            };
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestTextChannel.Create(client, guild, model);
        }

        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public static async Task<RestNewsChannel> CreateNewsChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<TextChannelProperties> func = null)
        {
            if (name == null)
                throw new ArgumentNullException(paramName: nameof(name));

            var props = new TextChannelProperties();
            func?.Invoke(props);

            var args = new CreateGuildChannelParams(name, ChannelType.News)
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
                DefaultAutoArchiveDuration = props.AutoArchiveDuration
            };
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestNewsChannel.Create(client, guild, model);
        }

        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public static async Task<RestVoiceChannel> CreateVoiceChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<VoiceChannelProperties> func = null)
        {
            if (name == null)
                throw new ArgumentNullException(paramName: nameof(name));

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
                VideoQuality = props.VideoQualityMode,
                RtcRegion = props.RTCRegion
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
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public static async Task<RestCategoryChannel> CreateCategoryChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<GuildChannelProperties> func = null)
        {
            if (name == null)
                throw new ArgumentNullException(paramName: nameof(name));

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

        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public static async Task<RestForumChannel> CreateForumChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<ForumChannelProperties> func = null)
        {
            if (name == null)
                throw new ArgumentNullException(paramName: nameof(name));

            var props = new ForumChannelProperties();
            func?.Invoke(props);

            Preconditions.AtMost(props.Tags.IsSpecified ? props.Tags.Value.Count() : 0, 5, nameof(props.Tags), "Forum channel can have max 20 tags.");

            var args = new CreateGuildChannelParams(name, ChannelType.Forum)
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
                SlowModeInterval = props.ThreadCreationInterval,
                AvailableTags = props.Tags.GetValueOrDefault(Array.Empty<ForumTagProperties>()).Select(
                    x => new ModifyForumTagParams
                    {
                        Id = x.Id ?? Optional<ulong>.Unspecified,
                        Name = x.Name,
                        EmojiId = x.Emoji is Emote emote
                            ? emote.Id
                            : Optional<ulong?>.Unspecified,
                        EmojiName = x.Emoji is Emoji emoji
                            ? emoji.Name
                            : Optional<string>.Unspecified,
                        Moderated = x.IsModerated
                    }).ToArray(),
                DefaultReactionEmoji = props.DefaultReactionEmoji.IsSpecified
                    ? new API.ModifyForumReactionEmojiParams
                    {
                        EmojiId = props.DefaultReactionEmoji.Value is Emote emote ?
                            emote.Id : Optional<ulong?>.Unspecified,
                        EmojiName = props.DefaultReactionEmoji.Value is Emoji emoji ?
                            emoji.Name : Optional<string>.Unspecified
                    }
                    : Optional<ModifyForumReactionEmojiParams>.Unspecified,
                ThreadRateLimitPerUser = props.DefaultSlowModeInterval,
                CategoryId = props.CategoryId,
                IsNsfw = props.IsNsfw,
                Topic = props.Topic,
                DefaultAutoArchiveDuration = props.AutoArchiveDuration,
                DefaultSortOrder = props.DefaultSortOrder.GetValueOrDefault(ForumSortOrder.LatestActivity),
                DefaultLayout = props.DefaultLayout,
            };

            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestForumChannel.Create(client, guild, model);
        }

        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public static async Task<RestMediaChannel> CreateMediaChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options, Action<ForumChannelProperties> func = null)
        {
            if (name == null)
                throw new ArgumentNullException(paramName: nameof(name));

            var props = new ForumChannelProperties();
            func?.Invoke(props);

            Preconditions.AtMost(props.Tags.IsSpecified ? props.Tags.Value.Count() : 0, 20, nameof(props.Tags), "Media channel can have max 20 tags.");

            var args = new CreateGuildChannelParams(name, ChannelType.Media)
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
                SlowModeInterval = props.ThreadCreationInterval,
                AvailableTags = props.Tags.GetValueOrDefault(Array.Empty<ForumTagProperties>()).Select(
                    x => new ModifyForumTagParams
                    {
                        Id = x.Id ?? Optional<ulong>.Unspecified,
                        Name = x.Name,
                        EmojiId = x.Emoji is Emote emote
                            ? emote.Id
                            : Optional<ulong?>.Unspecified,
                        EmojiName = x.Emoji is Emoji emoji
                            ? emoji.Name
                            : Optional<string>.Unspecified,
                        Moderated = x.IsModerated
                    }).ToArray(),
                DefaultReactionEmoji = props.DefaultReactionEmoji.IsSpecified
                    ? new API.ModifyForumReactionEmojiParams
                    {
                        EmojiId = props.DefaultReactionEmoji.Value is Emote emote ?
                            emote.Id : Optional<ulong?>.Unspecified,
                        EmojiName = props.DefaultReactionEmoji.Value is Emoji emoji ?
                            emoji.Name : Optional<string>.Unspecified
                    }
                    : Optional<ModifyForumReactionEmojiParams>.Unspecified,
                ThreadRateLimitPerUser = props.DefaultSlowModeInterval,
                CategoryId = props.CategoryId,
                IsNsfw = props.IsNsfw,
                Topic = props.Topic,
                DefaultAutoArchiveDuration = props.AutoArchiveDuration,
                DefaultSortOrder = props.DefaultSortOrder.GetValueOrDefault(ForumSortOrder.LatestActivity),
            };

            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestMediaChannel.Create(client, guild, model);
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

        public static Task DeleteIntegrationAsync(IGuild guild, BaseDiscordClient client, ulong id, RequestOptions options)
            => client.ApiClient.DeleteIntegrationAsync(guild.Id, id, options);
        #endregion

        #region Interactions
        public static async Task<IReadOnlyCollection<RestGuildCommand>> GetSlashCommandsAsync(IGuild guild, BaseDiscordClient client, bool withLocalizations,
            string locale, RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildApplicationCommandsAsync(guild.Id, withLocalizations, locale, options);
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
            if (vanityModel == null)
                throw new InvalidOperationException("This guild does not have a vanity URL.");
            var inviteModel = await client.ApiClient.GetInviteAsync(vanityModel.Code, options).ConfigureAwait(false);
            inviteModel.Uses = vanityModel.Uses;
            return RestInviteMetadata.Create(client, guild, null, inviteModel);
        }

        #endregion

        #region Roles
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public static async Task<RestRole> CreateRoleAsync(IGuild guild, BaseDiscordClient client,
            string name, GuildPermissions? permissions, Color? color, bool isHoisted, bool isMentionable, RequestOptions options, Image? icon, Emoji emoji)
        {
            if (name == null)
                throw new ArgumentNullException(paramName: nameof(name));

            if (icon is not null || emoji is not null)
            {
                guild.Features.EnsureFeature(GuildFeature.RoleIcons);

                if (icon is not null && emoji is not null)
                {
                    throw new ArgumentException("Emoji and Icon properties cannot be present on a role at the same time.");
                }
            }

            var createGuildRoleParams = new API.Rest.ModifyGuildRoleParams
            {
                Color = color?.RawValue ?? Optional.Create<uint>(),
                Hoist = isHoisted,
                Mentionable = isMentionable,
                Name = name,
                Permissions = permissions?.RawValue.ToString() ?? Optional.Create<string>(),
                Icon = icon?.ToModel(),
                Emoji = emoji?.Name
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
                    args.RoleIds = Optional.Create(args.RoleIds.Value.Concat(ids));
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

        public static Task AddGuildUserAsync(ulong guildId, BaseDiscordClient client, ulong userId, string accessToken,
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

            return client.ApiClient.AddGuildMemberAsync(guildId, userId, apiArgs, options);
        }

        public static async Task<RestGuildUser> GetUserAsync(IGuild guild, BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildMemberAsync(guild.Id, id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildUser.Create(client, guild, model);
            return null;
        }
        public static Task<RestGuildUser> GetCurrentUserAsync(IGuild guild, BaseDiscordClient client, RequestOptions options)
            => GetUserAsync(guild, client, client.CurrentUser.Id, options);

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
            ulong? from, int? limit, RequestOptions options, ulong? userId = null, ActionType? actionType = null, ulong? afterId = null)
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
                    if (afterId.HasValue)
                        args.AfterEntryId = afterId.Value;
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
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null" />.</exception>
        public static async Task<GuildEmote> ModifyEmoteAsync(IGuild guild, BaseDiscordClient client, ulong id, Action<EmoteProperties> func,
            RequestOptions options)
        {
            if (func == null)
                throw new ArgumentNullException(paramName: nameof(func));

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

        public static async Task<API.Sticker> CreateStickerAsync(BaseDiscordClient client, IGuild guild, string name, Image image, IEnumerable<string> tags,
            string description = null, RequestOptions options = null)
        {
            Preconditions.NotNull(name, nameof(name));

            if (description is not null)
            {
                Preconditions.AtLeast(description.Length, 2, nameof(description));
                Preconditions.AtMost(description.Length, 100, nameof(description));
            }

            var tagString = string.Join(", ", tags);

            Preconditions.AtLeast(tagString.Length, 1, nameof(tags));
            Preconditions.AtMost(tagString.Length, 200, nameof(tags));


            Preconditions.AtLeast(name.Length, 2, nameof(name));

            Preconditions.AtMost(name.Length, 30, nameof(name));

            var apiArgs = new CreateStickerParams()
            {
                Name = name,
                Description = description,
                File = image.Stream,
                Tags = tagString
            };

            return await client.ApiClient.CreateGuildStickerAsync(apiArgs, guild.Id, options).ConfigureAwait(false);
        }

        public static Task<API.Sticker> CreateStickerAsync(BaseDiscordClient client, IGuild guild, string name, Stream file, string filename, IEnumerable<string> tags,
            string description = null, RequestOptions options = null)
        {
            Preconditions.NotNull(name, nameof(name));
            Preconditions.NotNull(file, nameof(file));
            Preconditions.NotNull(filename, nameof(filename));

            Preconditions.AtLeast(name.Length, 2, nameof(name));

            Preconditions.AtMost(name.Length, 30, nameof(name));


            if (description is not null)
            {
                Preconditions.AtLeast(description.Length, 2, nameof(description));
                Preconditions.AtMost(description.Length, 100, nameof(description));
            }

            var tagString = string.Join(", ", tags);

            Preconditions.AtLeast(tagString.Length, 1, nameof(tags));
            Preconditions.AtMost(tagString.Length, 200, nameof(tags));

            var apiArgs = new CreateStickerParams()
            {
                Name = name,
                Description = description,
                File = file,
                Tags = tagString,
                FileName = filename
            };

            return client.ApiClient.CreateGuildStickerAsync(apiArgs, guild.Id, options);
        }

        public static Task<API.Sticker> ModifyStickerAsync(BaseDiscordClient client, ulong guildId, ISticker sticker, Action<StickerProperties> func,
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

            return client.ApiClient.ModifyStickerAsync(apiArgs, guildId, sticker.Id, options);
        }

        public static Task DeleteStickerAsync(BaseDiscordClient client, ulong guildId, ISticker sticker, RequestOptions options = null)
            => client.ApiClient.DeleteStickerAsync(guildId, sticker.Id, options);
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

        public static Task<API.GuildScheduledEvent> ModifyGuildEventAsync(BaseDiscordClient client, Action<GuildScheduledEventsProperties> func,
            IGuildScheduledEvent guildEvent, RequestOptions options = null)
        {
            var args = new GuildScheduledEventsProperties();

            func(args);

            if (args.Status.IsSpecified)
            {
                switch (args.Status.Value)
                {
                    case GuildScheduledEventStatus.Active when guildEvent.Status != GuildScheduledEventStatus.Scheduled:
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

            if (args.Location.IsSpecified)
            {
                apiArgs.EntityMetadata = new API.GuildScheduledEventEntityMetadata()
                {
                    Location = args.Location,
                };
            }

            return client.ApiClient.ModifyGuildScheduledEventAsync(apiArgs, guildEvent.Id, guildEvent.Guild.Id, options);
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
            if (location != null)
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

            if (location != null)
            {
                apiArgs.EntityMetadata = new API.GuildScheduledEventEntityMetadata()
                {
                    Location = location
                };
            }

            var model = await client.ApiClient.CreateGuildScheduledEventAsync(apiArgs, guild.Id, options).ConfigureAwait(false);

            return RestGuildEvent.Create(client, guild, client.CurrentUser, model);
        }

        public static Task DeleteEventAsync(BaseDiscordClient client, IGuildScheduledEvent guildEvent, RequestOptions options = null)
            => client.ApiClient.DeleteGuildScheduledEventAsync(guildEvent.Id, guildEvent.Guild.Id, options);

        #endregion

        #region Welcome Screen

        public static async Task<WelcomeScreen> GetWelcomeScreenAsync(IGuild guild, BaseDiscordClient client, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildWelcomeScreenAsync(guild.Id, options);

            if (model.WelcomeChannels.Length == 0)
                return null;

            return new WelcomeScreen(model.Description.GetValueOrDefault(null), model.WelcomeChannels.Select(
                x => new WelcomeScreenChannel(
                    x.ChannelId, x.Description,
                    x.EmojiName.GetValueOrDefault(null),
                    x.EmojiId.GetValueOrDefault(0))).ToList());
        }

        public static async Task<WelcomeScreen> ModifyWelcomeScreenAsync(bool enabled, string description, WelcomeScreenChannelProperties[] channels, IGuild guild, BaseDiscordClient client, RequestOptions options)
        {
            if (!guild.Features.HasFeature(GuildFeature.Community))
                throw new InvalidOperationException("Cannot update welcome screen in a non-community guild.");

            var args = new ModifyGuildWelcomeScreenParams
            {
                Enabled = enabled,
                Description = description,
                WelcomeChannels = channels?.Select(ch => new API.WelcomeScreenChannel
                {
                    ChannelId = ch.Id,
                    Description = ch.Description,
                    EmojiName = ch.Emoji is Emoji emoj ? emoj.Name : Optional<string>.Unspecified,
                    EmojiId = ch.Emoji is Emote emote ? emote.Id : Optional<ulong?>.Unspecified
                }).ToArray()
            };

            var model = await client.ApiClient.ModifyGuildWelcomeScreenAsync(args, guild.Id, options);

            if (model.WelcomeChannels.Length == 0)
                return null;

            return new WelcomeScreen(model.Description.GetValueOrDefault(null), model.WelcomeChannels.Select(
                x => new WelcomeScreenChannel(
                    x.ChannelId, x.Description,
                    x.EmojiName.GetValueOrDefault(null),
                    x.EmojiId.GetValueOrDefault(0))).ToList());
        }

        #endregion

        #region Auto Mod

        public static Task<AutoModerationRule> CreateAutoModRuleAsync(IGuild guild, Action<AutoModRuleProperties> func, BaseDiscordClient client, RequestOptions options)
        {
            var args = new AutoModRuleProperties();
            func(args);

            #region Validations

            if (!args.TriggerType.IsSpecified)
                throw new ArgumentException(message: $"AutoMod rule must have a specified type.", paramName: nameof(args.TriggerType));

            if (!args.Name.IsSpecified || string.IsNullOrWhiteSpace(args.Name.Value))
                throw new ArgumentException("Name of the rule must not be empty", paramName: nameof(args.Name));

            Preconditions.AtLeast(args.Actions.GetValueOrDefault(Array.Empty<AutoModRuleActionProperties>()).Length, 1, nameof(args.Actions), "Auto moderation rule must have at least 1 action");

            if (args.RegexPatterns.IsSpecified)
            {
                if (args.TriggerType.Value is not AutoModTriggerType.Keyword and not AutoModTriggerType.MemberProfile)
                    throw new ArgumentException(message: $"Regex patterns can only be used with 'Keyword' or 'MemberProfile' trigger type.", paramName: nameof(args.RegexPatterns));

                Preconditions.AtMost(args.RegexPatterns.Value.Length, AutoModRuleProperties.MaxRegexPatternCount, nameof(args.RegexPatterns), $"Regex pattern count must be less than or equal to {AutoModRuleProperties.MaxRegexPatternCount}.");

                if (args.RegexPatterns.Value.Any(x => x.Length > AutoModRuleProperties.MaxRegexPatternLength))
                    throw new ArgumentException(message: $"Regex pattern must be less than or equal to {AutoModRuleProperties.MaxRegexPatternLength}.", paramName: nameof(args.RegexPatterns));
            }

            if (args.KeywordFilter.IsSpecified)
            {
                if (args.TriggerType.Value is not AutoModTriggerType.Keyword and not AutoModTriggerType.MemberProfile)
                    throw new ArgumentException(message: $"Keyword filter can only be used with 'Keyword' or 'MemberProfile' trigger type.", paramName: nameof(args.KeywordFilter));

                Preconditions.AtMost(args.KeywordFilter.Value.Length, AutoModRuleProperties.MaxKeywordCount, nameof(args.KeywordFilter), $"Keyword count must be less than or equal to {AutoModRuleProperties.MaxKeywordCount}");

                if (args.KeywordFilter.Value.Any(x => x.Length > AutoModRuleProperties.MaxKeywordLength))
                    throw new ArgumentException(message: $"Keyword length must be less than or equal to {AutoModRuleProperties.MaxKeywordLength}.", paramName: nameof(args.KeywordFilter));
            }

            if (args.TriggerType.Value is AutoModTriggerType.Keyword)
                Preconditions.AtLeast(args.KeywordFilter.GetValueOrDefault(Array.Empty<string>()).Length + args.RegexPatterns.GetValueOrDefault(Array.Empty<string>()).Length, 1, "KeywordFilter & RegexPatterns", "Auto moderation rule must have at least 1 keyword or regex pattern");

            if (args.AllowList.IsSpecified)
            {
                if (args.TriggerType.Value is not AutoModTriggerType.Keyword and not AutoModTriggerType.KeywordPreset and not AutoModTriggerType.MemberProfile)
                    throw new ArgumentException(message: $"Allow list can only be used with 'Keyword', 'KeywordPreset' or 'MemberProfile' trigger type.", paramName: nameof(args.AllowList));

                if (args.TriggerType.Value is AutoModTriggerType.Keyword)
                    Preconditions.AtMost(args.AllowList.Value.Length, AutoModRuleProperties.MaxAllowListCountKeyword, nameof(args.AllowList), $"Allow list entry count must be less than or equal to {AutoModRuleProperties.MaxAllowListCountKeyword}.");

                if (args.TriggerType.Value is AutoModTriggerType.KeywordPreset)
                    Preconditions.AtMost(args.AllowList.Value.Length, AutoModRuleProperties.MaxAllowListCountKeywordPreset, nameof(args.AllowList), $"Allow list entry count must be less than or equal to {AutoModRuleProperties.MaxAllowListCountKeywordPreset}.");

                if (args.AllowList.Value.Any(x => x.Length > AutoModRuleProperties.MaxAllowListEntryLength))
                    throw new ArgumentException(message: $"Allow list entry length must be less than or equal to {AutoModRuleProperties.MaxAllowListEntryLength}.", paramName: nameof(args.AllowList));

            }

            if (args.TriggerType.Value is not AutoModTriggerType.KeywordPreset && args.Presets.IsSpecified)
                throw new ArgumentException(message: $"Keyword presets scan only be used with 'KeywordPreset' trigger type.", paramName: nameof(args.Presets));

            if (args.MentionLimit.IsSpecified)
            {
                if (args.TriggerType.Value is AutoModTriggerType.MentionSpam)
                {
                    Preconditions.AtMost(args.MentionLimit.Value, AutoModRuleProperties.MaxMentionLimit, nameof(args.MentionLimit), $"Mention limit must be less or equal to {AutoModRuleProperties.MaxMentionLimit}");
                    Preconditions.AtLeast(args.MentionLimit.Value, 1, nameof(args.MentionLimit), $"Mention limit must be greater or equal to 1");
                }
                else
                {
                    throw new ArgumentException(message: $"MentionLimit can only be used with 'MentionSpam' trigger type.", paramName: nameof(args.MentionLimit));
                }
            }

            if (args.ExemptRoles.IsSpecified)
                Preconditions.AtMost(args.ExemptRoles.Value.Length, AutoModRuleProperties.MaxExemptRoles, nameof(args.ExemptRoles), $"Exempt roles count must be less than or equal to {AutoModRuleProperties.MaxExemptRoles}.");

            if (args.ExemptChannels.IsSpecified)
                Preconditions.AtMost(args.ExemptChannels.Value.Length, AutoModRuleProperties.MaxExemptChannels, nameof(args.ExemptChannels), $"Exempt channels count must be less than or equal to {AutoModRuleProperties.MaxExemptChannels}.");

            if (!args.Actions.IsSpecified || args.Actions.Value.Length == 0)
            {
                throw new ArgumentException(message: $"At least 1 action must be set for an auto moderation rule.", paramName: nameof(args.Actions));
            }

            if (args.Actions.Value.Any(x => x.TimeoutDuration.GetValueOrDefault().TotalSeconds > AutoModRuleProperties.MaxTimeoutSeconds))
                throw new ArgumentException(message: $"Field count must be less than or equal to {AutoModRuleProperties.MaxTimeoutSeconds}.", paramName: nameof(AutoModRuleActionProperties.TimeoutDuration));

            if (args.Actions.Value.Any(x => x.CustomMessage.IsSpecified && x.CustomMessage.Value.Length > AutoModRuleProperties.MaxCustomBlockMessageLength))
                throw new ArgumentException(message: $"Custom message length must be less than or equal to {AutoModRuleProperties.MaxCustomBlockMessageLength}.", paramName: nameof(AutoModRuleActionProperties.CustomMessage));

            #endregion

            var props = new CreateAutoModRuleParams
            {
                EventType = args.EventType.GetValueOrDefault(AutoModEventType.MessageSend),
                Enabled = args.Enabled.GetValueOrDefault(true),
                ExemptRoles = args.ExemptRoles.GetValueOrDefault(),
                ExemptChannels = args.ExemptChannels.GetValueOrDefault(),
                Name = args.Name.Value,
                TriggerType = args.TriggerType.Value,
                Actions = args.Actions.Value.Select(x => new AutoModAction
                {
                    Metadata = new ActionMetadata
                    {
                        ChannelId = x.ChannelId ?? Optional<ulong>.Unspecified,
                        DurationSeconds = (int?)x.TimeoutDuration?.TotalSeconds ?? Optional<int>.Unspecified,
                        CustomMessage = x.CustomMessage,
                    },
                    Type = x.Type
                }).ToArray(),
                TriggerMetadata = new TriggerMetadata
                {
                    AllowList = args.AllowList,
                    KeywordFilter = args.KeywordFilter,
                    MentionLimit = args.MentionLimit,
                    Presets = args.Presets,
                    RegexPatterns = args.RegexPatterns,
                },
            };

            return client.ApiClient.CreateGuildAutoModRuleAsync(guild.Id, props, options);
        }

        public static Task<AutoModerationRule> GetAutoModRuleAsync(ulong ruleId, IGuild guild, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.GetGuildAutoModRuleAsync(guild.Id, ruleId, options);

        public static Task<AutoModerationRule[]> GetAutoModRulesAsync(IGuild guild, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.GetGuildAutoModRulesAsync(guild.Id, options);

        public static Task<AutoModerationRule> ModifyRuleAsync(BaseDiscordClient client, IAutoModRule rule, Action<AutoModRuleProperties> func, RequestOptions options)
        {
            var args = new AutoModRuleProperties();
            func(args);

            var apiArgs = new API.Rest.ModifyAutoModRuleParams
            {
                Actions = args.Actions.IsSpecified ? args.Actions.Value.Select(x => new API.AutoModAction()
                {
                    Type = x.Type,
                    Metadata = x.ChannelId.HasValue || x.TimeoutDuration.HasValue ? new API.ActionMetadata
                    {
                        ChannelId = x.ChannelId ?? Optional<ulong>.Unspecified,
                        DurationSeconds = x.TimeoutDuration.HasValue ? (int)Math.Floor(x.TimeoutDuration.Value.TotalSeconds) : Optional<int>.Unspecified,
                        CustomMessage = x.CustomMessage,
                    } : Optional<API.ActionMetadata>.Unspecified
                }).ToArray() : Optional<API.AutoModAction[]>.Unspecified,
                Enabled = args.Enabled,
                EventType = args.EventType,
                ExemptChannels = args.ExemptChannels,
                ExemptRoles = args.ExemptRoles,
                Name = args.Name,
                TriggerType = args.TriggerType,
                TriggerMetadata = args.KeywordFilter.IsSpecified
                                  || args.Presets.IsSpecified
                                  || args.MentionLimit.IsSpecified
                                  || args.RegexPatterns.IsSpecified
                                  || args.AllowList.IsSpecified ? new API.TriggerMetadata
                                  {
                                      KeywordFilter = args.KeywordFilter.IsSpecified ? args.KeywordFilter : rule.KeywordFilter.ToArray(),
                                      RegexPatterns = args.RegexPatterns.IsSpecified ? args.RegexPatterns : rule.RegexPatterns.ToArray(),
                                      AllowList = args.AllowList.IsSpecified ? args.AllowList : rule.AllowList.ToArray(),
                                      MentionLimit = args.MentionLimit.IsSpecified ? args.MentionLimit : rule.MentionTotalLimit ?? Optional<int>.Unspecified,
                                      Presets = args.Presets.IsSpecified ? args.Presets : rule.Presets.ToArray(),
                                  } : Optional<API.TriggerMetadata>.Unspecified
            };

            return client.ApiClient.ModifyGuildAutoModRuleAsync(rule.GuildId, rule.Id, apiArgs, options);
        }

        public static Task DeleteRuleAsync(BaseDiscordClient client, IAutoModRule rule, RequestOptions options)
            => client.ApiClient.DeleteGuildAutoModRuleAsync(rule.GuildId, rule.Id, options);
        #endregion

        #region Onboarding

        public static Task<GuildOnboarding> GetGuildOnboardingAsync(IGuild guild, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.GetGuildOnboardingAsync(guild.Id, options);

        public static Task<GuildOnboarding> ModifyGuildOnboardingAsync(IGuild guild, Action<GuildOnboardingProperties> func, BaseDiscordClient client, RequestOptions options)
        {
            var props = new GuildOnboardingProperties();
            func(props);

            var args = new ModifyGuildOnboardingParams
            {
                DefaultChannelIds = props.ChannelIds.IsSpecified
                    ? props.ChannelIds.Value.ToArray()
                    : Optional<ulong[]>.Unspecified,
                Enabled = props.IsEnabled,
                Mode = props.Mode,
                Prompts = props.Prompts.IsSpecified ? props.Prompts.Value?
                    .Select(prompt => new GuildOnboardingPromptParams
                    {
                        Id = prompt.Id ?? 0,
                        Type = prompt.Type,
                        IsInOnboarding = prompt.IsInOnboarding,
                        IsRequired = prompt.IsRequired,
                        IsSingleSelect = prompt.IsSingleSelect,
                        Title = prompt.Title,
                        Options = prompt.Options?
                            .Select(option => new GuildOnboardingPromptOptionParams
                            {
                                Title = option.Title,
                                ChannelIds = option.ChannelIds?.ToArray(),
                                RoleIds = option.RoleIds?.ToArray(),
                                Description = option.Description,
                                EmojiName = option.Emoji.GetValueOrDefault(null)?.Name,
                                EmojiId = option.Emoji.GetValueOrDefault(null) is Emote emote ? emote.Id : null,
                                EmojiAnimated = option.Emoji.GetValueOrDefault(null) is Emote emt ? emt.Animated : null,
                                Id = option.Id ?? Optional<ulong>.Unspecified,
                            }).ToArray(),
                    }).ToArray()
                : Optional<GuildOnboardingPromptParams[]>.Unspecified,
            };

            return client.ApiClient.ModifyGuildOnboardingAsync(guild.Id, args, options);
        }

        #endregion
    }
}
