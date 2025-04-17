using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions
{
    internal static class ApplicationCommandRestUtil
    {
        #region Parameters
        public static ApplicationCommandOptionProperties ToApplicationCommandOptionProps(this SlashCommandParameterInfo parameterInfo)
        {
            var localizationManager = parameterInfo.Command.Module.CommandService.LocalizationManager;
            var parameterPath = parameterInfo.GetParameterPath();

            var props = new ApplicationCommandOptionProperties
            {
                Name = parameterInfo.Name,
                Description = parameterInfo.Description,
                Type = parameterInfo.DiscordOptionType.Value,
                IsRequired = parameterInfo.IsRequired,
                Choices = parameterInfo.Choices?.Select(x => new ApplicationCommandOptionChoiceProperties
                {
                    Name = x.Name,
                    Value = x.Value,
                    NameLocalizations = localizationManager?.GetAllNames(parameterInfo.GetChoicePath(x), LocalizationTarget.Choice) ?? ImmutableDictionary<string, string>.Empty
                })?.ToList(),
                ChannelTypes = parameterInfo.ChannelTypes?.ToList(),
                IsAutocomplete = parameterInfo.IsAutocomplete,
                MaxValue = parameterInfo.MaxValue,
                MinValue = parameterInfo.MinValue,
                NameLocalizations = localizationManager?.GetAllNames(parameterPath, LocalizationTarget.Parameter) ?? ImmutableDictionary<string, string>.Empty,
                DescriptionLocalizations = localizationManager?.GetAllDescriptions(parameterPath, LocalizationTarget.Parameter) ?? ImmutableDictionary<string, string>.Empty,
                MinLength = parameterInfo.MinLength,
                MaxLength = parameterInfo.MaxLength,
            };

            parameterInfo.TypeConverter.Write(props, parameterInfo);

            return props;
        }
        #endregion

        #region Commands

        public static SlashCommandProperties ToApplicationCommandProps(this SlashCommandInfo commandInfo)
        {
            var commandPath = commandInfo.GetCommandPath();
            var localizationManager = commandInfo.Module.CommandService.LocalizationManager;

            var props = new SlashCommandBuilder()
            {
                Name = commandInfo.Name,
                Description = commandInfo.Description,
                IsDefaultPermission = commandInfo.DefaultPermission,
#pragma warning disable CS0618 // Type or member is obsolete
                IsDMEnabled = commandInfo.IsEnabledInDm,
#pragma warning restore CS0618 // Type or member is obsolete
                IsNsfw = commandInfo.IsNsfw,
                DefaultMemberPermissions = ((commandInfo.DefaultMemberPermissions ?? 0) | (commandInfo.Module.DefaultMemberPermissions ?? 0)).SanitizeGuildPermissions(),
                IntegrationTypes = commandInfo.IntegrationTypes is not null
                    ? new HashSet<ApplicationIntegrationType>(commandInfo.IntegrationTypes)
                    : null,
                ContextTypes = commandInfo.ContextTypes is not null
                    ? new HashSet<InteractionContextType>(commandInfo.ContextTypes)
                    : null,
            }.WithNameLocalizations(localizationManager?.GetAllNames(commandPath, LocalizationTarget.Command) ?? ImmutableDictionary<string, string>.Empty)
            .WithDescriptionLocalizations(localizationManager?.GetAllDescriptions(commandPath, LocalizationTarget.Command) ?? ImmutableDictionary<string, string>.Empty)
            .Build();

            if (commandInfo.Parameters.Count > SlashCommandBuilder.MaxOptionsCount)
                throw new InvalidOperationException($"Slash Commands cannot have more than {SlashCommandBuilder.MaxOptionsCount} command parameters");

            props.Options = commandInfo.FlattenedParameters.Select(x => x.ToApplicationCommandOptionProps())?.ToList() ?? Optional<List<ApplicationCommandOptionProperties>>.Unspecified;

            return props;
        }

        public static ApplicationCommandOptionProperties ToApplicationCommandOptionProps(this SlashCommandInfo commandInfo)
        {
            var localizationManager = commandInfo.Module.CommandService.LocalizationManager;
            var commandPath = commandInfo.GetCommandPath();

            return new ApplicationCommandOptionProperties
            {
                Name = commandInfo.Name,
                Description = commandInfo.Description,
                Type = ApplicationCommandOptionType.SubCommand,
                IsRequired = false,
                Options = commandInfo.FlattenedParameters?.Select(x => x.ToApplicationCommandOptionProps())
                    ?.ToList(),
                NameLocalizations = localizationManager?.GetAllNames(commandPath, LocalizationTarget.Command) ?? ImmutableDictionary<string, string>.Empty,
                DescriptionLocalizations = localizationManager?.GetAllDescriptions(commandPath, LocalizationTarget.Command) ?? ImmutableDictionary<string, string>.Empty, 
            };
        }

        public static ApplicationCommandProperties ToApplicationCommandProps(this ContextCommandInfo commandInfo)
        {
            var localizationManager = commandInfo.Module.CommandService.LocalizationManager;
            var commandPath = commandInfo.GetCommandPath();

            return commandInfo.CommandType switch
            {
                ApplicationCommandType.Message => new MessageCommandBuilder
                {
                    Name = commandInfo.Name,
                    IsDefaultPermission = commandInfo.DefaultPermission,
                    DefaultMemberPermissions = ((commandInfo.DefaultMemberPermissions ?? 0) | (commandInfo.Module.DefaultMemberPermissions ?? 0)).SanitizeGuildPermissions(),
#pragma warning disable CS0618 // Type or member is obsolete
                    IsDMEnabled = commandInfo.IsEnabledInDm,
#pragma warning restore CS0618 // Type or member is obsolete
                    IsNsfw = commandInfo.IsNsfw,
                    IntegrationTypes = commandInfo.IntegrationTypes is not null
                        ? new HashSet<ApplicationIntegrationType>(commandInfo.IntegrationTypes)
                        : null,
                    ContextTypes = commandInfo.ContextTypes is not null
                        ? new HashSet<InteractionContextType>(commandInfo.ContextTypes)
                        : null,
                }
                .WithNameLocalizations(localizationManager?.GetAllNames(commandPath, LocalizationTarget.Command) ?? ImmutableDictionary<string, string>.Empty)
                .Build(),
                ApplicationCommandType.User => new UserCommandBuilder
                {
                    Name = commandInfo.Name,
                    IsDefaultPermission = commandInfo.DefaultPermission,
                    DefaultMemberPermissions = ((commandInfo.DefaultMemberPermissions ?? 0) | (commandInfo.Module.DefaultMemberPermissions ?? 0)).SanitizeGuildPermissions(),
                    IsNsfw = commandInfo.IsNsfw,
#pragma warning disable CS0618 // Type or member is obsolete
                    IsDMEnabled = commandInfo.IsEnabledInDm,
#pragma warning restore CS0618 // Type or member is obsolete
                    IntegrationTypes = commandInfo.IntegrationTypes is not null
                        ? new HashSet<ApplicationIntegrationType>(commandInfo.IntegrationTypes)
                        : null,
                    ContextTypes = commandInfo.ContextTypes is not null
                        ? new HashSet<InteractionContextType>(commandInfo.ContextTypes)
                        : null,
                }
                .WithNameLocalizations(localizationManager?.GetAllNames(commandPath, LocalizationTarget.Command) ?? ImmutableDictionary<string, string>.Empty)
                .Build(),
                _ => throw new InvalidOperationException($"{commandInfo.CommandType} isn't a supported command type.")
            };
        }
        #endregion

        #region Modules

        public static IReadOnlyCollection<ApplicationCommandProperties> ToApplicationCommandProps(this ModuleInfo moduleInfo, bool ignoreDontRegister = false)
        {
            var args = new List<ApplicationCommandProperties>();

            moduleInfo.ParseModuleModel(args, ignoreDontRegister);
            return args;
        }

        private static void ParseModuleModel(this ModuleInfo moduleInfo, List<ApplicationCommandProperties> args, bool ignoreDontRegister)
        {
            if (moduleInfo.DontAutoRegister && !ignoreDontRegister)
                return;

            args.AddRange(moduleInfo.ContextCommands?.Select(x => x.ToApplicationCommandProps()));

            if (!moduleInfo.IsSlashGroup)
            {
                args.AddRange(moduleInfo.SlashCommands?.Select(x => x.ToApplicationCommandProps()));

                foreach (var submodule in moduleInfo.SubModules)
                    submodule.ParseModuleModel(args, ignoreDontRegister);
            }
            else
            {
                var options = new List<ApplicationCommandOptionProperties>();

                foreach (var command in moduleInfo.SlashCommands)
                {
                    if (command.IgnoreGroupNames)
                        args.Add(command.ToApplicationCommandProps());
                    else
                        options.Add(command.ToApplicationCommandOptionProps());
                }

                options.AddRange(moduleInfo.SubModules?.SelectMany(x => x.ParseSubModule(args, ignoreDontRegister)));

                var localizationManager = moduleInfo.CommandService.LocalizationManager;
                var modulePath = moduleInfo.GetModulePath();

                var props = new SlashCommandBuilder
                {
                    Name = moduleInfo.SlashGroupName,
                    Description = moduleInfo.Description,
#pragma warning disable CS0618 // Type or member is obsolete
                    IsDefaultPermission = moduleInfo.DefaultPermission,
                    IsDMEnabled = moduleInfo.IsEnabledInDm,
#pragma warning restore CS0618 // Type or member is obsolete
                    IsNsfw = moduleInfo.IsNsfw,
                    DefaultMemberPermissions = moduleInfo.DefaultMemberPermissions,
                    IntegrationTypes = moduleInfo.IntegrationTypes is not null
                        ? new HashSet<ApplicationIntegrationType>(moduleInfo.IntegrationTypes)
                        : null,
                    ContextTypes = moduleInfo.ContextTypes is not null
                        ? new HashSet<InteractionContextType>(moduleInfo.ContextTypes)
                        : null,
                }
                .WithNameLocalizations(localizationManager?.GetAllNames(modulePath, LocalizationTarget.Group) ?? ImmutableDictionary<string, string>.Empty)
                .WithDescriptionLocalizations(localizationManager?.GetAllDescriptions(modulePath, LocalizationTarget.Group) ?? ImmutableDictionary<string, string>.Empty)
                .Build();

                if (options.Count > SlashCommandBuilder.MaxOptionsCount)
                    throw new InvalidOperationException($"Slash Commands cannot have more than {SlashCommandBuilder.MaxOptionsCount} command parameters");

                props.Options = options;

                args.Add(props);
            }
        }

        private static IReadOnlyCollection<ApplicationCommandOptionProperties> ParseSubModule(this ModuleInfo moduleInfo, List<ApplicationCommandProperties> args,
            bool ignoreDontRegister)
        {
            if (moduleInfo.DontAutoRegister && !ignoreDontRegister)
                return Array.Empty<ApplicationCommandOptionProperties>();

            args.AddRange(moduleInfo.ContextCommands?.Select(x => x.ToApplicationCommandProps()));

            var options = new List<ApplicationCommandOptionProperties>();
            options.AddRange(moduleInfo.SubModules?.SelectMany(x => x.ParseSubModule(args, ignoreDontRegister)));

            foreach (var command in moduleInfo.SlashCommands)
            {
                if (command.IgnoreGroupNames)
                    args.Add(command.ToApplicationCommandProps());
                else
                    options.Add(command.ToApplicationCommandOptionProps());
            }

            if (!moduleInfo.IsSlashGroup)
                return options;
            else
                return new List<ApplicationCommandOptionProperties>() { new ApplicationCommandOptionProperties
                {
                    Name = moduleInfo.SlashGroupName,
                    Description = moduleInfo.Description,
                    Type = ApplicationCommandOptionType.SubCommandGroup,
                    Options = options,
                    NameLocalizations = moduleInfo.CommandService.LocalizationManager?.GetAllNames(moduleInfo.GetModulePath(), LocalizationTarget.Group)
                        ?? ImmutableDictionary<string, string>.Empty,
                    DescriptionLocalizations = moduleInfo.CommandService.LocalizationManager?.GetAllDescriptions(moduleInfo.GetModulePath(), LocalizationTarget.Group)
                        ?? ImmutableDictionary<string, string>.Empty,
                } };
        }

        #endregion

        public static ApplicationCommandProperties ToApplicationCommandProps(this IApplicationCommand command)
        {
            return command.Type switch
            {
                ApplicationCommandType.Slash => new SlashCommandProperties
                {
                    Name = command.Name,
                    Description = command.Description,
                    IsDefaultPermission = command.IsDefaultPermission,
                    DefaultMemberPermissions = command.DefaultMemberPermissions.RawValue == 0 ? new Optional<GuildPermission>() : (GuildPermission)command.DefaultMemberPermissions.RawValue,
#pragma warning disable CS0618 // Type or member is obsolete
                    IsDMEnabled = command.IsEnabledInDm,
#pragma warning restore CS0618 // Type or member is obsolete
                    IsNsfw = command.IsNsfw,
                    Options = command.Options?.Select(x => x.ToApplicationCommandOptionProps())?.ToList() ?? Optional<List<ApplicationCommandOptionProperties>>.Unspecified,
                    NameLocalizations = command.NameLocalizations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty,
                    DescriptionLocalizations = command.DescriptionLocalizations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty,
                    ContextTypes = command.ContextTypes is not null
                        ? new HashSet<InteractionContextType>(command.ContextTypes)
                        : Optional<HashSet<InteractionContextType>>.Unspecified,
                    IntegrationTypes = command.IntegrationTypes is not null
                        ? new HashSet<ApplicationIntegrationType>(command.IntegrationTypes)
                        : Optional<HashSet<ApplicationIntegrationType>>.Unspecified,
                },
                ApplicationCommandType.User => new UserCommandProperties
                {
                    Name = command.Name,
                    IsDefaultPermission = command.IsDefaultPermission,
                    DefaultMemberPermissions = command.DefaultMemberPermissions.RawValue == 0 ? new Optional<GuildPermission>() : (GuildPermission)command.DefaultMemberPermissions.RawValue,
                    IsNsfw = command.IsNsfw,
#pragma warning disable CS0618 // Type or member is obsolete
                    IsDMEnabled = command.IsEnabledInDm,
#pragma warning restore CS0618 // Type or member is obsolete
                    NameLocalizations = command.NameLocalizations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty,
                    DescriptionLocalizations = command.DescriptionLocalizations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty,
                    ContextTypes = command.ContextTypes is not null
                        ? new HashSet<InteractionContextType>(command.ContextTypes)
                        : Optional<HashSet<InteractionContextType>>.Unspecified,
                    IntegrationTypes = command.IntegrationTypes is not null
                        ? new HashSet<ApplicationIntegrationType>(command.IntegrationTypes)
                        : Optional<HashSet<ApplicationIntegrationType>>.Unspecified,
                },
                ApplicationCommandType.Message => new MessageCommandProperties
                {
                    Name = command.Name,
                    IsDefaultPermission = command.IsDefaultPermission,
                    DefaultMemberPermissions = command.DefaultMemberPermissions.RawValue == 0 ? new Optional<GuildPermission>() : (GuildPermission)command.DefaultMemberPermissions.RawValue,
                    IsNsfw = command.IsNsfw,
#pragma warning disable CS0618 // Type or member is obsolete
                    IsDMEnabled = command.IsEnabledInDm,
#pragma warning restore CS0618 // Type or member is obsolete
                    NameLocalizations = command.NameLocalizations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty,
                    DescriptionLocalizations = command.DescriptionLocalizations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty,
                    ContextTypes = command.ContextTypes is not null
                        ? new HashSet<InteractionContextType>(command.ContextTypes)
                        : Optional<HashSet<InteractionContextType>>.Unspecified,
                    IntegrationTypes = command.IntegrationTypes is not null
                        ? new HashSet<ApplicationIntegrationType>(command.IntegrationTypes)
                        : Optional<HashSet<ApplicationIntegrationType>>.Unspecified,
                },
                _ => throw new InvalidOperationException($"Cannot create command properties for command type {command.Type}"),
            };
        }

        public static ApplicationCommandOptionProperties ToApplicationCommandOptionProps(this IApplicationCommandOption commandOption) =>
            new ApplicationCommandOptionProperties
            {
                Name = commandOption.Name,
                Description = commandOption.Description,
                Type = commandOption.Type,
                IsRequired = commandOption.IsRequired,
                ChannelTypes = commandOption.ChannelTypes?.ToList(),
                IsAutocomplete = commandOption.IsAutocomplete.GetValueOrDefault(),
                MinValue = commandOption.MinValue,
                MaxValue = commandOption.MaxValue,
                Choices = commandOption.Choices?.Select(x => new ApplicationCommandOptionChoiceProperties
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToList(),
                Options = commandOption.Options?.Select(x => x.ToApplicationCommandOptionProps()).ToList(),
                NameLocalizations = commandOption.NameLocalizations?.ToImmutableDictionary(),
                DescriptionLocalizations = commandOption.DescriptionLocalizations?.ToImmutableDictionary(),
                MaxLength = commandOption.MaxLength,
                MinLength = commandOption.MinLength,
            };

        public static Modal ToModal(this ModalInfo modalInfo, string customId, Action<ModalBuilder> modifyModal = null)
        {
            var builder = new ModalBuilder(modalInfo.Title, customId);

            foreach (var input in modalInfo.Components)
                switch (input)
                {
                    case TextInputComponentInfo textComponent:
                        builder.AddTextInput(textComponent.Label, textComponent.CustomId, textComponent.Style, textComponent.Placeholder, textComponent.IsRequired ? textComponent.MinLength : null,
                            textComponent.MaxLength, textComponent.IsRequired, textComponent.InitialValue);
                        break;
                    default:
                        throw new InvalidOperationException($"{input.GetType().FullName} isn't a valid component info class");
                }

            modifyModal?.Invoke(builder);

            return builder.Build();
        }

        public static GuildPermission? SanitizeGuildPermissions(this GuildPermission permissions) =>
            permissions == 0 ? null : permissions;
    }
}
