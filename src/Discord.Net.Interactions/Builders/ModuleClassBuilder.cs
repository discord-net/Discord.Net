using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord.Interactions.Builders
{
    internal static class ModuleClassBuilder
    {
        private static readonly TypeInfo ModuleTypeInfo = typeof(IInteractionModuleBase).GetTypeInfo();

        public const int MaxCommandDepth = 3;

        public static async Task<IEnumerable<TypeInfo>> SearchAsync (Assembly assembly, InteractionService commandService)
        {
            static bool IsLoadableModule (TypeInfo info)
            {
                return info.DeclaredMethods.Any(x => x.GetCustomAttribute<SlashCommandAttribute>() != null);
            }

            var result = new List<TypeInfo>();

            foreach (var type in assembly.DefinedTypes)
            {
                if (( type.IsPublic || type.IsNestedPublic ) && IsValidModuleDefinition(type))
                {
                    result.Add(type);
                }
                else if (IsLoadableModule(type))
                {
                    await commandService._cmdLogger.WarningAsync($"Class {type.FullName} is not public and cannot be loaded.").ConfigureAwait(false);
                }
            }
            return result;
        }

        public static async Task<Dictionary<Type, ModuleInfo>> BuildAsync (IEnumerable<TypeInfo> validTypes, InteractionService commandService,
            IServiceProvider services)
        {
            var topLevelGroups = validTypes.Where(x => x.DeclaringType == null || !IsValidModuleDefinition(x.DeclaringType.GetTypeInfo()));
            var built = new List<TypeInfo>();

            var result = new Dictionary<Type, ModuleInfo>();

            foreach (var type in topLevelGroups)
            {
                var builder = new ModuleBuilder(commandService);

                BuildModule(builder, type, commandService, services);
                BuildSubModules(builder, type.DeclaredNestedTypes, built, commandService, services);
                built.Add(type);

                var moduleInfo = builder.Build(commandService, services);

                result.Add(type.AsType(), moduleInfo);
            }

            await commandService._cmdLogger.DebugAsync($"Successfully built {built.Count} Slash Command modules.").ConfigureAwait(false);

            return result;
        }

        private static void BuildModule (ModuleBuilder builder, TypeInfo typeInfo, InteractionService commandService,
            IServiceProvider services)
        {
            var attributes = typeInfo.GetCustomAttributes();

            builder.Name = typeInfo.Name;
            builder.TypeInfo = typeInfo;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case GroupAttribute group:
                        {
                            builder.SlashGroupName = group.Name;
                            builder.Description = group.Description;
                        }
                        break;
                    case DefaultPermissionAttribute defPermission:
                        {
                            builder.DefaultPermission = defPermission.IsDefaultPermission;
                        }
                        break;
                    case PreconditionAttribute precondition:
                        builder.AddPreconditions(precondition);
                        break;
                    case DontAutoRegisterAttribute dontAutoRegister:
                        builder.DontAutoRegister = true;
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }

            var methods = typeInfo.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var validSlashCommands = methods.Where(IsValidSlashCommandDefinition);
            var validContextCommands = methods.Where(IsValidContextCommandDefinition);
            var validInteractions = methods.Where(IsValidComponentCommandDefinition);
            var validAutocompleteCommands = methods.Where(IsValidAutocompleteCommandDefinition);

            Func<IServiceProvider, IInteractionModuleBase> createInstance = commandService._useCompiledLambda ?
                ReflectionUtils<IInteractionModuleBase>.CreateLambdaBuilder(typeInfo, commandService) : ReflectionUtils<IInteractionModuleBase>.CreateBuilder(typeInfo, commandService);

            foreach (var method in validSlashCommands)
                builder.AddSlashCommand(x => BuildSlashCommand(x, createInstance, method, commandService, services));

            foreach (var method in validContextCommands)
                builder.AddContextCommand(x => BuildContextCommand(x, createInstance, method, commandService, services));

            foreach (var method in validInteractions)
                builder.AddComponentCommand(x => BuildComponentCommand(x, createInstance, method, commandService, services));

            foreach(var method in validAutocompleteCommands)
                builder.AddAutocompleteCommand(x => BuildAutocompleteCommand(x, createInstance, method, commandService, services));
        }

        private static void BuildSubModules (ModuleBuilder parent, IEnumerable<TypeInfo> subModules, IList<TypeInfo> builtTypes, InteractionService commandService,
            IServiceProvider services, int slashGroupDepth = 0)
        {
            foreach (var submodule in subModules.Where(IsValidModuleDefinition))
            {
                if (builtTypes.Contains(submodule))
                    continue;

                parent.AddModule((builder) =>
                {
                    BuildModule(builder, submodule, commandService, services);

                    if (slashGroupDepth >= MaxCommandDepth - 1)
                        throw new InvalidOperationException($"Slash Commands only support {MaxCommandDepth - 1} command prefixes for sub-commands");

                    BuildSubModules(builder, submodule.DeclaredNestedTypes, builtTypes, commandService, services, builder.IsSlashGroup ? slashGroupDepth + 1 : slashGroupDepth);
                });
                builtTypes.Add(submodule);
            }
        }

        private static void BuildSlashCommand (SlashCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
            InteractionService commandService, IServiceProvider services)
        {
            var attributes = methodInfo.GetCustomAttributes();

            builder.MethodName = methodInfo.Name;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case SlashCommandAttribute command:
                        {
                            builder.Name = command.Name;
                            builder.Description = command.Description;
                            builder.IgnoreGroupNames = command.IgnoreGroupNames;
                            builder.RunMode = command.RunMode;
                        }
                        break;
                    case DefaultPermissionAttribute defaultPermission:
                        {
                            builder.DefaultPermission = defaultPermission.IsDefaultPermission;
                        }
                        break;
                    case PreconditionAttribute precondition:
                        builder.WithPreconditions(precondition);
                        break;
                    default:
                        builder.WithAttributes(attribute);
                        break;
                }
            }

            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
                builder.AddParameter(x => BuildSlashParameter(x, parameter, services));

            builder.Callback = CreateCallback(createInstance, methodInfo, commandService);
        }

        private static void BuildContextCommand (ContextCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
            InteractionService commandService, IServiceProvider services)
        {
            var attributes = methodInfo.GetCustomAttributes();

            builder.MethodName = methodInfo.Name;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ContextCommandAttribute command:
                        {
                            builder.Name = command.Name;
                            builder.CommandType = command.CommandType;
                            builder.RunMode = command.RunMode;

                            command.CheckMethodDefinition(methodInfo);
                        }
                        break;
                    case DefaultPermissionAttribute defaultPermission:
                        {
                            builder.DefaultPermission = defaultPermission.IsDefaultPermission;
                        }
                        break;
                    case PreconditionAttribute precondition:
                        builder.WithPreconditions(precondition);
                        break;
                    default:
                        builder.WithAttributes(attribute);
                        break;
                }
            }

            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
                builder.AddParameter(x => BuildParameter(x, parameter));

            builder.Callback = CreateCallback(createInstance, methodInfo, commandService);
        }

        private static void BuildComponentCommand (ComponentCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
            InteractionService commandService, IServiceProvider services)
        {
            if (!methodInfo.GetParameters().All(x => x.ParameterType == typeof(string) || x.ParameterType == typeof(string[])))
                throw new InvalidOperationException($"Interaction method parameters all must be types of {typeof(string).Name} or {typeof(string[]).Name}");

            var attributes = methodInfo.GetCustomAttributes();

            builder.MethodName = methodInfo.Name;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ComponentInteractionAttribute interaction:
                        {
                            builder.Name = interaction.CustomId;
                            builder.RunMode = interaction.RunMode;
                            builder.IgnoreGroupNames = interaction.IgnoreGroupNames;
                        }
                        break;
                    case PreconditionAttribute precondition:
                        builder.WithPreconditions(precondition);
                        break;
                    default:
                        builder.WithAttributes(attribute);
                        break;
                }
            }

            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
                builder.AddParameter(x => BuildParameter(x, parameter));

            builder.Callback = CreateCallback(createInstance, methodInfo, commandService);
        }

        private static void BuildAutocompleteCommand(AutocompleteCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
            InteractionService commandService, IServiceProvider services)
        {
            var attributes = methodInfo.GetCustomAttributes();

            builder.MethodName = methodInfo.Name;

            foreach(var attribute in attributes)
            {
                switch (attribute)
                {
                    case AutocompleteCommandAttribute autocomplete:
                        {
                            builder.ParameterName = autocomplete.ParameterName;
                            builder.CommandName = autocomplete.CommandName;
                            builder.Name = autocomplete.CommandName + " " + autocomplete.ParameterName;
                            builder.RunMode = autocomplete.RunMode;
                        }
                        break;
                    case PreconditionAttribute precondition:
                        builder.WithPreconditions(precondition);
                        break;
                    default:
                        builder.WithAttributes(attribute);
                        break;
                }
            }

            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
                builder.AddParameter(x => BuildParameter(x, parameter));

            builder.Callback = CreateCallback(createInstance, methodInfo, commandService);
        }

        private static ExecuteCallback CreateCallback (Func<IServiceProvider, IInteractionModuleBase> createInstance,
            MethodInfo methodInfo, InteractionService commandService)
        {
            Func<IInteractionModuleBase, object[], Task> commandInvoker = commandService._useCompiledLambda ?
                ReflectionUtils<IInteractionModuleBase>.CreateMethodInvoker(methodInfo) : (module, args) => methodInfo.Invoke(module, args) as Task;

            async Task<IResult> ExecuteCallback (IInteractionContext context, object[] args, IServiceProvider serviceProvider, ICommandInfo commandInfo)
            {
                var instance = createInstance(serviceProvider);
                instance.SetContext(context);

                try
                {
                    await instance.BeforeExecuteAsync(commandInfo).ConfigureAwait(false);
                    instance.BeforeExecute(commandInfo);
                    var task = commandInvoker(instance, args) ?? Task.Delay(0);

                    if (task is Task<RuntimeResult> runtimeTask)
                    {
                        return await runtimeTask.ConfigureAwait(false);
                    }
                    else
                    {
                        await task.ConfigureAwait(false);
                        return ExecuteResult.FromSuccess();

                    }
                }
                catch (Exception ex)
                {
                    await commandService._cmdLogger.ErrorAsync(ex).ConfigureAwait(false);
                    return ExecuteResult.FromError(ex);
                }
                finally
                {
                    await instance.AfterExecuteAsync(commandInfo).ConfigureAwait(false);
                    instance.AfterExecute(commandInfo);
                    ( instance as IDisposable )?.Dispose();
                }
            }

            return ExecuteCallback;
        }

        #region Parameters
        private static void BuildSlashParameter (SlashCommandParameterBuilder builder, ParameterInfo paramInfo, IServiceProvider services)
        {
            var attributes = paramInfo.GetCustomAttributes();
            var paramType = paramInfo.ParameterType;

            builder.Name = paramInfo.Name;
            builder.Description = paramInfo.Name;
            builder.IsRequired = !paramInfo.IsOptional;
            builder.DefaultValue = paramInfo.DefaultValue;
            builder.SetParameterType(paramType, services);

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case SummaryAttribute description:
                        {
                            if (!string.IsNullOrEmpty(description.Name))
                                builder.Name = description.Name;

                            if (!string.IsNullOrEmpty(description.Description))
                                builder.Description = description.Description;
                        }
                        break;
                    case ChoiceAttribute choice:
                        builder.WithChoices(new ParameterChoice(choice.Name, choice.Value));
                        break;
                    case ParamArrayAttribute _:
                        builder.IsParameterArray = true;
                        break;
                    case ParameterPreconditionAttribute precondition:
                        builder.AddPreconditions(precondition);
                        break;
                    case ChannelTypesAttribute channelTypes:
                        builder.WithChannelTypes(channelTypes.ChannelTypes);
                        break;
                    case AutocompleteAttribute autocomplete:
                        builder.Autocomplete = true;
                        if(autocomplete.AutocompleteHandlerType is not null)
                            builder.WithAutocompleteHandler(autocomplete.AutocompleteHandlerType, services);
                        break;
                    case MaxValueAttribute maxValue:
                        builder.MaxValue = maxValue.Value;
                        break;
                    case MinValueAttribute minValue:
                        builder.MinValue = minValue.Value;
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }

            // Replace pascal casings with '-'
            builder.Name = Regex.Replace(builder.Name, "(?<=[a-z])(?=[A-Z])", "-").ToLower();
        }

        private static void BuildParameter (CommandParameterBuilder builder, ParameterInfo paramInfo)
        {
            var attributes = paramInfo.GetCustomAttributes();
            var paramType = paramInfo.ParameterType;

            builder.Name = paramInfo.Name;
            builder.IsRequired = !paramInfo.IsOptional;
            builder.DefaultValue = paramInfo.DefaultValue;
            builder.SetParameterType(paramType);

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ParameterPreconditionAttribute precondition:
                        builder.AddPreconditions(precondition);
                        break;
                    case ParamArrayAttribute _:
                        builder.IsParameterArray = true;
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }
        }
        #endregion

        internal static bool IsValidModuleDefinition (TypeInfo typeInfo)
        {
            return ModuleTypeInfo.IsAssignableFrom(typeInfo) &&
                   !typeInfo.IsAbstract &&
                   !typeInfo.ContainsGenericParameters;
        }

        private static bool IsValidSlashCommandDefinition (MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(SlashCommandAttribute)) &&
                   ( methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>) ) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }

        private static bool IsValidContextCommandDefinition (MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(ContextCommandAttribute)) &&
                   ( methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>) ) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }

        private static bool IsValidComponentCommandDefinition (MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(ComponentInteractionAttribute)) &&
                   (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>)) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }

        private static bool IsValidAutocompleteCommandDefinition (MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(AutocompleteCommandAttribute)) &&
                (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>)) &&
                !methodInfo.IsStatic &&
                !methodInfo.IsGenericMethod &&
                methodInfo.GetParameters().Length == 0;
        }
    }
}
