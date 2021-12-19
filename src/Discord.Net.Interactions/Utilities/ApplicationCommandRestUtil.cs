using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions
{
    internal static class ApplicationCommandRestUtil
    {
        #region Parameters
        public static ApplicationCommandOptionProperties ToApplicationCommandOptionProps(this SlashCommandParameterInfo parameterInfo)
        {
            var props = new ApplicationCommandOptionProperties
            {
                Name = parameterInfo.Name,
                Description = parameterInfo.Description,
                Type = parameterInfo.DiscordOptionType,
                IsRequired = parameterInfo.IsRequired,
                Choices = parameterInfo.Choices?.Select(x => new ApplicationCommandOptionChoiceProperties
                {
                    Name = x.Name,
                    Value = x.Value
                })?.ToList(),
                ChannelTypes = parameterInfo.ChannelTypes?.ToList(),
                IsAutocomplete = parameterInfo.IsAutocomplete,
                MaxValue = parameterInfo.MaxValue,
                MinValue = parameterInfo.MinValue
            };

            parameterInfo.TypeConverter.Write(props, parameterInfo);

            return props;
        }
        #endregion

        #region Commands

        public static SlashCommandProperties ToApplicationCommandProps(this SlashCommandInfo commandInfo)
        {
            var props = new SlashCommandBuilder()
            {
                Name = commandInfo.Name,
                Description = commandInfo.Description,
                IsDefaultPermission = commandInfo.DefaultPermission,
            }.Build();

            if (commandInfo.Parameters.Count > SlashCommandBuilder.MaxOptionsCount)
                throw new InvalidOperationException($"Slash Commands cannot have more than {SlashCommandBuilder.MaxOptionsCount} command parameters");

            props.Options = commandInfo.Parameters.Select(x => x.ToApplicationCommandOptionProps())?.ToList() ?? Optional<List<ApplicationCommandOptionProperties>>.Unspecified;

            return props;
        }

        public static ApplicationCommandOptionProperties ToApplicationCommandOptionProps(this SlashCommandInfo commandInfo) =>
            new ApplicationCommandOptionProperties
            {
                Name = commandInfo.Name,
                Description = commandInfo.Description,
                Type = ApplicationCommandOptionType.SubCommand,
                IsRequired = false,
                Options = commandInfo.Parameters?.Select(x => x.ToApplicationCommandOptionProps())?.ToList()
            };

        public static ApplicationCommandProperties ToApplicationCommandProps(this ContextCommandInfo commandInfo)
            => commandInfo.CommandType switch
            {
                ApplicationCommandType.Message => new MessageCommandBuilder { Name = commandInfo.Name, IsDefaultPermission = commandInfo.DefaultPermission}.Build(),
                ApplicationCommandType.User => new UserCommandBuilder { Name = commandInfo.Name, IsDefaultPermission=commandInfo.DefaultPermission}.Build(),
                _ => throw new InvalidOperationException($"{commandInfo.CommandType} isn't a supported command type.")
            };
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

                var props = new SlashCommandBuilder
                {
                    Name = moduleInfo.SlashGroupName.ToLower(),
                    Description = moduleInfo.Description,
                    IsDefaultPermission = moduleInfo.DefaultPermission,
                }.Build();

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
                    Name = moduleInfo.SlashGroupName.ToLower(),
                    Description = moduleInfo.Description,
                    Type = ApplicationCommandOptionType.SubCommandGroup,
                    Options = options
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
                    Options = command.Options?.Select(x => x.ToApplicationCommandOptionProps())?.ToList() ?? Optional<List<ApplicationCommandOptionProperties>>.Unspecified
                },
                ApplicationCommandType.User => new UserCommandProperties
                {
                    Name = command.Name,
                    IsDefaultPermission = command.IsDefaultPermission
                },
                ApplicationCommandType.Message => new MessageCommandProperties
                {
                    Name = command.Name,
                    IsDefaultPermission = command.IsDefaultPermission
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
                Choices = commandOption.Choices?.Select(x => new ApplicationCommandOptionChoiceProperties
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToList(),
                Options = commandOption.Options?.Select(x => x.ToApplicationCommandOptionProps()).ToList()
            };
    }
}
