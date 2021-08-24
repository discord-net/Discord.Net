using Discord.API;
using Discord.API.Rest;
using Discord.Net;
//using Discord.Rest.Entities.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class InteractionHelper
    {
        public static Task DeleteAllGuildCommandsAsync(BaseDiscordClient client, ulong guildId, RequestOptions options = null)
        {
            return client.ApiClient.BulkOverwriteGuildApplicationCommands(guildId, new CreateApplicationCommandParams[0], options);
        }

        public static Task DeleteAllGlobalCommandsAsync(BaseDiscordClient client, RequestOptions options = null)
        {
            return client.ApiClient.BulkOverwriteGlobalApplicationCommands(new CreateApplicationCommandParams[0], options);
        }

        public static Task SendInteractionResponse(BaseDiscordClient client, InteractionResponse response,
            ulong interactionId, string interactionToken, RequestOptions options = null)
        {
            return client.ApiClient.CreateInteractionResponse(response, interactionId, interactionToken, options);
        }

        public static async Task<RestInteractionMessage> GetOriginalResponseAsync(BaseDiscordClient client, IMessageChannel channel,
            IDiscordInteraction interaction, RequestOptions options = null)
        {
            var model = await client.ApiClient.GetInteractionResponse(interaction.Token, options).ConfigureAwait(false);
            return RestInteractionMessage.Create(client, model, interaction.Token, channel);
        }

        public static async Task<RestFollowupMessage> SendFollowupAsync(BaseDiscordClient client, CreateWebhookMessageParams args,
            string token, IMessageChannel channel, RequestOptions options = null)
        {
            var model = await client.ApiClient.CreateInteractionFollowupMessage(args, token, options).ConfigureAwait(false);

            RestFollowupMessage entity = RestFollowupMessage.Create(client, model, token, channel);
            return entity;
        }

        // Global commands
        public static async Task<RestGlobalCommand> GetGlobalCommandAsync(BaseDiscordClient client, ulong id,
            RequestOptions options = null)
        {
            var model = await client.ApiClient.GetGlobalApplicationCommandAsync(id, options).ConfigureAwait(false);
            

            return RestGlobalCommand.Create(client, model);
        }
        public static Task<ApplicationCommand> CreateGlobalCommand<TArg>(BaseDiscordClient client,
            Action<TArg> func, RequestOptions options = null) where TArg : ApplicationCommandProperties
        {
            var args = Activator.CreateInstance(typeof(TArg));
            func((TArg)args);
            return CreateGlobalCommand(client, (TArg)args, options);
        }
        public static async Task<ApplicationCommand> CreateGlobalCommand(BaseDiscordClient client,
            ApplicationCommandProperties arg, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));

            var model = new CreateApplicationCommandParams()
            {
                Name = arg.Name.Value,
                Type = arg.Type,
            };

            if (arg is SlashCommandProperties slashProps)
            {
                Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                model.Description = slashProps.Description.Value;

                model.Options = slashProps.Options.IsSpecified
                    ? slashProps.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified;

                model.DefaultPermission = slashProps.DefaultPermission.IsSpecified
                    ? slashProps.DefaultPermission.Value
                    : Optional<bool>.Unspecified;
            }

            return await client.ApiClient.CreateGlobalApplicationCommandAsync(model, options).ConfigureAwait(false);
        }

        public static async Task<ApplicationCommand[]> BulkOverwriteGlobalCommands(BaseDiscordClient client,
            ApplicationCommandProperties[] args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));

            List<CreateApplicationCommandParams> models = new List<CreateApplicationCommandParams>();

            foreach (var arg in args)
            {
                Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));

                var model = new CreateApplicationCommandParams()
                {
                    Name = arg.Name.Value,
                    Type = arg.Type,
                };

                if (arg is SlashCommandProperties slashProps)
                {
                    Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                    model.Description = slashProps.Description.Value;

                    model.Options = slashProps.Options.IsSpecified
                        ? slashProps.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                        : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified;

                    model.DefaultPermission = slashProps.DefaultPermission.IsSpecified
                        ? slashProps.DefaultPermission.Value
                        : Optional<bool>.Unspecified;
                }

                models.Add(model);
            }

           return await client.ApiClient.BulkOverwriteGlobalApplicationCommands(models.ToArray(), options).ConfigureAwait(false);
        }

        public static async Task<IReadOnlyCollection<ApplicationCommand>> BulkOverwriteGuildCommands(BaseDiscordClient client, ulong guildId,
            ApplicationCommandProperties[] args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));

            List<CreateApplicationCommandParams> models = new List<CreateApplicationCommandParams>();

            foreach (var arg in args)
            {
                Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));

                var model = new CreateApplicationCommandParams()
                {
                    Name = arg.Name.Value,
                    Type = arg.Type,
                };

                if (arg is SlashCommandProperties slashProps)
                {
                    Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                    model.Description = slashProps.Description.Value;

                    model.Options = slashProps.Options.IsSpecified
                        ? slashProps.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                        : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified;

                    model.DefaultPermission = slashProps.DefaultPermission.IsSpecified
                        ? slashProps.DefaultPermission.Value
                        : Optional<bool>.Unspecified;
                }

                models.Add(model);
            }

            return await client.ApiClient.BulkOverwriteGuildApplicationCommands(guildId, models.ToArray(), options).ConfigureAwait(false);
        }

        public static Task<ApplicationCommand> ModifyGlobalCommand<TArg>(BaseDiscordClient client, IApplicationCommand command,
           Action<TArg> func, RequestOptions options = null) where TArg : ApplicationCommandProperties
        {
            var arg = (TArg)Activator.CreateInstance(typeof(TArg));
            func(arg);
            return ModifyGlobalCommand(client, command, arg, options);
        }

        public static async Task<ApplicationCommand> ModifyGlobalCommand(BaseDiscordClient client, IApplicationCommand command,
           ApplicationCommandProperties args, RequestOptions options = null)
        {
            if (args.Name.IsSpecified)
            {
                Preconditions.AtMost(args.Name.Value.Length, 32, nameof(args.Name));
                Preconditions.AtLeast(args.Name.Value.Length, 3, nameof(args.Name));
            }

            var model = new Discord.API.Rest.ModifyApplicationCommandParams()
            {
                Name = args.Name,
            };

            if(args is SlashCommandProperties slashProps)
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
                    ? slashProps.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified;

                model.DefaultPermission = slashProps.DefaultPermission.IsSpecified
                    ? slashProps.DefaultPermission.Value
                    : Optional<bool>.Unspecified;
            }

            return await client.ApiClient.ModifyGlobalApplicationCommandAsync(model, command.Id, options).ConfigureAwait(false);
        }


        public static async Task DeleteGlobalCommand(BaseDiscordClient client, IApplicationCommand command, RequestOptions options = null)
        {
            Preconditions.NotNull(command, nameof(command));
            Preconditions.NotEqual(command.Id, 0, nameof(command.Id));

            await client.ApiClient.DeleteGlobalApplicationCommandAsync(command.Id, options).ConfigureAwait(false);
        }

        // Guild Commands
        public static Task<ApplicationCommand> CreateGuildCommand<TArg>(BaseDiscordClient client, ulong guildId,
            Action<TArg> func, RequestOptions options) where TArg : ApplicationCommandProperties
        {
            var args = Activator.CreateInstance(typeof(TArg));
            func((TArg)args);
            return CreateGuildCommand(client, guildId, (TArg)args, options);
        }

        public static async Task<ApplicationCommand> CreateGuildCommand(BaseDiscordClient client, ulong guildId,
           ApplicationCommandProperties arg, RequestOptions options = null)
        {
            var model = new CreateApplicationCommandParams()
            {
                Name = arg.Name.Value,
                Type = arg.Type,
            };

            if (arg is SlashCommandProperties slashProps)
            {
                Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                model.Description = slashProps.Description.Value;

                model.Options = slashProps.Options.IsSpecified
                    ? slashProps.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified;

                model.DefaultPermission = slashProps.DefaultPermission.IsSpecified
                    ? slashProps.DefaultPermission.Value
                    : Optional<bool>.Unspecified;
            }

            return await client.ApiClient.CreateGuildApplicationCommandAsync(model, guildId, options).ConfigureAwait(false);
        }

        public static Task<ApplicationCommand> ModifyGuildCommand<TArg>(BaseDiscordClient client, IApplicationCommand command, ulong guildId,
           Action<TArg> func, RequestOptions options = null) where TArg : ApplicationCommandProperties
        {
            var arg = (TArg)Activator.CreateInstance(typeof(TArg));
            func(arg);
            return ModifyGuildCommand(client, command, guildId, arg, options);
        }

        public static async Task<ApplicationCommand> ModifyGuildCommand(BaseDiscordClient client, IApplicationCommand command, ulong guildId,
            ApplicationCommandProperties arg, RequestOptions options = null)
        {
            var model = new ModifyApplicationCommandParams()
            {
                Name = arg.Name.Value,
            };

            if (arg is SlashCommandProperties slashProps)
            {
                Preconditions.NotNullOrEmpty(slashProps.Description, nameof(slashProps.Description));

                model.Description = slashProps.Description.Value;

                model.Options = slashProps.Options.IsSpecified
                    ? slashProps.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified;

                model.DefaultPermission = slashProps.DefaultPermission.IsSpecified
                    ? slashProps.DefaultPermission.Value
                    : Optional<bool>.Unspecified;
            }

            return await client.ApiClient.ModifyGuildApplicationCommandAsync(model, guildId, command.Id, options).ConfigureAwait(false);
        }

        public static async Task DeleteGuildCommand(BaseDiscordClient client, ulong guildId, IApplicationCommand command, RequestOptions options = null)
        {
            Preconditions.NotNull(command, nameof(command));
            Preconditions.NotEqual(command.Id, 0, nameof(command.Id));

            await client.ApiClient.DeleteGuildApplicationCommandAsync(guildId, command.Id, options).ConfigureAwait(false);
        }

        public static Task DeleteUnknownApplicationCommand(BaseDiscordClient client, ulong? guildId, IApplicationCommand command, RequestOptions options = null)
        {
            if (guildId.HasValue)
            {
                return DeleteGuildCommand(client, guildId.Value, command, options);
            }
            else
            {
                return DeleteGlobalCommand(client, command, options);
            }
        }

        // Responses
        public static async Task<Discord.API.Message> ModifyFollowupMessage(BaseDiscordClient client, RestFollowupMessage message, Action<MessageProperties> func,
            RequestOptions options = null)
        {
            var args = new MessageProperties();
            func(args);

            var embed = args.Embed;
            var embeds = args.Embeds;

            bool hasText = args.Content.IsSpecified ? !string.IsNullOrEmpty(args.Content.Value) : !string.IsNullOrEmpty(message.Content);
            bool hasEmbeds = (embed.IsSpecified && embed.Value != null) || (embeds.IsSpecified && embeds.Value?.Length > 0) || message.Embeds.Any();

            if (!hasText && !hasEmbeds)
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

            var apiArgs = new API.Rest.ModifyInteractionResponseParams
            {
                Content = args.Content,
                Embeds = apiEmbeds?.ToArray() ?? Optional<API.Embed[]>.Unspecified,
                AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value.ToModel() : Optional<API.AllowedMentions>.Unspecified,
                Components = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified,
            };

            return await client.ApiClient.ModifyInteractionFollowupMessage(apiArgs, message.Id, message.Token, options).ConfigureAwait(false);
        }

        public static async Task DeleteFollowupMessage(BaseDiscordClient client, RestFollowupMessage message, RequestOptions options = null)
            => await client.ApiClient.DeleteInteractionFollowupMessage(message.Id, message.Token, options);

        public static async Task<Message> ModifyInteractionResponse(BaseDiscordClient client, string token, Action<MessageProperties> func,
           RequestOptions options = null)
        {
            var args = new MessageProperties();
            func(args);

            var embed = args.Embed;
            var embeds = args.Embeds;

            bool hasText = !string.IsNullOrEmpty(args.Content.GetValueOrDefault());
            bool hasEmbeds = (embed.IsSpecified && embed.Value != null) || (embeds.IsSpecified && embeds.Value?.Length > 0);

            if (!hasText && !hasEmbeds)
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
                AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value?.ToModel() : Optional<API.AllowedMentions>.Unspecified,
                Components = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified,
                Flags = args.Flags
            };

            return await client.ApiClient.ModifyInteractionResponse(apiArgs, token, options).ConfigureAwait(false);
        }

        public static async Task DeletedInteractionResponse(BaseDiscordClient client, RestInteractionMessage message, RequestOptions options = null)
            => await client.ApiClient.DeleteInteractionFollowupMessage(message.Id, message.Token, options);

        // Guild permissions
        public static async Task<IReadOnlyCollection<GuildApplicationCommandPermission>> GetGuildCommandPermissionsAsync(BaseDiscordClient client,
            ulong guildId, RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildApplicationCommandPermissions(guildId, options);
            return models.Select(x =>
                new GuildApplicationCommandPermission(x.Id, x.ApplicationId, guildId, x.Permissions.Select(
                    y => new Discord.ApplicationCommandPermission(y.Id, y.Type, y.Permission))
                .ToArray())
            ).ToArray();
        }

        public static async Task<GuildApplicationCommandPermission> GetGuildCommandPermissionAsync(BaseDiscordClient client,
            ulong guildId, ulong commandId, RequestOptions options)
        {
            try
            {
                var model = await client.ApiClient.GetGuildApplicationCommandPermission(guildId, commandId, options);
                return new GuildApplicationCommandPermission(model.Id, model.ApplicationId, guildId, model.Permissions.Select(
                    y => new ApplicationCommandPermission(y.Id, y.Type, y.Permission)).ToArray());
            }
            catch (HttpException x)
            {
                if (x.HttpCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public static async Task<GuildApplicationCommandPermission> ModifyGuildCommandPermissionsAsync(BaseDiscordClient client, ulong guildId, ulong commandId,
            ApplicationCommandPermission[] args, RequestOptions options)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtMost(args.Length, 10, nameof(args));
            Preconditions.GreaterThan(args.Length, 0, nameof(args));

            List<ApplicationCommandPermissions> permissionsList = new List<ApplicationCommandPermissions>();

            foreach (var arg in args)
            {
                var permissions = new ApplicationCommandPermissions()
                {
                    Id = arg.TargetId,
                    Permission = arg.Permission,
                    Type = arg.TargetType
                };

                permissionsList.Add(permissions);
            }

            ModifyGuildApplicationCommandPermissionsParams model = new ModifyGuildApplicationCommandPermissionsParams()
            {
                Permissions = permissionsList.ToArray()
            };

            var apiModel = await client.ApiClient.ModifyApplicationCommandPermissions(model, guildId, commandId, options);

            return new GuildApplicationCommandPermission(apiModel.Id, apiModel.ApplicationId, guildId, apiModel.Permissions.Select(
                x => new ApplicationCommandPermission(x.Id, x.Type, x.Permission)).ToArray());
        }

        public static async Task<IReadOnlyCollection<GuildApplicationCommandPermission>> BatchEditGuildCommandPermissionsAsync(BaseDiscordClient client, ulong guildId,
            IDictionary<ulong, ApplicationCommandPermission[]> args, RequestOptions options)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.Count, 0, nameof(args));

            List<ModifyGuildApplicationCommandPermissions> models = new List<ModifyGuildApplicationCommandPermissions>();

            foreach (var arg in args)
            {
                Preconditions.AtMost(arg.Value.Length, 10, nameof(args));

                var model = new ModifyGuildApplicationCommandPermissions()
                {
                    Id = arg.Key,
                    Permissions = arg.Value.Select(x => new ApplicationCommandPermissions()
                    {
                        Id = x.TargetId,
                        Permission = x.Permission,
                        Type = x.TargetType
                    }).ToArray()
                };

                models.Add(model);
            }

            var apiModels = await client.ApiClient.BatchModifyApplicationCommandPermissions(models.ToArray(), guildId, options);

            return apiModels.Select(
                x => new GuildApplicationCommandPermission(x.Id, x.ApplicationId, x.GuildId, x.Permissions.Select(
                    y => new ApplicationCommandPermission(y.Id, y.Type, y.Permission)).ToArray())).ToArray();
        }
    }
}
