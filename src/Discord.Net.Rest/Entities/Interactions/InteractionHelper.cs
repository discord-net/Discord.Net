using Discord.API;
using Discord.API.Rest;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static Task SendInteractionResponse(BaseDiscordClient client, IMessageChannel channel, InteractionResponse response,
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
        public static async Task<RestGlobalCommand> CreateGlobalCommand(BaseDiscordClient client,
            Action<SlashCommandCreationProperties> func, RequestOptions options = null)
        {
            var args = new SlashCommandCreationProperties();
            func(args);
            return await CreateGlobalCommand(client, args, options).ConfigureAwait(false);
        }
        public static async Task<RestGlobalCommand> CreateGlobalCommand(BaseDiscordClient client,
            SlashCommandCreationProperties arg, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));
            Preconditions.NotNullOrEmpty(arg.Description, nameof(arg.Description));

            if (arg.Options.IsSpecified)
                Preconditions.AtMost(arg.Options.Value.Count, 25, nameof(arg.Options));

            var model = new CreateApplicationCommandParams()
            {
                Name = arg.Name,
                Description = arg.Description,
                Options = arg.Options.IsSpecified
                    ? arg.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified,
                DefaultPermission = arg.DefaultPermission.IsSpecified
                    ? arg.DefaultPermission.Value
                    : Optional<bool>.Unspecified
            };

            var cmd = await client.ApiClient.CreateGlobalApplicationCommandAsync(model, options).ConfigureAwait(false);
            return RestGlobalCommand.Create(client, cmd);
        }

        public static async Task<IReadOnlyCollection<RestGlobalCommand>> BulkOverwriteGlobalCommands(BaseDiscordClient client,
            SlashCommandCreationProperties[] args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));

            List<CreateApplicationCommandParams> models = new List<CreateApplicationCommandParams>();

            foreach (var arg in args)
            {
                Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));
                Preconditions.NotNullOrEmpty(arg.Description, nameof(arg.Description));

                if (arg.Options.IsSpecified)
                    Preconditions.AtMost(arg.Options.Value.Count, 25, nameof(arg.Options));

                var model = new CreateApplicationCommandParams()
                {
                    Name = arg.Name,
                    Description = arg.Description,
                    Options = arg.Options.IsSpecified
                    ? arg.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified,
                    DefaultPermission = arg.DefaultPermission.IsSpecified
                    ? arg.DefaultPermission.Value
                    : Optional<bool>.Unspecified
                };

                models.Add(model);
            }

            var apiModels = await client.ApiClient.BulkOverwriteGlobalApplicationCommands(models.ToArray(), options);

            return apiModels.Select(x => RestGlobalCommand.Create(client, x)).ToArray();
        }

        public static async Task<IReadOnlyCollection<RestGuildCommand>> BulkOverwriteGuildCommands(BaseDiscordClient client, ulong guildId,
            SlashCommandCreationProperties[] args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));

            List<CreateApplicationCommandParams> models = new List<CreateApplicationCommandParams>();

            foreach (var arg in args)
            {
                Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));
                Preconditions.NotNullOrEmpty(arg.Description, nameof(arg.Description));

                if (arg.Options.IsSpecified)
                    Preconditions.AtMost(arg.Options.Value.Count, 25, nameof(arg.Options));

                var model = new CreateApplicationCommandParams()
                {
                    Name = arg.Name,
                    Description = arg.Description,
                    Options = arg.Options.IsSpecified
                    ? arg.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified,
                    DefaultPermission = arg.DefaultPermission.IsSpecified
                    ? arg.DefaultPermission.Value
                    : Optional<bool>.Unspecified
                };

                models.Add(model);
            }

            var apiModels = await client.ApiClient.BulkOverwriteGuildApplicationCommands(guildId, models.ToArray(), options);

            return apiModels.Select(x => RestGuildCommand.Create(client, x, guildId)).ToArray();
        }

        public static async Task<RestGlobalCommand> ModifyGlobalCommand(BaseDiscordClient client, RestGlobalCommand command,
           Action<ApplicationCommandProperties> func, RequestOptions options = null)
        {
            ApplicationCommandProperties args = new ApplicationCommandProperties();
            func(args);

            if (args.Name.IsSpecified)
            {
                Preconditions.AtMost(args.Name.Value.Length, 32, nameof(args.Name));
                Preconditions.AtLeast(args.Name.Value.Length, 3, nameof(args.Name));
            }
            if (args.Description.IsSpecified)
            {
                Preconditions.AtMost(args.Description.Value.Length, 100, nameof(args.Description));
                Preconditions.AtLeast(args.Description.Value.Length, 1, nameof(args.Description));
            }


            if (args.Options.IsSpecified)
            {
                if (args.Options.Value.Count > 10)
                    throw new ArgumentException("Option count must be 10 or less");
            }

            var model = new Discord.API.Rest.ModifyApplicationCommandParams()
            {
                Name = args.Name,
                Description = args.Description,
                Options = args.Options.IsSpecified
                    ? args.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified,
                DefaultPermission = args.DefaultPermission.IsSpecified
                    ? args.DefaultPermission.Value
                    : Optional<bool>.Unspecified
            };

            var msg = await client.ApiClient.ModifyGlobalApplicationCommandAsync(model, command.Id, options).ConfigureAwait(false);
            command.Update(msg);
            return command;
        }


        public static async Task DeleteGlobalCommand(BaseDiscordClient client, RestGlobalCommand command, RequestOptions options = null)
        {
            Preconditions.NotNull(command, nameof(command));
            Preconditions.NotEqual(command.Id, 0, nameof(command.Id));

            await client.ApiClient.DeleteGlobalApplicationCommandAsync(command.Id, options).ConfigureAwait(false);
        }

        // Guild Commands
        public static async Task<RestGuildCommand> CreateGuildCommand(BaseDiscordClient client, ulong guildId,
            Action<SlashCommandCreationProperties> func, RequestOptions options = null)
        {
            var args = new SlashCommandCreationProperties();
            func(args);

            return await CreateGuildCommand(client, guildId, args, options).ConfigureAwait(false);
        }
        public static async Task<RestGuildCommand> CreateGuildCommand(BaseDiscordClient client, ulong guildId,
           SlashCommandCreationProperties args, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            Preconditions.NotNullOrEmpty(args.Description, nameof(args.Description));

            Preconditions.AtMost(args.Name.Length, 32, nameof(args.Name));
            Preconditions.AtLeast(args.Name.Length, 3, nameof(args.Name));
            Preconditions.AtMost(args.Description.Length, 100, nameof(args.Description));
            Preconditions.AtLeast(args.Description.Length, 1, nameof(args.Description));

            if (args.Options.IsSpecified)
            {
                if (args.Options.Value.Count > 10)
                    throw new ArgumentException("Option count must be 10 or less");

                foreach (var item in args.Options.Value)
                {
                    Preconditions.NotNullOrEmpty(item.Name, nameof(item.Name));
                    Preconditions.NotNullOrEmpty(item.Description, nameof(item.Description));
                }
            }

            var model = new CreateApplicationCommandParams()
            {
                Name = args.Name,
                Description = args.Description,
                Options = args.Options.IsSpecified
                    ? args.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified,
                DefaultPermission = args.DefaultPermission.IsSpecified
                    ? args.DefaultPermission.Value
                    : Optional<bool>.Unspecified
            };

            var cmd = await client.ApiClient.CreateGuildApplicationCommandAsync(model, guildId, options).ConfigureAwait(false);
            return RestGuildCommand.Create(client, cmd, guildId);
        }
        public static async Task<RestGuildCommand> ModifyGuildCommand(BaseDiscordClient client, RestGuildCommand command,
           Action<ApplicationCommandProperties> func, RequestOptions options = null)
        {
            ApplicationCommandProperties args = new ApplicationCommandProperties();
            func(args);

            if (args.Name.IsSpecified)
            {
                Preconditions.AtMost(args.Name.Value.Length, 32, nameof(args.Name));
                Preconditions.AtLeast(args.Name.Value.Length, 3, nameof(args.Name));
            }
            if (args.Description.IsSpecified)
            {
                Preconditions.AtMost(args.Description.Value.Length, 100, nameof(args.Description));
                Preconditions.AtLeast(args.Description.Value.Length, 1, nameof(args.Description));
            }

            if (args.Options.IsSpecified)
            {
                if (args.Options.Value.Count > 10)
                    throw new ArgumentException("Option count must be 10 or less");
            }

            var model = new Discord.API.Rest.ModifyApplicationCommandParams()
            {
                Name = args.Name,
                Description = args.Description,
                Options = args.Options.IsSpecified
                    ? args.Options.Value.Select(x => new Discord.API.ApplicationCommandOption(x)).ToArray()
                    : Optional<Discord.API.ApplicationCommandOption[]>.Unspecified,
                DefaultPermission = args.DefaultPermission.IsSpecified
                    ? args.DefaultPermission.Value
                    : Optional<bool>.Unspecified
            };

            var msg = await client.ApiClient.ModifyGuildApplicationCommandAsync(model, command.GuildId, command.Id, options).ConfigureAwait(false);
            command.Update(msg);
            return command;
        }

        public static async Task DeleteGuildCommand(BaseDiscordClient client, ulong guildId, IApplicationCommand command, RequestOptions options = null)
        {
            Preconditions.NotNull(command, nameof(command));
            Preconditions.NotEqual(command.Id, 0, nameof(command.Id));

            await client.ApiClient.DeleteGuildApplicationCommandAsync(guildId, command.Id, options).ConfigureAwait(false);
        }

        public static async Task<Discord.API.Message> ModifyFollowupMessage(BaseDiscordClient client, RestFollowupMessage message, Action<MessageProperties> func,
            RequestOptions options = null)
        {
            var args = new MessageProperties();
            func(args);

            bool hasText = args.Content.IsSpecified ? !string.IsNullOrEmpty(args.Content.Value) : !string.IsNullOrEmpty(message.Content);
            bool hasEmbed = args.Embed.IsSpecified ? args.Embed.Value != null : message.Embeds.Any();
            if (!hasText && !hasEmbed)
                Preconditions.NotNullOrEmpty(args.Content.IsSpecified ? args.Content.Value : string.Empty, nameof(args.Content));

            var apiArgs = new API.Rest.ModifyInteractionResponseParams
            {
                Content = args.Content,
                Embeds = args.Embed.IsSpecified ? new API.Embed[] { args.Embed.Value.ToModel() } : Optional.Create<API.Embed[]>()
            };

            return await client.ApiClient.ModifyInteractionFollowupMessage(apiArgs, message.Id, message.Token, options).ConfigureAwait(false);
        }

        public static async Task DeleteFollowupMessage(BaseDiscordClient client, RestFollowupMessage message, RequestOptions options = null)
            => await client.ApiClient.DeleteInteractionFollowupMessage(message.Id, message.Token, options);

        public static async Task<Discord.API.Message> ModifyInteractionResponse(BaseDiscordClient client, RestInteractionMessage message, Action<MessageProperties> func,
           RequestOptions options = null)
        {
            var args = new MessageProperties();
            func(args);

            bool hasText = args.Content.IsSpecified ? !string.IsNullOrEmpty(args.Content.Value) : !string.IsNullOrEmpty(message.Content);
            bool hasEmbed = args.Embed.IsSpecified ? args.Embed.Value != null : message.Embeds.Any();
            if (!hasText && !hasEmbed)
                Preconditions.NotNullOrEmpty(args.Content.IsSpecified ? args.Content.Value : string.Empty, nameof(args.Content));

            var apiArgs = new API.Rest.ModifyInteractionResponseParams
            {
                Content = args.Content,
                Embeds = args.Embed.IsSpecified ? new API.Embed[] { args.Embed.Value.ToModel() } : Optional.Create<API.Embed[]>()
            };

            return await client.ApiClient.ModifyInteractionFollowupMessage(apiArgs, message.Id, message.Token, options).ConfigureAwait(false);
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

            List<ApplicationCommandPermissions> models = new List<ApplicationCommandPermissions>();

            foreach (var arg in args)
            {
                var model = new ApplicationCommandPermissions()
                {
                    Id = arg.TargetId,
                    Permission = arg.Permission,
                    Type = arg.TargetType
                };

                models.Add(model);
            }

            var apiModel = await client.ApiClient.ModifyApplicationCommandPermissions(models.ToArray(), guildId, commandId, options);

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
