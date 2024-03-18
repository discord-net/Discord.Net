using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public static async Task<IEnumerable<TypeInfo>> SearchAsync(Assembly assembly, InteractionService commandService)
        {
            static bool IsLoadableModule(TypeInfo info)
            {
                return info.DeclaredMethods.Any(x => x.GetCustomAttribute<SlashCommandAttribute>() != null);
            }

            var result = new List<TypeInfo>();

            foreach (var type in assembly.DefinedTypes)
            {
                if ((type.IsPublic || type.IsNestedPublic) && IsValidModuleDefinition(type))
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

        public static async Task<Dictionary<Type, ModuleInfo>> BuildAsync(IEnumerable<TypeInfo> validTypes, InteractionService commandService,
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

        private static void BuildModule(ModuleBuilder builder, TypeInfo typeInfo, InteractionService commandService,
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
#pragma warning disable CS0618 // Type or member is obsolete
                    case DefaultPermissionAttribute defPermission:
                        {
                            builder.DefaultPermission = defPermission.IsDefaultPermission;
                        }
                        break;
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
                    case EnabledInDmAttribute enabledInDm:
                    {
                            builder.IsEnabledInDm = enabledInDm.IsEnabled;
                        }
                        break;
#pragma warning restore CS0618 // Type or member is obsolete
                    case DefaultMemberPermissionsAttribute memberPermission:
                        {
                            builder.DefaultMemberPermissions = memberPermission.Permissions;
                        }
                        break;
                    case PreconditionAttribute precondition:
                        builder.AddPreconditions(precondition);
                        break;
                    case DontAutoRegisterAttribute dontAutoRegister:
                        builder.DontAutoRegister = true;
                        break;
                    case NsfwCommandAttribute nsfwCommand:
                        builder.SetNsfw(nsfwCommand.IsNsfw);
                        break;
                    case CommandContextTypeAttribute contextType:
                        builder.WithContextTypes(contextType.ContextTypes?.ToArray());
                        break;
                    case IntegrationTypeAttribute integrationType:
                        builder.WithIntegrationTypes(integrationType.IntegrationTypes?.ToArray());
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
            var validModalCommands = methods.Where(IsValidModalCommanDefinition);

            Func<IServiceProvider, IInteractionModuleBase> createInstance = commandService._useCompiledLambda ?
                ReflectionUtils<IInteractionModuleBase>.CreateLambdaBuilder(typeInfo, commandService) : ReflectionUtils<IInteractionModuleBase>.CreateBuilder(typeInfo, commandService);

            foreach (var method in validSlashCommands)
                builder.AddSlashCommand(x => BuildSlashCommand(x, createInstance, method, commandService, services));

            foreach (var method in validContextCommands)
                builder.AddContextCommand(x => BuildContextCommand(x, createInstance, method, commandService, services));

            foreach (var method in validInteractions)
                builder.AddComponentCommand(x => BuildComponentCommand(x, createInstance, method, commandService, services));

            foreach (var method in validAutocompleteCommands)
                builder.AddAutocompleteCommand(x => BuildAutocompleteCommand(x, createInstance, method, commandService, services));

            foreach (var method in validModalCommands)
                builder.AddModalCommand(x => BuildModalCommand(x, createInstance, method, commandService, services));
        }

        private static void BuildSubModules(ModuleBuilder parent, IEnumerable<TypeInfo> subModules, IList<TypeInfo> builtTypes, InteractionService commandService,
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

        private static void BuildSlashCommand(SlashCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
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
#pragma warning disable CS0618 // Type or member is obsolete
                    case DefaultPermissionAttribute defaultPermission:
                        {
                            builder.DefaultPermission = defaultPermission.IsDefaultPermission;
                        }
                        break;
                    case EnabledInDmAttribute enabledInDm:
                        {
                            builder.IsEnabledInDm = enabledInDm.IsEnabled;
                        }
                        break;
#pragma warning restore CS0618 // Type or member is obsolete
                    case DefaultMemberPermissionsAttribute memberPermission:
                        {
                            builder.DefaultMemberPermissions = memberPermission.Permissions;
                        }
                        break;
                    case PreconditionAttribute precondition:
                        builder.WithPreconditions(precondition);
                        break;
                    case NsfwCommandAttribute nsfwCommand:
                        builder.SetNsfw(nsfwCommand.IsNsfw);
                        break;
                    case CommandContextTypeAttribute contextType:
                        builder.WithContextTypes(contextType.ContextTypes.ToArray());
                        break;
                    case IntegrationTypeAttribute integrationType:
                        builder.WithIntegrationTypes(integrationType.IntegrationTypes.ToArray());
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

        private static void BuildContextCommand(ContextCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
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
#pragma warning disable CS0618 // Type or member is obsolete
                    case DefaultPermissionAttribute defaultPermission:
                        {
                            builder.DefaultPermission = defaultPermission.IsDefaultPermission;
                        }
                        break;
                    case EnabledInDmAttribute enabledInDm:
                        {
                            builder.IsEnabledInDm = enabledInDm.IsEnabled;
                        }
                        break;
#pragma warning restore CS0618 // Type or member is obsolete
                    case DefaultMemberPermissionsAttribute memberPermission:
                        {
                            builder.DefaultMemberPermissions = memberPermission.Permissions;
                        }
                        break;
                    case PreconditionAttribute precondition:
                        builder.WithPreconditions(precondition);
                        break;
                    case NsfwCommandAttribute nsfwCommand:
                        builder.SetNsfw(nsfwCommand.IsNsfw);
                        break;
                    case CommandContextTypeAttribute contextType:
                        builder.WithContextTypes(contextType.ContextTypes.ToArray());
                        break;
                    case IntegrationTypeAttribute integrationType:
                        builder.WithIntegrationTypes(integrationType.IntegrationTypes.ToArray());
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

        private static void BuildComponentCommand(ComponentCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
            InteractionService commandService, IServiceProvider services)
        {
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
                            builder.TreatNameAsRegex = interaction.TreatAsRegex;
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

            var wildCardCount = RegexUtils.GetWildCardCount(builder.Name, commandService._wildCardExp);

            foreach (var parameter in parameters)
                builder.AddParameter(x => BuildComponentParameter(x, parameter, parameter.Position >= wildCardCount));

            builder.Callback = CreateCallback(createInstance, methodInfo, commandService);
        }

        private static void BuildAutocompleteCommand(AutocompleteCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
            InteractionService commandService, IServiceProvider services)
        {
            var attributes = methodInfo.GetCustomAttributes();

            builder.MethodName = methodInfo.Name;

            foreach (var attribute in attributes)
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

        private static void BuildModalCommand(ModalCommandBuilder builder, Func<IServiceProvider, IInteractionModuleBase> createInstance, MethodInfo methodInfo,
            InteractionService commandService, IServiceProvider services)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Count(x => typeof(IModal).IsAssignableFrom(x.ParameterType)) > 1)
                throw new InvalidOperationException($"A modal command can only have one {nameof(IModal)} parameter.");

            if (!typeof(IModal).IsAssignableFrom(parameters.Last().ParameterType))
                throw new InvalidOperationException($"Last parameter of a modal command must be an implementation of {nameof(IModal)}");

            var attributes = methodInfo.GetCustomAttributes();

            builder.MethodName = methodInfo.Name;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ModalInteractionAttribute modal:
                        {
                            builder.Name = modal.CustomId;
                            builder.RunMode = modal.RunMode;
                            builder.IgnoreGroupNames = modal.IgnoreGroupNames;
                            builder.TreatNameAsRegex = modal.TreatAsRegex;
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

            foreach (var parameter in parameters)
                builder.AddParameter(x => BuildParameter(x, parameter));

            builder.Callback = CreateCallback(createInstance, methodInfo, commandService);
        }

        private static ExecuteCallback CreateCallback(Func<IServiceProvider, IInteractionModuleBase> createInstance,
            MethodInfo methodInfo, InteractionService commandService)
        {
            Func<IInteractionModuleBase, object[], Task> commandInvoker = commandService._useCompiledLambda ?
                ReflectionUtils<IInteractionModuleBase>.CreateMethodInvoker(methodInfo) : (module, args) => methodInfo.Invoke(module, args) as Task;

            async Task<IResult> ExecuteCallback(IInteractionContext context, object[] args, IServiceProvider serviceProvider, ICommandInfo commandInfo)
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
                    var interactionException = new InteractionException(commandInfo, context, ex);
                    await commandService._cmdLogger.ErrorAsync(interactionException).ConfigureAwait(false);
                    return ExecuteResult.FromError(interactionException);
                }
                finally
                {
                    await instance.AfterExecuteAsync(commandInfo).ConfigureAwait(false);
                    instance.AfterExecute(commandInfo);
                    (instance as IDisposable)?.Dispose();
                }
            }

            return ExecuteCallback;
        }

        #region Parameters
        private static void BuildSlashParameter(SlashCommandParameterBuilder builder, ParameterInfo paramInfo, IServiceProvider services)
        {
            var attributes = paramInfo.GetCustomAttributes();
            var paramType = paramInfo.ParameterType;

            builder.Name = paramInfo.Name;
            builder.Description = paramInfo.Name;
            builder.IsRequired = !paramInfo.IsOptional;
            builder.DefaultValue = paramInfo.DefaultValue;

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
                        if (autocomplete.AutocompleteHandlerType is not null)
                            builder.WithAutocompleteHandler(autocomplete.AutocompleteHandlerType, services);
                        break;
                    case MaxValueAttribute maxValue:
                        builder.MaxValue = maxValue.Value;
                        break;
                    case MinValueAttribute minValue:
                        builder.MinValue = minValue.Value;
                        break;
                    case MinLengthAttribute minLength:
                        builder.MinLength = minLength.Length;
                        break;
                    case MaxLengthAttribute maxLength:
                        builder.MaxLength = maxLength.Length;
                        break;
                    case ComplexParameterAttribute complexParameter:
                        {
                            builder.IsComplexParameter = true;
                            ConstructorInfo ctor = GetComplexParameterConstructor(paramInfo.ParameterType.GetTypeInfo(), complexParameter);

                            foreach (var parameter in ctor.GetParameters())
                            {
                                if (parameter.IsDefined(typeof(ComplexParameterAttribute)))
                                    throw new InvalidOperationException("You cannot create nested complex parameters.");

                                builder.AddComplexParameterField(fieldBuilder => BuildSlashParameter(fieldBuilder, parameter, services));
                            }

                            var initializer = builder.Command.Module.InteractionService._useCompiledLambda ?
                                ReflectionUtils<object>.CreateLambdaConstructorInvoker(paramInfo.ParameterType.GetTypeInfo()) : ctor.Invoke;
                            builder.ComplexParameterInitializer = args => initializer(args);
                        }
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }

            builder.SetParameterType(paramType, services);

            // Replace pascal casings with '-'
            builder.Name = Regex.Replace(builder.Name, "(?<=[a-z])(?=[A-Z])", "-").ToLower();
        }

        private static void BuildComponentParameter(ComponentCommandParameterBuilder builder, ParameterInfo paramInfo, bool isComponentParam)
        {
            builder.SetIsRouteSegment(!isComponentParam);
            BuildParameter(builder, paramInfo);
        }

        private static void BuildParameter<TInfo, TBuilder>(ParameterBuilder<TInfo, TBuilder> builder, ParameterInfo paramInfo)
            where TInfo : class, IParameterInfo
            where TBuilder : ParameterBuilder<TInfo, TBuilder>
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

        #region Modals
        public static ModalInfo BuildModalInfo(Type modalType, InteractionService interactionService)
        {
            if (!typeof(IModal).IsAssignableFrom(modalType))
                throw new InvalidOperationException($"{modalType.FullName} isn't an implementation of {typeof(IModal).FullName}");

            var instance = Activator.CreateInstance(modalType, false) as IModal;

            try
            {
                var builder = new ModalBuilder(modalType, interactionService)
                {
                    Title = instance.Title
                };

                var inputs = modalType.GetProperties().Where(IsValidModalInputDefinition);

                foreach (var prop in inputs)
                {
                    var componentType = prop.GetCustomAttribute<ModalInputAttribute>()?.ComponentType;

                    switch (componentType)
                    {
                        case ComponentType.TextInput:
                            builder.AddTextComponent(x => BuildTextInput(x, prop, prop.GetValue(instance)));
                            break;
                        case null:
                            throw new InvalidOperationException($"{prop.Name} of {prop.DeclaringType.Name} isn't a valid modal input field.");
                        default:
                            throw new InvalidOperationException($"Component type {componentType} cannot be used in modals.");
                    }
                }

                var memberInit = ReflectionUtils<IModal>.CreateLambdaMemberInit(modalType.GetTypeInfo(), modalType.GetConstructor(Type.EmptyTypes), x => x.IsDefined(typeof(ModalInputAttribute)));
                builder.ModalInitializer = (args) => memberInit(Array.Empty<object>(), args);
                return builder.Build();
            }
            finally
            {
                (instance as IDisposable)?.Dispose();
            }
        }

        private static void BuildTextInput(TextInputComponentBuilder builder, PropertyInfo propertyInfo, object defaultValue)
        {
            var attributes = propertyInfo.GetCustomAttributes();

            builder.Label = propertyInfo.Name;
            builder.DefaultValue = defaultValue;
            builder.WithType(propertyInfo.PropertyType);
            builder.PropertyInfo = propertyInfo;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ModalTextInputAttribute textInput:
                        builder.CustomId = textInput.CustomId;
                        builder.ComponentType = textInput.ComponentType;
                        builder.Style = textInput.Style;
                        builder.Placeholder = textInput.Placeholder;
                        builder.MaxLength = textInput.MaxLength;
                        builder.MinLength = textInput.MinLength;
                        builder.InitialValue = textInput.InitialValue;
                        break;
                    case RequiredInputAttribute requiredInput:
                        builder.IsRequired = requiredInput.IsRequired;
                        break;
                    case InputLabelAttribute inputLabel:
                        builder.Label = inputLabel.Label;
                        break;
                    default:
                        builder.WithAttributes(attribute);
                        break;
                }
            }
        }
        #endregion

        internal static bool IsValidModuleDefinition(TypeInfo typeInfo)
        {
            return ModuleTypeInfo.IsAssignableFrom(typeInfo) &&
                   !typeInfo.IsAbstract &&
                   !typeInfo.ContainsGenericParameters;
        }

        private static bool IsValidSlashCommandDefinition(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(SlashCommandAttribute)) &&
                   (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>)) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }

        private static bool IsValidContextCommandDefinition(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(ContextCommandAttribute)) &&
                   (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>)) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }

        private static bool IsValidComponentCommandDefinition(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(ComponentInteractionAttribute)) &&
                   (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>)) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }

        private static bool IsValidAutocompleteCommandDefinition(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(AutocompleteCommandAttribute)) &&
                (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>)) &&
                !methodInfo.IsStatic &&
                !methodInfo.IsGenericMethod &&
                methodInfo.GetParameters().Length == 0;
        }

        private static bool IsValidModalCommanDefinition(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(ModalInteractionAttribute)) &&
                (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>)) &&
                !methodInfo.IsStatic &&
                !methodInfo.IsGenericMethod &&
                typeof(IModal).IsAssignableFrom(methodInfo.GetParameters().Last().ParameterType);
        }

        private static bool IsValidModalInputDefinition(PropertyInfo propertyInfo)
        {
            return propertyInfo.SetMethod?.IsPublic == true &&
                propertyInfo.SetMethod?.IsStatic == false &&
                propertyInfo.IsDefined(typeof(ModalInputAttribute));
        }

        private static ConstructorInfo GetComplexParameterConstructor(TypeInfo typeInfo, ComplexParameterAttribute complexParameter)
        {
            var ctors = typeInfo.GetConstructors();

            if (ctors.Length == 0)
                throw new InvalidOperationException($"No constructor found for \"{typeInfo.FullName}\".");

            if (complexParameter.PrioritizedCtorSignature is not null)
            {
                var ctor = typeInfo.GetConstructor(complexParameter.PrioritizedCtorSignature);

                if (ctor is null)
                    throw new InvalidOperationException($"No constructor was found with the signature: {string.Join(",", complexParameter.PrioritizedCtorSignature.Select(x => x.Name))}");

                return ctor;
            }

            var prioritizedCtors = ctors.Where(x => x.IsDefined(typeof(ComplexParameterCtorAttribute), true));

            switch (prioritizedCtors.Count())
            {
                case > 1:
                    throw new InvalidOperationException($"{nameof(ComplexParameterCtorAttribute)} can only be used once in a type.");
                case 1:
                    return prioritizedCtors.First();
            }

            switch (ctors.Length)
            {
                case > 1:
                    throw new InvalidOperationException($"Multiple constructors found for \"{typeInfo.FullName}\".");
                default:
                    return ctors.First();
            }
        }
    }
}
