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

            foreach (var method in validCommands)
            {
                builder.AddCommand((command) => 
                {
                    BuildCommand(command, typeInfo, method, service);
                });
            }
        }

        private static void BuildCommand(CommandBuilder builder, TypeInfo typeInfo, MethodInfo method, CommandService service)
        {
            var attributes = method.GetCustomAttributes();
            
            foreach (var attribute in attributes)
            {
                // TODO: C#7 type switch
                if (attribute is CommandAttribute)
                {
                    var cmdAttr = attribute as CommandAttribute;
                    builder.AddAliases(cmdAttr.Text);
                    builder.RunMode = cmdAttr.RunMode;
                    builder.Name = builder.Name ?? cmdAttr.Text;
                }
                else if (attribute is NameAttribute)
                    builder.Name = (attribute as NameAttribute).Text;
                else if (attribute is PriorityAttribute)
                    builder.Priority = (attribute as PriorityAttribute).Priority;
                else if (attribute is SummaryAttribute)
                    builder.Summary = (attribute as SummaryAttribute).Text;
                else if (attribute is RemarksAttribute)
                    builder.Remarks = (attribute as RemarksAttribute).Text;
                else if (attribute is AliasAttribute)
                    builder.AddAliases((attribute as AliasAttribute).Aliases);
                else if (attribute is PreconditionAttribute)
                    builder.AddPrecondition(attribute as PreconditionAttribute);
            }

            if (builder.Name == null)
                builder.Name = method.Name;

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

            builder.Callback = async (ctx, args, map) => 
            {
                var instance = createInstance(map);
                instance.SetContext(ctx);
                try
                {
                    instance.BeforeExecute();
                    var task = method.Invoke(instance, args) as Task ?? Task.Delay(0);
                    await task.ConfigureAwait(false);
                }
                finally
                {
                    instance.AfterExecute();
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
                // TODO: C#7 type switch
                if (attribute is SummaryAttribute)
                    builder.Summary = (attribute as SummaryAttribute).Text;
                else if (attribute is OverrideTypeReaderAttribute)
                    builder.TypeReader = GetTypeReader(service, paramType, (attribute as OverrideTypeReaderAttribute).TypeReader);
                else if (attribute is ParameterPreconditionAttribute)
                    builder.AddPrecondition(attribute as ParameterPreconditionAttribute);
                else if (attribute is ParamArrayAttribute)
                {
                    builder.IsMultiple = true;
                    paramType = paramType.GetElementType();
                }
                else if (attribute is RemainderAttribute)
                {
                    if (position != count-1)
                        throw new InvalidOperationException("Remainder parameters must be the last parameter in a command.");
                    
                    builder.IsRemainder = true;
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
            reader = ReflectionUtils.CreateObject<TypeReader>(typeReaderType.GetTypeInfo(), service, DependencyMap.Empty);
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