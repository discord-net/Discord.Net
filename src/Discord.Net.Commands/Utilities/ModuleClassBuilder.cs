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
        private static readonly TypeInfo _moduleTypeInfo = typeof(ModuleBase).GetTypeInfo();

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
            if (!validTypes.Any())
                throw new InvalidOperationException("Could not find any valid modules from the given selection");
            
            var topLevelGroups = validTypes.Where(x => x.DeclaringType == null);
            var subGroups = validTypes.Intersect(topLevelGroups);

            var builtTypes = new List<TypeInfo>();

            var result = new Dictionary<Type, ModuleInfo>();

            foreach (var typeInfo in topLevelGroups)
            {
                // this shouldn't be the case; may be safe to remove?
                if (result.ContainsKey(typeInfo.AsType()))
                    continue;

                var module = new ModuleBuilder();

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
                
                builder.AddSubmodule((module) => {
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
                // TODO: C#7 type switch
                if (attribute is NameAttribute)
                    builder.Name = (attribute as NameAttribute).Text;
                else if (attribute is SummaryAttribute)
                    builder.Summary = (attribute as SummaryAttribute).Text;
                else if (attribute is RemarksAttribute)
                    builder.Remarks = (attribute as RemarksAttribute).Text;
                else if (attribute is AliasAttribute)
                    builder.AddAliases((attribute as AliasAttribute).Aliases);
                else if (attribute is GroupAttribute)
                {
                    var groupAttr = attribute as GroupAttribute;
                    builder.Name = builder.Name ?? groupAttr.Prefix;
                    builder.AddAliases(groupAttr.Prefix);
                }
                else if (attribute is PreconditionAttribute)
                    builder.AddPrecondition(attribute as PreconditionAttribute);
            }

            var validCommands = typeInfo.DeclaredMethods.Where(x => IsValidCommandDefinition(x));

            foreach (var method in validCommands)
            {
                builder.AddCommand((command) => {
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

            var parameters = method.GetParameters();
            int pos = 0, count = parameters.Length;
            foreach (var paramInfo in parameters)
            {
                builder.AddParameter((parameter) => {
                    BuildParameter(parameter, paramInfo, pos++, count, service);
                });
            }

            var createInstance = ReflectionUtils.CreateBuilder<ModuleBase>(typeInfo, service);

            builder.Callback = (ctx, args, map) => {
                var instance = createInstance(map);
                instance.Context = ctx;
                try
                {
                    return method.Invoke(instance, args) as Task ?? Task.CompletedTask;
                }
                finally{
                    (instance as IDisposable)?.Dispose();
                }
            };
        }

        private static void BuildParameter(ParameterBuilder builder, System.Reflection.ParameterInfo paramInfo, int position, int count, CommandService service)
        {
            var attributes = paramInfo.GetCustomAttributes();
            var paramType = paramInfo.ParameterType;

            builder.Name = paramInfo.Name;

            builder.Optional = paramInfo.IsOptional;
            builder.DefaultValue = paramInfo.HasDefaultValue ? paramInfo.DefaultValue : null;

            foreach (var attribute in attributes)
            {
                // TODO: C#7 type switch
                if (attribute is SummaryAttribute)
                    builder.Summary = (attribute as SummaryAttribute).Text;
                else if (attribute is ParamArrayAttribute)
                {
                    builder.Multiple = true;
                    paramType = paramType.GetElementType();
                }
                else if (attribute is RemainderAttribute)
                {
                    if (position != count-1)
                        throw new InvalidOperationException("Remainder parameters must be the last parameter in a command.");
                    
                    builder.Remainder = true;
                }
            }

            var reader = service.GetTypeReader(paramType);
            if (reader == null)
            {
                var paramTypeInfo = paramType.GetTypeInfo();
                if (paramTypeInfo.IsEnum)
                {
                    reader = EnumTypeReader.GetReader(paramType);
                    service.AddTypeReader(paramType, reader);
                }
                else
                {
                    throw new InvalidOperationException($"{paramType.FullName} is not supported as a command parameter, are you missing a TypeReader?");
                }
            }

            builder.ParameterType = paramType;
            builder.TypeReader = reader;
        }

        private static bool IsValidModuleDefinition(TypeInfo typeInfo)
        {
            return _moduleTypeInfo.IsAssignableFrom(typeInfo) &&
                   !typeInfo.IsAbstract;
        }

        private static bool IsValidCommandDefinition(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(CommandAttribute)) &&
                   methodInfo.ReturnType == typeof(Task) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }
    }
}