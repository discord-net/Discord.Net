using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Discord.Commands.Builders;

namespace Discord.Commands
{
    internal static class ModuleClassBuilder
    {
        private static readonly TypeInfo _moduleTypeInfo = typeof(IModuleBase).GetTypeInfo();

        public static IEnumerable<TypeInfo> Search(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes)
            {
                var typeInfo = type.GetTypeInfo();
                if (IsValidModuleDefinition(typeInfo) &&
                    !typeInfo.IsDefined(typeof(DontAutoLoadAttribute)))
                {
                    yield return typeInfo;
                }
            }
        }

        public static Dictionary<Type, ModuleInfo> Build(CommandService service, params TypeInfo[] validTypes) => Build(validTypes, service);
        public static Dictionary<Type, ModuleInfo> Build(IEnumerable<TypeInfo> validTypes, CommandService service)
        {
            /*if (!validTypes.Any())
                throw new InvalidOperationException("Could not find any valid modules from the given selection");*/
            
            var topLevelGroups = validTypes.Where(x => x.DeclaringType == null);
            var subGroups = validTypes.Intersect(topLevelGroups);

            var builtTypes = new List<TypeInfo>();

            var result = new Dictionary<Type, ModuleInfo>();

            foreach (var typeInfo in topLevelGroups)
            {
                // TODO: This shouldn't be the case; may be safe to remove?
                if (result.ContainsKey(typeInfo.AsType()))
                    continue;

                var module = new ModuleBuilder(service, null);

                BuildModule(module, typeInfo, service);
                BuildSubTypes(module, typeInfo.DeclaredNestedTypes, builtTypes, service);

                result[typeInfo.AsType()] = module.Build(service);
            }

            return result;
        }

        private static void BuildSubTypes(ModuleBuilder builder, IEnumerable<TypeInfo> subTypes, List<TypeInfo> builtTypes, CommandService service)
        {
            foreach (var typeInfo in subTypes)
            {
                if (!IsValidModuleDefinition(typeInfo))
                    continue;
                
                if (builtTypes.Contains(typeInfo))
                    continue;
                
                builder.AddModule((module) => 
                {
                    BuildModule(module, typeInfo, service);
                    BuildSubTypes(module, typeInfo.DeclaredNestedTypes, builtTypes, service);
                });

                builtTypes.Add(typeInfo);
            }
        }

        private static void BuildModule(ModuleBuilder builder, TypeInfo typeInfo, CommandService service)
        {
            var attributes = typeInfo.GetCustomAttributes();

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case NameAttribute name:
                        builder.Name = name.Text;
                        break;
                    case SummaryAttribute summary:
                        builder.Summary = summary.Text;
                        break;
                    case RemarksAttribute remarks:
                        builder.Remarks = remarks.Text;
                        break;
                    case AliasAttribute alias:
                        builder.AddAliases(alias.Aliases);
                        break;
                    case GroupAttribute group:
                        builder.Name = builder.Name ?? group.Prefix;
                        builder.AddAliases(group.Prefix);
                        break;
                    case PreconditionAttribute precondition:
                        builder.AddPrecondition(precondition);
                        break;
                }
            }

            //Check for unspecified info
            if (builder.Aliases.Count == 0)
                builder.AddAliases("");
            if (builder.Name == null)
                builder.Name = typeInfo.Name;

            var validCommands = typeInfo.DeclaredMethods.Where(x => IsValidCommandDefinition(x));

            var groupedCommands = validCommands.GroupBy(x => x.GetCustomAttribute<CommandAttribute>().Text);
            foreach (var overloads in groupedCommands)
            {
                builder.AddCommand((command) => 
                {
                    string firstName = null;

                    foreach (var method in overloads)
                    {
                        if (firstName == null)
                            firstName = method.Name;

                        command.AddOverload((overload) =>
                        {
                            BuildOverload(overload, typeInfo, method, service);
                        });
                    }

                    var allAttributes = overloads.SelectMany(x => x.GetCustomAttributes());
                    BuildCommand(command, firstName, allAttributes, service);
                });
            }
        }

        private static void BuildCommand(CommandBuilder builder, string defaultName, IEnumerable<Attribute> attributes, CommandService service)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case CommandAttribute command:
                        builder.AddAliases(command.Text);
                        builder.Name = builder.Name ?? command.Text;
                        break;
                    case NameAttribute name:
                        builder.Name = name.Text;
                        break;
                    case SummaryAttribute summary:
                        builder.Summary = summary.Text;
                        break;
                    case RemarksAttribute remarks:
                        builder.Remarks = remarks.Text;
                        break;
                    case AliasAttribute alias:
                        builder.AddAliases(alias.Aliases);
                        break;
                }
            }

            if (builder.Name == null)
                builder.Name = defaultName;
        }

        private static void BuildOverload(OverloadBuilder builder, TypeInfo typeInfo, MethodInfo method, CommandService service)
        {
            var attributes = method.GetCustomAttributes();

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case CommandAttribute command:
                        builder.RunMode = command.RunMode;
                        break;
                    case PriorityAttribute priority:
                        builder.Priority = priority.Priority;
                        break;
                    case PreconditionAttribute precondition:
                        builder.AddPrecondition(precondition);
                        break;
                }
            }

            var parameters = method.GetParameters();
            int pos = 0, count = parameters.Length;
            foreach (var paramInfo in parameters)
            {
                builder.AddParameter((parameter) => 
                {
                    BuildParameter(parameter, paramInfo, pos++, count, service);
                });
            }

            var createInstance = ReflectionUtils.CreateBuilder<IModuleBase>(typeInfo, service);

            builder.Callback = async (ctx, args, map, overload) => 
            {
                var instance = createInstance(map);
                instance.SetContext(ctx);

                try
                {
                    instance.BeforeExecute(overload);
                    var task = method.Invoke(instance, args) as Task ?? Task.Delay(0);
                    await task.ConfigureAwait(false);
                }
                finally
                {
                    instance.AfterExecute(overload);
                    (instance as IDisposable)?.Dispose();
                }
            };
        }

        private static void BuildParameter(ParameterBuilder builder, System.Reflection.ParameterInfo paramInfo, int position, int count, CommandService service)
        {
            var attributes = paramInfo.GetCustomAttributes();
            var paramType = paramInfo.ParameterType;

            builder.Name = paramInfo.Name;

            builder.IsOptional = paramInfo.IsOptional;
            builder.DefaultValue = paramInfo.HasDefaultValue ? paramInfo.DefaultValue : null;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case SummaryAttribute summary:
                        builder.Summary = summary.Text;
                        break;
                    case OverrideTypeReaderAttribute typeReader:
                        builder.TypeReader = GetTypeReader(service, paramType, typeReader.TypeReader);
                        break;
                    case ParameterPreconditionAttribute precondition:
                        builder.AddPrecondition(precondition);
                        break;
                    case ParamArrayAttribute paramArray:
                        if (position != count - 1)
                            throw new InvalidOperationException("Params parameters must be the last parameter in a command.");
                        builder.IsMultiple = true;
                        paramType = paramType.GetElementType();
                        break;
                    case RemainderAttribute remainder:
                        if (position != count - 1)
                            throw new InvalidOperationException("Remainder parameters must be the last parameter in a command.");

                        builder.IsRemainder = true;
                        break;
                }
            }

            builder.ParameterType = paramType;

            if (builder.TypeReader == null)
            {
                var readers = service.GetTypeReaders(paramType);
                TypeReader reader = null;

                if (readers != null)
                    reader = readers.FirstOrDefault().Value;
                else
                    reader = service.GetDefaultTypeReader(paramType);

                builder.TypeReader = reader;
            }
        }

        private static TypeReader GetTypeReader(CommandService service, Type paramType, Type typeReaderType)
        {
            var readers = service.GetTypeReaders(paramType);
            TypeReader reader = null;
            if (readers != null)
            {
                if (readers.TryGetValue(typeReaderType, out reader))
                    return reader;
            }

            //We dont have a cached type reader, create one
            reader = ReflectionUtils.CreateObject<TypeReader>(typeReaderType.GetTypeInfo(), service, EmptyServiceProvider.Instance);
            service.AddTypeReader(paramType, reader);

            return reader;
        }

        private static bool IsValidModuleDefinition(TypeInfo typeInfo)
        {
            return _moduleTypeInfo.IsAssignableFrom(typeInfo) &&
                   !typeInfo.IsAbstract;
        }

        private static bool IsValidCommandDefinition(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(CommandAttribute)) &&
                   (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(void)) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }
    }
}
