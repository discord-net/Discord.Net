using Discord.Commands.Builders;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    internal static class SlashCommandServiceHelper
    {
        /// <summary>
        /// Get all of the valid user-defined slash command modules 
        /// </summary>
        public static async Task<IReadOnlyList<TypeInfo>> GetValidModuleClasses(Assembly assembly, SlashCommandService service)
        {
            var result = new List<TypeInfo>();

            foreach (TypeInfo typeInfo in assembly.DefinedTypes)
            {
                if (IsValidModuleDefinition(typeInfo))
                {
                    // To simplify our lives, we need the modules to be public.
                    if (typeInfo.IsPublic || typeInfo.IsNestedPublic)
                    {
                        result.Add(typeInfo);
                    }
                    else
                    {
                        await service._logger.WarningAsync($"Found class {typeInfo.FullName} as a valid SlashCommand Module, but it's not public!");
                    }
                }
            }

            return result;
        }
        private static bool IsValidModuleDefinition(TypeInfo typeInfo)
        {
            // See if the base type (SlashCommandInfo<T>) implements interface ISlashCommandModule
            return typeInfo.BaseType.GetInterfaces()
                .Any(n => n == typeof(ISlashCommandModule)) &&
                typeInfo.GetCustomAttributes(typeof(CommandGroup)).Count() == 0;
        }

        /// <summary>
        /// Create an instance of each user-defined module
        /// </summary>
        public static async Task<Dictionary<Type, SlashModuleInfo>> InstantiateModules(IReadOnlyList<TypeInfo> types, SlashCommandService slashCommandService)
        {
            var result = new Dictionary<Type, SlashModuleInfo>();
            // Here we get all modules thate are NOT sub command groups
            foreach (Type userModuleType in types)
            {
                SlashModuleInfo moduleInfo = new SlashModuleInfo(slashCommandService);
                moduleInfo.SetType(userModuleType);

                // If they want a constructor with different parameters, this is the place to add them.
                object instance = userModuleType.GetConstructor(Type.EmptyTypes).Invoke(null);
                moduleInfo.SetCommandModule(instance);

                // ,,
                moduleInfo.SetSubCommandGroups(InstantiateSubCommands(userModuleType, moduleInfo, slashCommandService));

                result.Add(userModuleType, moduleInfo);
            }
            return result;
        }
        public static List<SlashModuleInfo> InstantiateSubCommands(Type rootModule,SlashModuleInfo rootModuleInfo, SlashCommandService slashCommandService)
        {
            List<SlashModuleInfo> commandGroups = new List<SlashModuleInfo>();
            foreach(Type commandGroupType in rootModule.GetNestedTypes())
            {
                if(TryGetCommandGroupAttribute(commandGroupType, out CommandGroup commandGroup))
                {
                    SlashModuleInfo groupInfo = new SlashModuleInfo(slashCommandService);
                    groupInfo.SetType(commandGroupType);

                    object instance = commandGroupType.GetConstructor(Type.EmptyTypes).Invoke(null);
                    groupInfo.SetCommandModule(instance);

                    groupInfo.MakeCommandGroup(commandGroup,rootModuleInfo);
                    groupInfo.MakePath();

                    groupInfo.SetSubCommandGroups(InstantiateSubCommands(commandGroupType, groupInfo, slashCommandService));

                    commandGroups.Add(groupInfo);
                }
            }
            return commandGroups;
        }
        public static bool TryGetCommandGroupAttribute(Type module, out CommandGroup commandGroup)
        {
            if(!module.IsPublic && !module.IsNestedPublic)
            {
                commandGroup = null;
                return false;
            }

            var commandGroupAttributes = module.GetCustomAttributes(typeof(CommandGroup));
            if( commandGroupAttributes.Count() == 0)
            {
                commandGroup = null;
                return false;
            }
            else if(commandGroupAttributes.Count() > 1)
            {
                throw new Exception($"Too many CommandGroup attributes on a single class ({module.FullName}). It can only contain one!");
            }
            else
            {
                commandGroup = commandGroupAttributes.First() as CommandGroup;
                return true;
            }
        }

        /// <summary>
        /// Prepare all of the commands and register them internally.
        /// </summary>
        public static async Task<Dictionary<string, SlashCommandInfo>> CreateCommandInfos(IReadOnlyList<TypeInfo> types, Dictionary<Type, SlashModuleInfo> moduleDefs, SlashCommandService slashCommandService)
        {
            // Create the resulting dictionary ahead of time
            var result = new Dictionary<string, SlashCommandInfo>();
            // For each user-defined module ...
            foreach (var userModule in types)
            {
                // Get its associated information. If there isn't any it means something went wrong, but it's not a critical error.
                SlashModuleInfo moduleInfo;
                if (moduleDefs.TryGetValue(userModule, out moduleInfo))
                {
                    var commandInfos = RegisterSameLevelCommands(result, userModule, moduleInfo);
                    moduleInfo.SetCommands(commandInfos);
                    CreateSubCommandInfos(result, moduleInfo.commandGroups, slashCommandService);
                }
            }
            return result;
        }
        public static void CreateSubCommandInfos(Dictionary<string, SlashCommandInfo> result, List<SlashModuleInfo> subCommandGroups, SlashCommandService slashCommandService)
        {
            foreach (var subCommandGroup in subCommandGroups)
            {
                var commandInfos = RegisterSameLevelCommands(result, subCommandGroup.moduleType.GetTypeInfo(), subCommandGroup);
                subCommandGroup.SetCommands(commandInfos);
                CreateSubCommandInfos(result, subCommandGroup.commandGroups, slashCommandService);
            }
        }
        private static List<SlashCommandInfo> RegisterSameLevelCommands(Dictionary<string, SlashCommandInfo> result, TypeInfo userModule, SlashModuleInfo moduleInfo)
        {
            var commandMethods = userModule.GetMethods();
            List<SlashCommandInfo> commandInfos = new List<SlashCommandInfo>();
            foreach (var commandMethod in commandMethods)
            {
                SlashCommand slashCommand;
                if (IsValidSlashCommand(commandMethod, out slashCommand))
                {
                    // Create the delegate for the method we want to call once the user interacts with the bot.
                    // We use a delegate because of the unknown number and type of parameters we will have.
                    Delegate delegateMethod = CreateDelegate(commandMethod, moduleInfo.userCommandModule);

                    SlashCommandInfo commandInfo = new SlashCommandInfo(
                        module: moduleInfo,
                        name: slashCommand.commandName,
                        description: slashCommand.description,
                        // Generate the parameters. Due to it's complicated way the algorithm has been moved to its own function.
                        parameters: ConstructCommandParameters(commandMethod),
                        userMethod: delegateMethod
                        );

                    result.Add(commandInfo.Module.Path + SlashModuleInfo.PathSeperator + commandInfo.Name, commandInfo);
                    commandInfos.Add(commandInfo);
                }
            }

            return commandInfos;
        }

        /// <summary>
        /// Determines wheater a method can be clasified as a slash command
        /// </summary>
        private static bool IsValidSlashCommand(MethodInfo method, out SlashCommand slashCommand)
        {
            // Verify that we only have one [SlashCommand(...)] attribute
            IEnumerable<Attribute> slashCommandAttributes = method.GetCustomAttributes(typeof(SlashCommand));
            if (slashCommandAttributes.Count() > 1)
            {
                throw new Exception("Too many SlashCommand attributes on a single method. It can only contain one!");
            }
            // And at least one
            if (slashCommandAttributes.Count() == 0)
            {
                slashCommand = null;
                return false;
            }
            // And return the first (and only) attribute
            slashCommand = slashCommandAttributes.First() as SlashCommand;
            return true;
        }
        private static List<SlashParameterInfo> ConstructCommandParameters(MethodInfo method)
        {
            // Prepare the final list of parameters
            List<SlashParameterInfo> finalParameters = new List<SlashParameterInfo>();

            // For each mehod parameter ...
            // ex: ... MyCommand(string abc, int myInt)
            // `abc` and `myInt` are parameters
            foreach (var methodParameter in method.GetParameters())
            {
                SlashParameterInfo newParameter = new SlashParameterInfo();

                // Set the parameter name to that of the method
                // TODO: Implement an annotation that lets the user choose a custom name
                newParameter.Name = methodParameter.Name;

                // Get to see if it has a Description Attribute.
                // If it has
                // 0 -> then use the default description
                // 1 -> Use the value from that attribute
                // 2+ -> Throw an error. This shouldn't normaly happen, but we check for sake of sanity
                var descriptions = methodParameter.GetCustomAttributes(typeof(Description));
                if (descriptions.Count() == 0)
                    newParameter.Description = Description.DefaultDescription;
                else if (descriptions.Count() > 1)
                    throw new Exception($"Too many Description attributes on a single parameter ({method.Name} -> {methodParameter.Name}). It can only contain one!");
                else
                    newParameter.Description = (descriptions.First() as Description).description;

                // And get the parameter type
                newParameter.Type = TypeFromMethodParameter(methodParameter);

                // TODO: implement more attributes, such as [Required]

                finalParameters.Add(newParameter);
            }
            return finalParameters;
        }
        /// <summary>
        /// Get the type of command option from a method parameter info.
        /// </summary>
        private static ApplicationCommandOptionType TypeFromMethodParameter(ParameterInfo methodParameter)
        {
            // Can't do switch -- who knows why?
            if (methodParameter.ParameterType == typeof(int))
                return ApplicationCommandOptionType.Integer;
            if (methodParameter.ParameterType == typeof(string))
                return ApplicationCommandOptionType.String;
            if (methodParameter.ParameterType == typeof(bool))
                return ApplicationCommandOptionType.Boolean;
            if (methodParameter.ParameterType == typeof(SocketGuildChannel))
                return ApplicationCommandOptionType.Channel;
            if (methodParameter.ParameterType == typeof(SocketRole))
                return ApplicationCommandOptionType.Role;
            if (methodParameter.ParameterType == typeof(SocketGuildUser))
                return ApplicationCommandOptionType.User;

            throw new Exception($"Got parameter type other than int, string, bool, guild, role, or user. {methodParameter.Name}");
        }
        /// <summary>
        /// Creae a delegate from methodInfo. Taken from
        /// https://stackoverflow.com/a/40579063/8455128
        /// </summary>
        public static Delegate CreateDelegate(MethodInfo methodInfo, object target)
        {
            Func<Type[], Type> getType;
            var isAction = methodInfo.ReturnType.Equals((typeof(void)));
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { methodInfo.ReturnType });
            }

            if (methodInfo.IsStatic)
            {
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
            }

            return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
        }

        public static async Task RegisterCommands(DiscordSocketClient socketClient, Dictionary<Type, SlashModuleInfo> rootModuleInfos, Dictionary<string, SlashCommandInfo> commandDefs, SlashCommandService slashCommandService, CommandRegistrationOptions options)
        {
            // Get existing commmands
            ulong devGuild = 386658607338618891;
            var existingCommands = await socketClient.Rest.GetGuildApplicationCommands(devGuild).ConfigureAwait(false);
            List<string> existingCommandNames = new List<string>();
            foreach (var existingCommand in existingCommands)
            {
                existingCommandNames.Add(existingCommand.Name);
            }

            // Delete old ones that we want to re-implement
            if (options.OldCommands == OldCommandOptions.DELETE_UNUSED ||
                options.OldCommands == OldCommandOptions.WIPE)
            {
                foreach (var existingCommand in existingCommands)
                {
                    // If we want to wipe all commands
                    // or if the existing command isn't re-defined (probably code deleted by user)
                    // remove it from discord.
                    if(options.OldCommands == OldCommandOptions.WIPE ||
                      !commandDefs.ContainsKey(SlashModuleInfo.RootCommandPrefix + existingCommand.Name))
                    {
                        await existingCommand.DeleteAsync();
                    }
                }
            }
            //foreach (var entry in commandDefs)
            //{
            //    if (existingCommandNames.Contains(entry.Value.Name) &&
            //       options.ExistingCommands == ExistingCommandOptions.KEEP_EXISTING)
            //    {
            //        continue;
            //    }
            //    // If it's a new command or we want to overwrite an old one...
            //    else
            //    {
            //        SlashCommandInfo slashCommandInfo = entry.Value;
            //        SlashCommandCreationProperties command = slashCommandInfo.BuildCommand();
            //        
            //        await socketClient.Rest.CreateGuildCommand(command, devGuild).ConfigureAwait(false);
            //    }
            //}
            foreach (var pair in rootModuleInfos)
            {
                var rootModuleInfo = pair.Value;
                List<SlashCommandCreationProperties> builtCommands = rootModuleInfo.BuildCommands();
                foreach (var builtCommand in builtCommands)
                {
                    // TODO: Implement Global and Guild Commands.
                    await socketClient.Rest.CreateGuildCommand(builtCommand, devGuild).ConfigureAwait(false);
                }
            }

            return;
        }
    }
}
