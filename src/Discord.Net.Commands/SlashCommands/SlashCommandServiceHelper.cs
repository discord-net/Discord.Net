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
                .Any(n => n == typeof(ISlashCommandModule));
        }

        /// <summary>
        /// Create an instance of each user-defined module
        /// </summary>
        public static async Task<Dictionary<Type, SlashModuleInfo>> InstantiateModules(IReadOnlyList<TypeInfo> types, SlashCommandService slashCommandService)
        {
            var result = new Dictionary<Type, SlashModuleInfo>();
            foreach (Type userModuleType in types)
            {
                SlashModuleInfo moduleInfo = new SlashModuleInfo(slashCommandService);

                // If they want a constructor with different parameters, this is the place to add them.
                object instance = userModuleType.GetConstructor(Type.EmptyTypes).Invoke(null);
                moduleInfo.SetCommandModule(instance);

                result.Add(userModuleType, moduleInfo);
            }
            return result;
        }

        /// <summary>
        /// Prepare all of the commands and register them internally.
        /// </summary>
        public static async Task<Dictionary<string, SlashCommandInfo>> PrepareAsync(IReadOnlyList<TypeInfo> types, Dictionary<Type, SlashModuleInfo> moduleDefs, SlashCommandService slashCommandService)
        {
            var result = new Dictionary<string, SlashCommandInfo>();
            // fore each user-defined module
            foreach (var userModule in types)
            {
                // Get its associated information
                SlashModuleInfo moduleInfo;
                if (moduleDefs.TryGetValue(userModule, out moduleInfo))
                {
                    // and get all of its method, and check if they are valid, and if so create a new SlashCommandInfo for them.
                    var commandMethods = userModule.GetMethods();
                    List<SlashCommandInfo> commandInfos = new List<SlashCommandInfo>();
                    foreach (var commandMethod in commandMethods)
                    {
                        SlashCommand slashCommand;
                        if (IsValidSlashCommand(commandMethod, out slashCommand))
                        {
                            Delegate delegateMethod = CreateDelegate(commandMethod, moduleInfo.userCommandModule);
                            SlashCommandInfo commandInfo = new SlashCommandInfo(
                                module: moduleInfo,
                                name: slashCommand.commandName,
                                description: slashCommand.description,
                                userMethod: delegateMethod
                                );
                            result.Add(slashCommand.commandName, commandInfo);
                            commandInfos.Add(commandInfo);
                        }
                    }
                    moduleInfo.SetCommands(commandInfos);
                }
            }
            return result;
        }
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

        public static async Task RegisterCommands(Dictionary<string, SlashCommandInfo> commandDefs, SlashCommandService slashCommandService, IServiceProvider services)
        {
            return;
        }
    }
}
