using Discord.API;
using Discord.API.Rest;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class InteractionHelper
    {
        public const double ResponseTimeLimit = 3;
        public const double ResponseAndFollowupLimit = 15;

        #region InteractionHelper
        public static bool CanSendResponse(IDiscordInteraction interaction)
        {
            return (DateTime.UtcNow - interaction.CreatedAt).TotalSeconds < ResponseTimeLimit;
        }
        public static bool CanRespondOrFollowup(IDiscordInteraction interaction)
        {
            return (DateTime.UtcNow - interaction.CreatedAt).TotalMinutes <= ResponseAndFollowupLimit;
        }

        public static Task DeleteAllGuildCommandsAsync(BaseDiscordClient client, ulong guildId, RequestOptions options = null)
        {
            return client.ApiClient.BulkOverwriteGuildApplicationCommandsAsync(guildId, Array.Empty<CreateApplicationCommandParams>(), options);
        }

        public static Task DeleteAllGlobalCommandsAsync(BaseDiscordClient client, RequestOptions options = null)
        {
            return client.ApiClient.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<CreateApplicationCommandParams>(), options);
        }

        public static async Task SendInteractionResponseAsync(BaseDiscordClient client, InteractionResponse response,
            IDiscordInteraction interaction, IMessageChannel channel = null, RequestOptions options = null)
        {
            await client.ApiClient.CreateInteractionResponseAsync(response, interaction.Id, interaction.Token, options).ConfigureAwait(false);
        }

        public static async Task SendInteractionResponseAsync(BaseDiscordClient client, UploadInteractionFileParams response,
            IDiscordInteraction interaction, IMessageChannel channel = null, RequestOptions options = null)
        {
            await client.ApiClient.CreateInteractionResponseAsync(response, interaction.Id, interaction.Token, options).ConfigureAwait(false);
        }

        public static async Task<RestInteractionMessage> GetOriginalResponseAsync(BaseDiscordClient client, IMessageChannel channel,
            IDiscordInteraction interaction, RequestOptions options = null)
        {
            var model = await client.ApiClient.GetInteractionResponseAsync(interaction.Token, options).ConfigureAwait(false);
            if(model != null)
                return RestInteractionMessage.Create(client, model, interaction.Token, channel);
            return null;
        }

        public static async Task<RestFollowupMessage> SendFollowupAsync(BaseDiscordClient client, CreateWebhookMessageParams args,
            string token, IMessageChannel channel, RequestOptions options = null)
        {
            var model = await client.ApiClient.CreateInteractionFollowupMessageAsync(args, token, options).ConfigureAwait(false);

            var entity = RestFollowupMessage.Create(client, model, token, channel);
            return entity;
        }

        public static async Task<RestFollowupMessage> SendFollowupAsync(BaseDiscordClient client, UploadWebhookFileParams args,
           string token, IMessageChannel channel, RequestOptions options = null)
        {
            var model = await client.ApiClient.CreateInteractionFollowupMessageAsync(args, token, options).ConfigureAwait(false);

            var entity = RestFollowupMessage.Create(client, model, token, channel);
            return entity;
        }
        #endregion

        #region Global commands
        public static async Task<RestGlobalCommand> GetGlobalCommandAsync(BaseDiscordClient client, ulong id,
            RequestOptions options = null)
        {
            var model = await client.ApiClient.GetGlobalApplicationCommandAsync(id, options).ConfigureAwait(false);

            return RestGlobalCommand.Create(client, model);
        }
        public static Task<ApplicationCommand> CreateGlobalCommandAsync<TArg>(BaseDiscordClient client,
            Action<TArg> func, RequestOptions options = null) where TArg : ApplicationCommandProperties
        {
            var args = Activator.CreateInstance(typeof(TArg));
            func((TArg)args);
            return CreateGlobalCommandAsync(client, (TArg)args, options);
        }
        public static async Task<ApplicationCommand> CreateGlobalCommandAsync(BaseDiscordClient client,
            ApplicationCommandProperties arg, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));

            var model = new CreateApplicationCommandParams
            {
                Name = arg.Name.Value,
                Type = arg.Type,
                DefaultPermission = arg.IsDefaultPermission.IsSpecified
                        ? arg.IsDefaultPermission.Value
                        : Optional<bool>.Unspecified,

                // TODO: better conversion to nullable optionals
                DefaultMemberPermission = arg.DefaultMemberPermissions.ToNullable(),
                DmPermission = arg.IsDMEnabled.ToNullable() 
                
            };

            if (arg is SlashCommandProperties slashProps)
            {
                Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                model.Description = slashProps.Description.Value;

                model.Options = slashProps.Options.IsSpecified
                    ? slashProps.Options.Value.Select(x => new ApplicationCommandOption(x)).ToArray()
                    : Optional<ApplicationCommandOption[]>.Unspecified;
            }

            return await client.ApiClient.CreateGlobalApplicationCommandAsync(model, options).ConfigureAwait(false);
        }

        public static async Task<ApplicationCommand[]> BulkOverwriteGlobalCommandsAsync(BaseDiscordClient client,
            ApplicationCommandProperties[] args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));

            var models = new List<CreateApplicationCommandParams>();

            foreach (var arg in args)
            {
                Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));

                var model = new CreateApplicationCommandParams
                {
                    Name = arg.Name.Value,
                    Type = arg.Type,
                    DefaultPermission = arg.IsDefaultPermission.IsSpecified
                        ? arg.IsDefaultPermission.Value
                        : Optional<bool>.Unspecified,

                    // TODO: better conversion to nullable optionals
                    DefaultMemberPermission = arg.DefaultMemberPermissions.ToNullable(),
                    DmPermission = arg.IsDMEnabled.ToNullable()
                };

                if (arg is SlashCommandProperties slashProps)
                {
                    Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                    model.Description = slashProps.Description.Value;

                    model.Options = slashProps.Options.IsSpecified
                        ? slashProps.Options.Value.Select(x => new ApplicationCommandOption(x)).ToArray()
                        : Optional<ApplicationCommandOption[]>.Unspecified;
                }

                models.Add(model);
            }

            return await client.ApiClient.BulkOverwriteGlobalApplicationCommandsAsync(models.ToArray(), options).ConfigureAwait(false);
        }

        public static async Task<IReadOnlyCollection<ApplicationCommand>> BulkOverwriteGuildCommandsAsync(BaseDiscordClient client, ulong guildId,
            ApplicationCommandProperties[] args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));

            var models = new List<CreateApplicationCommandParams>();

            foreach (var arg in args)
            {
                Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));

                var model = new CreateApplicationCommandParams
                {
                    Name = arg.Name.Value,
                    Type = arg.Type,
                    DefaultPermission = arg.IsDefaultPermission.IsSpecified
                        ? arg.IsDefaultPermission.Value
                        : Optional<bool>.Unspecified,

                    // TODO: better conversion to nullable optionals
                    DefaultMemberPermission = arg.DefaultMemberPermissions.ToNullable(),
                    DmPermission = arg.IsDMEnabled.ToNullable()
                };

                if (arg is SlashCommandProperties slashProps)
                {
                    Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                    model.Description = slashProps.Description.Value;

                    model.Options = slashProps.Options.IsSpecified
                        ? slashProps.Options.Value.Select(x => new ApplicationCommandOption(x)).ToArray()
                        : Optional<ApplicationCommandOption[]>.Unspecified;
                }

                models.Add(model);
            }

            return await client.ApiClient.BulkOverwriteGuildApplicationCommandsAsync(guildId, models.ToArray(), options).ConfigureAwait(false);
        }

        private static TArg GetApplicationCommandProperties<TArg>(IApplicationCommand command)
            where TArg : ApplicationCommandProperties
        {
            bool isBaseClass = typeof(TArg) == typeof(ApplicationCommandProperties);

            switch (true)
            {
                case true when (typeof(TArg) == typeof(SlashCommandProperties) || isBaseClass) && command.Type == ApplicationCommandType.Slash:
                    return new SlashCommandProperties() as TArg;
                case true when (typeof(TArg) == typeof(MessageCommandProperties) || isBaseClass) && command.Type == ApplicationCommandType.Message:
                    return new MessageCommandProperties() as TArg;
                case true when (typeof(TArg) == typeof(UserCommandProperties) || isBaseClass) && command.Type == ApplicationCommandType.User:
                    return new UserCommandProperties() as TArg;
                default:
                    throw new InvalidOperationException($"Cannot modify application command of type {command.Type} with the parameter type {typeof(TArg).FullName}");
            }
        }

        public static Task<ApplicationCommand> ModifyGlobalCommandAsync<TArg>(BaseDiscordClient client, IApplicationCommand command,
            Action<TArg> func, RequestOptions options = null) where TArg : ApplicationCommandProperties
        {
            var arg = GetApplicationCommandProperties<TArg>(command);
            func(arg);
            return ModifyGlobalCommandAsync(client, command, arg, options);
        }

        public static async Task<ApplicationCommand> ModifyGlobalCommandAsync(BaseDiscordClient client, IApplicationCommand command,
           ApplicationCommandProperties args, RequestOptions options = null)
        {
            if (args.Name.IsSpecified)
            {
                Preconditions.AtMost(args.Name.Value.Length, 32, nameof(args.Name));
                Preconditions.AtLeast(args.Name.Value.Length, 1, nameof(args.Name));
            }

            var model = new ModifyApplicationCommandParams
            {
                Name = args.Name,
                DefaultPermission = args.IsDefaultPermission.IsSpecified
                        ? args.IsDefaultPermission.Value
                        : Optional<bool>.Unspecified
            };

            if (args is SlashCommandProperties slashProps)
            {
                if (slashProps.Description.IsSpecified)
                {
                    Preconditions.AtMost(slashProps.Description.Value.Length, 100, nameof(slashProps.Description));
                    Preconditions.AtLeast(slashProps.Description.Value.Length, 1, nameof(slashProps.Description));
                }

                if (slashProps.Options.IsSpecified)
                {
                    if (slashProps.Options.Value.Count > 10)
                        throw new ArgumentException("Option count must be 10 or less");
                }

                model.Description = slashProps.Description;

                model.Options = slashProps.Options.IsSpecified
                    ? slashProps.Options.Value.Select(x => new ApplicationCommandOption(x)).ToArray()
                    : Optional<ApplicationCommandOption[]>.Unspecified;
            }

            return await client.ApiClient.ModifyGlobalApplicationCommandAsync(model, command.Id, options).ConfigureAwait(false);
        }

        public static async Task DeleteGlobalCommandAsync(BaseDiscordClient client, IApplicationCommand command, RequestOptions options = null)
        {
            Preconditions.NotNull(command, nameof(command));
            Preconditions.NotEqual(command.Id, 0, nameof(command.Id));

            await client.ApiClient.DeleteGlobalApplicationCommandAsync(command.Id, options).ConfigureAwait(false);
        }
        #endregion

        #region Guild Commands
        public static Task<ApplicationCommand> CreateGuildCommandAsync<TArg>(BaseDiscordClient client, ulong guildId,
            Action<TArg> func, RequestOptions options) where TArg : ApplicationCommandProperties
        {
            var args = Activator.CreateInstance(typeof(TArg));
            func((TArg)args);
            return CreateGuildCommandAsync(client, guildId, (TArg)args, options);
        }

        public static async Task<ApplicationCommand> CreateGuildCommandAsync(BaseDiscordClient client, ulong guildId,
           ApplicationCommandProperties arg, RequestOptions options = null)
        {
            var model = new CreateApplicationCommandParams
            {
                Name = arg.Name.Value,
                Type = arg.Type,
                DefaultPermission = arg.IsDefaultPermission.IsSpecified
                        ? arg.IsDefaultPermission.Value
                        : Optional<bool>.Unspecified,

                // TODO: better conversion to nullable optionals
                DefaultMemberPermission = arg.DefaultMemberPermissions.ToNullable(),
                DmPermission = arg.IsDMEnabled.ToNullable()
            };

            if (arg is SlashCommandProperties slashProps)
            {
                Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                model.Description = slashProps.Description.Value;

                model.Options = slashProps.Options.IsSpecified
                    ? slashProps.Options.Value.Select(x => new ApplicationCommandOption(x)).ToArray()
                    : Optional<ApplicationCommandOption[]>.Unspecified;
            }

            return await client.ApiClient.CreateGuildApplicationCommandAsync(model, guildId, options).ConfigureAwait(false);
        }

        public static Task<ApplicationCommand> ModifyGuildCommandAsync<TArg>(BaseDiscordClient client, IApplicationCommand command, ulong guildId,
            Action<TArg> func, RequestOptions options = null) where TArg : ApplicationCommandProperties
        {
            var arg = GetApplicationCommandProperties<TArg>(command);
            func(arg);
            return ModifyGuildCommandAsync(client, command, guildId, arg, options);
        }

        public static async Task<ApplicationCommand> ModifyGuildCommandAsync(BaseDiscordClient client, IApplicationCommand command, ulong guildId,
            ApplicationCommandProperties arg, RequestOptions options = null)
        {
            var model = new ModifyApplicationCommandParams
            {
                Name = arg.Name,
                DefaultPermission = arg.IsDefaultPermission.IsSpecified
                        ? arg.IsDefaultPermission.Value
                        : Optional<bool>.Unspecified
            };

            if (arg is SlashCommandProperties slashProps)
            {
                Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                model.Description = slashProps.Description.Value;

                model.Options = slashProps.Options.IsSpecified
                    ? slashProps.Options.Value.Select(x => new ApplicationCommandOption(x)).ToArray()
                    : Optional<ApplicationCommandOption[]>.Unspecified;
            }

            return await client.ApiClient.ModifyGuildApplicationCommandAsync(model, guildId, command.Id, options).ConfigureAwait(false);
        }

        public static async Task DeleteGuildCommandAsync(BaseDiscordClient client, ulong guildId, IApplicationCommand command, RequestOptions options = null)
        {
            Preconditions.NotNull(command, nameof(command));
            Preconditions.NotEqual(command.Id, 0, nameof(command.Id));

            await client.ApiClient.DeleteGuildApplicationCommandAsync(guildId, command.Id, options).ConfigureAwait(false);
        }

        public static Task DeleteUnknownApplicationCommandAsync(BaseDiscordClient client, ulong? guildId, IApplicationCommand command, RequestOptions options = null)
        {
            return guildId.HasValue
                ? DeleteGuildCommandAsync(client, guildId.Value, command, options)
                : DeleteGlobalCommandAsync(client, command, options);
        }
        #endregion

        #region Responses
        public static async Task<Discord.API.Message> ModifyFollowupMessageAsync(BaseDiscordClient client, RestFollowupMessage message, Action<MessageProperties> func,
            RequestOptions options = null)
        {
            var args = new MessageProperties();
            func(args);

            var embed = args.Embed;
            var embeds = args.Embeds;

            bool hasText = args.Content.IsSpecified ? !string.IsNullOrEmpty(args.Content.Value) : !string.IsNullOrEmpty(message.Content);
            bool hasEmbeds = embed.IsSpecified && embed.Value != null || embeds.IsSpecified && embeds.Value?.Length > 0 || message.Embeds.Any();
            bool hasComponents = args.Components.IsSpecified && args.Components.Value != null;

            if (!hasComponents && !hasText && !hasEmbeds)
                Preconditions.NotNullOrEmpty(args.Content.IsSpecified ? args.Content.Value : string.Empty, nameof(args.Content));

            var apiEmbeds = embed.IsSpecified || embeds.IsSpecified ? new List<API.Embed>() : null;

            if (embed.IsSpecified && embed.Value != null)
            {
                apiEmbeds.Add(embed.Value.ToModel());
            }

            if (embeds.IsSpecified && embeds.Value != null)
            {
                apiEmbeds.AddRange(embeds.Value.Select(x => x.ToModel()));
            }

            Preconditions.AtMost(apiEmbeds?.Count ?? 0, 10, nameof(args.Embeds), "A max of 10 embeds are allowed.");

            var apiArgs = new ModifyInteractionResponseParams
            {
                Content = args.Content,
                Embeds = apiEmbeds?.ToArray() ?? Optional<API.Embed[]>.Unspecified,
                AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value.ToModel() : Optional<API.AllowedMentions>.Unspecified,
                Components = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified
            };

            return await client.ApiClient.ModifyInteractionFollowupMessageAsync(apiArgs, message.Id, message.Token, options).ConfigureAwait(false);
        }
        public static async Task DeleteFollowupMessageAsync(BaseDiscordClient client, RestFollowupMessage message, RequestOptions options = null)
            => await client.ApiClient.DeleteInteractionFollowupMessageAsync(message.Id, message.Token, options);
        public static async Task<API.Message> ModifyInteractionResponseAsync(BaseDiscordClient client, string token, Action<MessageProperties> func,
           RequestOptions options = null)
        {
            var args = new MessageProperties();
            func(args);

            var embed = args.Embed;
            var embeds = args.Embeds;

            bool hasText = !string.IsNullOrEmpty(args.Content.GetValueOrDefault());
            bool hasEmbeds = embed.IsSpecified && embed.Value != null || embeds.IsSpecified && embeds.Value?.Length > 0;
            bool hasComponents = args.Components.IsSpecified && args.Components.Value != null;

            if (!hasComponents && !hasText && !hasEmbeds)
                Preconditions.NotNullOrEmpty(args.Content.IsSpecified ? args.Content.Value : string.Empty, nameof(args.Content));

            var apiEmbeds = embed.IsSpecified || embeds.IsSpecified ? new List<API.Embed>() : null;

            if (embed.IsSpecified && embed.Value != null)
            {
                apiEmbeds.Add(embed.Value.ToModel());
            }

            if (embeds.IsSpecified && embeds.Value != null)
            {
                apiEmbeds.AddRange(embeds.Value.Select(x => x.ToModel()));
            }

            Preconditions.AtMost(apiEmbeds?.Count ?? 0, 10, nameof(args.Embeds), "A max of 10 embeds are allowed.");

            if (!args.Attachments.IsSpecified)
            {
                var apiArgs = new ModifyInteractionResponseParams
                {
                    Content = args.Content,
                    Embeds = apiEmbeds?.ToArray() ?? Optional<API.Embed[]>.Unspecified,
                    AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value?.ToModel() : Optional<API.AllowedMentions>.Unspecified,
                    Components = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified,
                    Flags = args.Flags
                };

                return await client.ApiClient.ModifyInteractionResponseAsync(apiArgs, token, options).ConfigureAwait(false);
            }
            else
            {
                var apiArgs = new UploadWebhookFileParams(args.Attachments.Value.ToArray())
                {
                    Content = args.Content,
                    Embeds = apiEmbeds?.ToArray() ?? Optional<API.Embed[]>.Unspecified,
                    AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value?.ToModel() : Optional<API.AllowedMentions>.Unspecified,
                    MessageComponents = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified,
                };

                return await client.ApiClient.ModifyInteractionResponseAsync(apiArgs, token, options).ConfigureAwait(false);
            }
        }

        public static async Task DeleteInteractionResponseAsync(BaseDiscordClient client, RestInteractionMessage message, RequestOptions options = null)
            => await client.ApiClient.DeleteInteractionResponseAsync(message.Token, options);

        public static async Task DeleteInteractionResponseAsync(BaseDiscordClient client, IDiscordInteraction interaction, RequestOptions options = null)
            => await client.ApiClient.DeleteInteractionResponseAsync(interaction.Token, options);

        public static Task SendAutocompleteResultAsync(BaseDiscordClient client, IEnumerable<AutocompleteResult> result, ulong interactionId,
            string interactionToken, RequestOptions options)
        {
            result ??= Array.Empty<AutocompleteResult>();

            Preconditions.AtMost(result.Count(), 25, nameof(result), "A maximum of 25 choices are allowed!");

            var apiArgs = new InteractionResponse
            {
                Type = InteractionResponseType.ApplicationCommandAutocompleteResult,
                Data = new InteractionCallbackData
                {
                    Choices = result.Any()
                        ? result.Select(x => new ApplicationCommandOptionChoice { Name = x.Name, Value = x.Value }).ToArray()
                        : Array.Empty<ApplicationCommandOptionChoice>()
                }
            };

            return client.ApiClient.CreateInteractionResponseAsync(apiArgs, interactionId, interactionToken, options);
        }
        #endregion

        #region Guild permissions
        public static async Task<IReadOnlyCollection<GuildApplicationCommandPermission>> GetGuildCommandPermissionsAsync(BaseDiscordClient client,
            ulong guildId, RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildApplicationCommandPermissionsAsync(guildId, options);
            return models.Select(x =>
                new GuildApplicationCommandPermission(x.Id, x.ApplicationId, guildId, x.Permissions.Select(
                    y => new ApplicationCommandPermission(y.Id, y.Type, y.Permission))
                .ToArray())
            ).ToArray();
        }

        public static async Task<GuildApplicationCommandPermission> GetGuildCommandPermissionAsync(BaseDiscordClient client,
            ulong guildId, ulong commandId, RequestOptions options)
        {
            try
            {
                var model = await client.ApiClient.GetGuildApplicationCommandPermissionAsync(guildId, commandId, options);
                return new GuildApplicationCommandPermission(model.Id, model.ApplicationId, guildId, model.Permissions.Select(
                    y => new ApplicationCommandPermission(y.Id, y.Type, y.Permission)).ToArray());
            }
            catch (HttpException x)
            {
                if (x.HttpCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public static async Task<GuildApplicationCommandPermission> ModifyGuildCommandPermissionsAsync(BaseDiscordClient client, ulong guildId, ulong commandId,
            ApplicationCommandPermission[] args, RequestOptions options)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtMost(args.Length, 10, nameof(args));
            Preconditions.AtLeast(args.Length, 0, nameof(args));

            var permissionsList = new List<ApplicationCommandPermissions>();

            foreach (var arg in args)
            {
                var permissions = new ApplicationCommandPermissions
                {
                    Id = arg.TargetId,
                    Permission = arg.Permission,
                    Type = arg.TargetType
                };

                permissionsList.Add(permissions);
            }

            var model = new ModifyGuildApplicationCommandPermissionsParams
            {
                Permissions = permissionsList.ToArray()
            };

            var apiModel = await client.ApiClient.ModifyApplicationCommandPermissionsAsync(model, guildId, commandId, options);

            return new GuildApplicationCommandPermission(apiModel.Id, apiModel.ApplicationId, guildId, apiModel.Permissions.Select(
                x => new ApplicationCommandPermission(x.Id, x.Type, x.Permission)).ToArray());
        }

        public static async Task<IReadOnlyCollection<GuildApplicationCommandPermission>> BatchEditGuildCommandPermissionsAsync(BaseDiscordClient client, ulong guildId,
            IDictionary<ulong, ApplicationCommandPermission[]> args, RequestOptions options)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.Count, 0, nameof(args));

            var models = new List<ModifyGuildApplicationCommandPermissions>();

            foreach (var arg in args)
            {
                Preconditions.AtMost(arg.Value.Length, 10, nameof(args));

                var model = new ModifyGuildApplicationCommandPermissions
                {
                    Id = arg.Key,
                    Permissions = arg.Value.Select(x => new ApplicationCommandPermissions
                    {
                        Id = x.TargetId,
                        Permission = x.Permission,
                        Type = x.TargetType
                    }).ToArray()
                };

                models.Add(model);
            }

            var apiModels = await client.ApiClient.BatchModifyApplicationCommandPermissionsAsync(models.ToArray(), guildId, options);

            return apiModels.Select(
                x => new GuildApplicationCommandPermission(x.Id, x.ApplicationId, x.GuildId, x.Permissions.Select(
                    y => new ApplicationCommandPermission(y.Id, y.Type, y.Permission)).ToArray())).ToArray();
        }
        #endregion
    }
}
