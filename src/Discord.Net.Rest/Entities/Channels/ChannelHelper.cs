using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;
using StageInstance = Discord.API.StageInstance;

namespace Discord.Rest
{
    internal static class ChannelHelper
    {
        #region General
        public static async Task DeleteAsync(IChannel channel, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.DeleteChannelAsync(channel.Id, options).ConfigureAwait(false);
        }
        public static async Task<Model> ModifyAsync(IGuildChannel channel, BaseDiscordClient client,
            Action<GuildChannelProperties> func,
            RequestOptions options)
        {
            var args = new GuildChannelProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyGuildChannelParams
            {
                Name = args.Name,
                Position = args.Position,
                CategoryId = args.CategoryId,
                Overwrites = args.PermissionOverwrites.IsSpecified
                    ? args.PermissionOverwrites.Value.Select(overwrite => new API.Overwrite
                    {
                        TargetId = overwrite.TargetId,
                        TargetType = overwrite.TargetType,
                        Allow = overwrite.Permissions.AllowValue.ToString(),
                        Deny = overwrite.Permissions.DenyValue.ToString()
                    }).ToArray()
                    : Optional.Create<API.Overwrite[]>(),
            };
            return await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
        }
        public static async Task<Model> ModifyAsync(ITextChannel channel, BaseDiscordClient client,
            Action<TextChannelProperties> func,
            RequestOptions options)
        {
            var args = new TextChannelProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyTextChannelParams
            {
                Name = args.Name,
                Position = args.Position,
                CategoryId = args.CategoryId,
                Topic = args.Topic,
                IsNsfw = args.IsNsfw,
                SlowModeInterval = args.SlowModeInterval,
                Overwrites = args.PermissionOverwrites.IsSpecified
                    ? args.PermissionOverwrites.Value.Select(overwrite => new API.Overwrite
                    {
                        TargetId = overwrite.TargetId,
                        TargetType = overwrite.TargetType,
                        Allow = overwrite.Permissions.AllowValue.ToString(),
                        Deny = overwrite.Permissions.DenyValue.ToString()
                    }).ToArray()
                    : Optional.Create<API.Overwrite[]>(),
            };
            return await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
        }
        public static async Task<Model> ModifyAsync(IVoiceChannel channel, BaseDiscordClient client,
            Action<VoiceChannelProperties> func,
            RequestOptions options)
        {
            var args = new VoiceChannelProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyVoiceChannelParams
            {
                Bitrate = args.Bitrate,
                Name = args.Name,
                Position = args.Position,
                CategoryId = args.CategoryId,
                UserLimit = args.UserLimit.IsSpecified ? (args.UserLimit.Value ?? 0) : Optional.Create<int>(),
                Overwrites = args.PermissionOverwrites.IsSpecified
                    ? args.PermissionOverwrites.Value.Select(overwrite => new API.Overwrite
                    {
                        TargetId = overwrite.TargetId,
                        TargetType = overwrite.TargetType,
                        Allow = overwrite.Permissions.AllowValue.ToString(),
                        Deny = overwrite.Permissions.DenyValue.ToString()
                    }).ToArray()
                    : Optional.Create<API.Overwrite[]>(),
            };
            return await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
        }

        public static async Task<StageInstance> ModifyAsync(IStageChannel channel, BaseDiscordClient client,
            Action<StageInstanceProperties> func, RequestOptions options = null)
        {
            var args = new StageInstanceProperties();
            func(args);

            var apiArgs = new ModifyStageInstanceParams()
            {
                PrivacyLevel = args.PrivacyLevel,
                Topic = args.Topic
            };

            return await client.ApiClient.ModifyStageInstanceAsync(channel.Id, apiArgs, options);
        }
        #endregion

        #region Invites
        public static async Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(IGuildChannel channel, BaseDiscordClient client,
            RequestOptions options)
        {
            var models = await client.ApiClient.GetChannelInvitesAsync(channel.Id, options).ConfigureAwait(false);
            return models.Select(x => RestInviteMetadata.Create(client, null, channel, x)).ToImmutableArray();
        }
        /// <exception cref="ArgumentException">
        /// <paramref name="channel.Id"/> may not be equal to zero.
        /// -and-
        /// <paramref name="maxAge"/> and <paramref name="maxUses"/> must be greater than zero.
        /// -and-
        /// <paramref name="maxAge"/> must be lesser than 86400.
        /// </exception>
        public static async Task<RestInviteMetadata> CreateInviteAsync(IGuildChannel channel, BaseDiscordClient client,
            int? maxAge, int? maxUses, bool isTemporary, bool isUnique, RequestOptions options)
        {
            var args = new API.Rest.CreateChannelInviteParams
            {
                IsTemporary = isTemporary,
                IsUnique = isUnique,
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0
            };
            var model = await client.ApiClient.CreateChannelInviteAsync(channel.Id, args, options).ConfigureAwait(false);
            return RestInviteMetadata.Create(client, null, channel, model);
        }

        /// <exception cref="ArgumentException">
        /// <paramref name="channel.Id"/> may not be equal to zero.
        /// -and-
        /// <paramref name="maxAge"/> and <paramref name="maxUses"/> must be greater than zero.
        /// -and-
        /// <paramref name="maxAge"/> must be lesser than 86400.
        /// </exception>
        public static async Task<RestInviteMetadata> CreateInviteToStreamAsync(IGuildChannel channel, BaseDiscordClient client,
            int? maxAge, int? maxUses, bool isTemporary, bool isUnique, IUser user,
            RequestOptions options)
        {
            var args = new API.Rest.CreateChannelInviteParams
            {
                IsTemporary = isTemporary,
                IsUnique = isUnique,
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                TargetType = TargetUserType.Stream,
                TargetUserId = user.Id
            };
            var model = await client.ApiClient.CreateChannelInviteAsync(channel.Id, args, options).ConfigureAwait(false);
            return RestInviteMetadata.Create(client, null, channel, model);
        }

        /// <exception cref="ArgumentException">
        /// <paramref name="channel.Id"/> may not be equal to zero.
        /// -and-
        /// <paramref name="maxAge"/> and <paramref name="maxUses"/> must be greater than zero.
        /// -and-
        /// <paramref name="maxAge"/> must be lesser than 86400.
        /// </exception>
        public static async Task<RestInviteMetadata> CreateInviteToApplicationAsync(IGuildChannel channel, BaseDiscordClient client,
            int? maxAge, int? maxUses, bool isTemporary, bool isUnique, ulong applicationId,
            RequestOptions options)
        {
            var args = new API.Rest.CreateChannelInviteParams
            {
                IsTemporary = isTemporary,
                IsUnique = isUnique,
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                TargetType = TargetUserType.EmbeddedApplication,
                TargetApplicationId = applicationId
            };
            var model = await client.ApiClient.CreateChannelInviteAsync(channel.Id, args, options).ConfigureAwait(false);
            return RestInviteMetadata.Create(client, null, channel, model);
        }
        #endregion

        #region Messages
        public static async Task<RestMessage> GetMessageAsync(IMessageChannel channel, BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var guildId = (channel as IGuildChannel)?.GuildId;
            var guild = guildId != null ? await (client as IDiscordClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).ConfigureAwait(false) : null;
            var model = await client.ApiClient.GetChannelMessageAsync(channel.Id, id, options).ConfigureAwait(false);
            if (model == null)
                return null;
            var author = MessageHelper.GetAuthor(client, guild, model.Author.Value, model.WebhookId.ToNullable());
            return RestMessage.Create(client, channel, author, model);
        }
        public static IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessageChannel channel, BaseDiscordClient client,
            ulong? fromMessageId, Direction dir, int limit, RequestOptions options)
        {
            var guildId = (channel as IGuildChannel)?.GuildId;
            var guild = guildId != null ? (client as IDiscordClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).Result : null;

            if (dir == Direction.Around && limit > DiscordConfig.MaxMessagesPerBatch)
            {
                int around = limit / 2;
                if (fromMessageId.HasValue)
                    return GetMessagesAsync(channel, client, fromMessageId.Value + 1, Direction.Before, around + 1, options) //Need to include the message itself
                        .Concat(GetMessagesAsync(channel, client, fromMessageId, Direction.After, around, options));
                else //Shouldn't happen since there's no public overload for ulong? and Direction
                    return GetMessagesAsync(channel, client, null, Direction.Before, around + 1, options);
            }

            return new PagedAsyncEnumerable<RestMessage>(
                DiscordConfig.MaxMessagesPerBatch,
                async (info, ct) =>
                {
                    var args = new GetChannelMessagesParams
                    {
                        RelativeDirection = dir,
                        Limit = info.PageSize
                    };
                    if (info.Position != null)
                        args.RelativeMessageId = info.Position.Value;

                    var models = await client.ApiClient.GetChannelMessagesAsync(channel.Id, args, options).ConfigureAwait(false);
                    var builder = ImmutableArray.CreateBuilder<RestMessage>();
                    foreach (var model in models)
                    {
                        var author = MessageHelper.GetAuthor(client, guild, model.Author.Value, model.WebhookId.ToNullable());
                        builder.Add(RestMessage.Create(client, channel, author, model));
                    }
                    return builder.ToImmutable();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxMessagesPerBatch)
                        return false;
                    if (dir == Direction.Before)
                        info.Position = lastPage.Min(x => x.Id);
                    else
                        info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                start: fromMessageId,
                count: limit
            );
        }
        public static async Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(IMessageChannel channel, BaseDiscordClient client,
            RequestOptions options)
        {
            var guildId = (channel as IGuildChannel)?.GuildId;
            var guild = guildId != null ? await (client as IDiscordClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).ConfigureAwait(false) : null;
            var models = await client.ApiClient.GetPinsAsync(channel.Id, options).ConfigureAwait(false);
            var builder = ImmutableArray.CreateBuilder<RestMessage>();
            foreach (var model in models)
            {
                var author = MessageHelper.GetAuthor(client, guild, model.Author.Value, model.WebhookId.ToNullable());
                builder.Add(RestMessage.Create(client, channel, author, model));
            }
            return builder.ToImmutable();
        }

        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public static async Task<RestUserMessage> SendMessageAsync(IMessageChannel channel, BaseDiscordClient client,
            string text, bool isTTS, Embed embed, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent component, ISticker[] stickers, RequestOptions options)
        {
            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");

            // check that user flag and user Id list are exclusive, same with role flag and role Id list
            if (allowedMentions != null && allowedMentions.AllowedTypes.HasValue)
            {
                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users) &&
                    allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                {
                    throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.", nameof(allowedMentions));
                }

                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles) &&
                    allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                {
                    throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.", nameof(allowedMentions));
                }
            }

            if (stickers != null)
            {
                Preconditions.AtMost(stickers.Length, 3, nameof(stickers), "A max of 3 stickers are allowed.");
            }

            var args = new CreateMessageParams(text) { IsTTS = isTTS, Embed = embed?.ToModel(), AllowedMentions = allowedMentions?.ToModel(), MessageReference = messageReference?.ToModel(), Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified, Stickers = stickers?.Any() ?? false ? stickers.Select(x => x.Id).ToArray() : Optional<ulong[]>.Unspecified };
            var model = await client.ApiClient.CreateMessageAsync(channel.Id, args, options).ConfigureAwait(false);
            return RestUserMessage.Create(client, channel, client.CurrentUser, model);
        }

        /// <exception cref="ArgumentException">
        /// <paramref name="filePath" /> is a zero-length string, contains only white space, or contains one or more
        /// invalid characters as defined by <see cref="System.IO.Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filePath" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length. For example, on
        /// Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260
        /// characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="filePath" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="filePath" /> was not found.
        /// </exception>
        /// <exception cref="NotSupportedException"><paramref name="filePath" /> is in an invalid format.</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public static async Task<RestUserMessage> SendFileAsync(IMessageChannel channel, BaseDiscordClient client,
            string filePath, string text, bool isTTS, Embed embed, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent component, ISticker[] stickers, RequestOptions options, bool isSpoiler)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
                return await SendFileAsync(channel, client, file, filename, text, isTTS, embed, allowedMentions, messageReference, component, stickers, options, isSpoiler).ConfigureAwait(false);
        }

        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public static async Task<RestUserMessage> SendFileAsync(IMessageChannel channel, BaseDiscordClient client,
            Stream stream, string filename, string text, bool isTTS, Embed embed, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent component, ISticker[] stickers, RequestOptions options, bool isSpoiler)
        {
            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");

            // check that user flag and user Id list are exclusive, same with role flag and role Id list
            if (allowedMentions != null && allowedMentions.AllowedTypes.HasValue)
            {
                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users) &&
                    allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                {
                    throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.", nameof(allowedMentions));
                }

                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles) &&
                    allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                {
                    throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.", nameof(allowedMentions));
                }
            }

            if (stickers != null)
            {
                Preconditions.AtMost(stickers.Length, 3, nameof(stickers), "A max of 3 stickers are allowed.");
            }

            var args = new UploadFileParams(stream) { Filename = filename, Content = text, IsTTS = isTTS, Embed = embed?.ToModel() ?? Optional<API.Embed>.Unspecified, AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified, MessageReference = messageReference?.ToModel() ?? Optional<API.MessageReference>.Unspecified, MessageComponent = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified, IsSpoiler = isSpoiler, Stickers = stickers?.Any() ?? false ? stickers.Select(x => x.Id).ToArray() : Optional<ulong[]>.Unspecified };
            var model = await client.ApiClient.UploadFileAsync(channel.Id, args, options).ConfigureAwait(false);
            return RestUserMessage.Create(client, channel, client.CurrentUser, model);
        }

        public static async Task<RestUserMessage> ModifyMessageAsync(IMessageChannel channel, ulong messageId, Action<MessageProperties> func,
            BaseDiscordClient client, RequestOptions options)
        {
            var msgModel = await MessageHelper.ModifyAsync(channel.Id, messageId, client, func, options).ConfigureAwait(false);
            return RestUserMessage.Create(client, channel, msgModel.Author.IsSpecified ? RestUser.Create(client, msgModel.Author.Value) : client.CurrentUser, msgModel);
        }

        public static Task DeleteMessageAsync(IMessageChannel channel, ulong messageId, BaseDiscordClient client,
            RequestOptions options)
            => MessageHelper.DeleteAsync(channel.Id, messageId, client, options);

        public static async Task DeleteMessagesAsync(ITextChannel channel, BaseDiscordClient client,
            IEnumerable<ulong> messageIds, RequestOptions options)
        {
            const int BATCH_SIZE = 100;

            var msgs = messageIds.ToArray();
            int batches = msgs.Length / BATCH_SIZE;
            for (int i = 0; i <= batches; i++)
            {
                ArraySegment<ulong> batch;
                if (i < batches)
                {
                    batch = new ArraySegment<ulong>(msgs, i * BATCH_SIZE, BATCH_SIZE);
                }
                else
                {
                    batch = new ArraySegment<ulong>(msgs, i * BATCH_SIZE, msgs.Length - batches * BATCH_SIZE);
                    if (batch.Count == 0)
                    {
                        break;
                    }
                }
                var args = new DeleteMessagesParams(batch.ToArray());
                await client.ApiClient.DeleteMessagesAsync(channel.Id, args, options).ConfigureAwait(false);
            }
        }
        #endregion

        #region Permission Overwrites
        public static async Task AddPermissionOverwriteAsync(IGuildChannel channel, BaseDiscordClient client,
            IUser user, OverwritePermissions perms, RequestOptions options)
        {
            var args = new ModifyChannelPermissionsParams((int)PermissionTarget.User, perms.AllowValue.ToString(), perms.DenyValue.ToString());
            await client.ApiClient.ModifyChannelPermissionsAsync(channel.Id, user.Id, args, options).ConfigureAwait(false);
        }
        public static async Task AddPermissionOverwriteAsync(IGuildChannel channel, BaseDiscordClient client,
            IRole role, OverwritePermissions perms, RequestOptions options)
        {
            var args = new ModifyChannelPermissionsParams((int)PermissionTarget.Role, perms.AllowValue.ToString(), perms.DenyValue.ToString());
            await client.ApiClient.ModifyChannelPermissionsAsync(channel.Id, role.Id, args, options).ConfigureAwait(false);
        }
        public static async Task RemovePermissionOverwriteAsync(IGuildChannel channel, BaseDiscordClient client,
            IUser user, RequestOptions options)
        {
            await client.ApiClient.DeleteChannelPermissionAsync(channel.Id, user.Id, options).ConfigureAwait(false);
        }
        public static async Task RemovePermissionOverwriteAsync(IGuildChannel channel, BaseDiscordClient client,
            IRole role, RequestOptions options)
        {
            await client.ApiClient.DeleteChannelPermissionAsync(channel.Id, role.Id, options).ConfigureAwait(false);
        }
        #endregion

        #region Users
        /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
        public static async Task<RestGuildUser> GetUserAsync(IGuildChannel channel, IGuild guild, BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildMemberAsync(channel.GuildId, id, options).ConfigureAwait(false);
            if (model == null)
                return null;
            var user = RestGuildUser.Create(client, guild, model);
            if (!user.GetPermissions(channel).ViewChannel)
                return null;

            return user;
        }
        /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
        public static IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(IGuildChannel channel, IGuild guild, BaseDiscordClient client,
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
                    return models
                        .Select(x => RestGuildUser.Create(client, guild, x))
                        .Where(x => x.GetPermissions(channel).ViewChannel)
                        .ToImmutableArray();
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
        #endregion

        #region Typing
        public static async Task TriggerTypingAsync(IMessageChannel channel, BaseDiscordClient client,
            RequestOptions options = null)
        {
            await client.ApiClient.TriggerTypingIndicatorAsync(channel.Id, options).ConfigureAwait(false);
        }
        public static IDisposable EnterTypingState(IMessageChannel channel, BaseDiscordClient client,
            RequestOptions options)
            => new TypingNotifier(channel, options);
        #endregion

        #region Webhooks
        public static async Task<RestWebhook> CreateWebhookAsync(ITextChannel channel, BaseDiscordClient client, string name, Stream avatar, RequestOptions options)
        {
            var args = new CreateWebhookParams { Name = name };
            if (avatar != null)
                args.Avatar = new API.Image(avatar);

            var model = await client.ApiClient.CreateWebhookAsync(channel.Id, args, options).ConfigureAwait(false);
            return RestWebhook.Create(client, channel, model);
        }
        public static async Task<RestWebhook> GetWebhookAsync(ITextChannel channel, BaseDiscordClient client, ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetWebhookAsync(id, options: options).ConfigureAwait(false);
            if (model == null)
                return null;
            return RestWebhook.Create(client, channel, model);
        }
        public static async Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(ITextChannel channel, BaseDiscordClient client, RequestOptions options)
        {
            var models = await client.ApiClient.GetChannelWebhooksAsync(channel.Id, options).ConfigureAwait(false);
            return models.Select(x => RestWebhook.Create(client, channel, x))
                .ToImmutableArray();
        }
        #endregion

        #region Categories
        public static async Task<ICategoryChannel> GetCategoryAsync(INestedChannel channel, BaseDiscordClient client, RequestOptions options)
        {
            // if no category id specified, return null
            if (!channel.CategoryId.HasValue)
                return null;
            // CategoryId will contain a value here
            var model = await client.ApiClient.GetChannelAsync(channel.CategoryId.Value, options).ConfigureAwait(false);
            return RestCategoryChannel.Create(client, model) as ICategoryChannel;
        }
        /// <exception cref="InvalidOperationException">This channel does not have a parent channel.</exception>
        public static async Task SyncPermissionsAsync(INestedChannel channel, BaseDiscordClient client, RequestOptions options)
        {
            var category = await GetCategoryAsync(channel, client, options).ConfigureAwait(false);
            if (category == null)
                throw new InvalidOperationException("This channel does not have a parent channel.");

            var apiArgs = new ModifyGuildChannelParams
            {
                Overwrites = category.PermissionOverwrites
                    .Select(overwrite => new API.Overwrite
                    {
                        TargetId = overwrite.TargetId,
                        TargetType = overwrite.TargetType,
                        Allow = overwrite.Permissions.AllowValue.ToString(),
                        Deny = overwrite.Permissions.DenyValue.ToString()
                    }).ToArray()
            };
            await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
        }
        #endregion
    }
}
