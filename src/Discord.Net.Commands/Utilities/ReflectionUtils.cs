using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Commands
{
    internal static class ReflectionUtils
    {
        private static readonly TypeInfo objectTypeInfo = typeof(object).GetTypeInfo();

        internal static T CreateObject<T>(TypeInfo typeInfo, CommandService service, IDependencyMap map = null)
            => CreateBuilder<T>(typeInfo, service)(map);

        private static System.Reflection.PropertyInfo[] GetProperties(TypeInfo typeInfo)
        {
            var result = new List<System.Reflection.PropertyInfo>();
            while (typeInfo != objectTypeInfo)
            {
                foreach (var prop in typeInfo.DeclaredProperties)
                {
                    if (prop.SetMethod?.IsPublic == true && prop.GetCustomAttribute<DontInjectAttribute>() == null)
                        result.Add(prop);
                }
                typeInfo = typeInfo.BaseType.GetTypeInfo();
            }
            return result.ToArray();
        }

        internal static Func<IDependencyMap, T> CreateBuilder<T>(TypeInfo typeInfo, CommandService service)
        {
            var constructors = typeInfo.DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
            if (constructors.Length == 0)
                throw new InvalidOperationException($"No constructor found for \"{typeInfo.FullName}\"");
            else if (constructors.Length > 1)
                throw new InvalidOperationException($"Multiple constructors found for \"{typeInfo.FullName}\"");

            var constructor = constructors[0];
            System.Reflection.ParameterInfo[] parameters = constructor.GetParameters();
            System.Reflection.PropertyInfo[] properties = GetProperties(typeInfo)
                  .Where(p => p.SetMethod?.IsPublic == true && p.GetCustomAttribute<DontInjectAttribute>() == null)
                  .ToArray();

            return (map) =>
            {
                object[] args = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    args[i] = GetMember(parameter.ParameterType, map, service, typeInfo);
                }

                T obj;
                try
                {
                    obj = (T)constructor.Invoke(args);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to create \"{typeInfo.FullName}\"", ex);
                }

                foreach(var property in properties)
                {
                    property.SetValue(obj, GetMember(property.PropertyType, map, service, typeInfo));
                }
                return obj;
            };
        }

        internal static object GetMember(Type targetType, IDependencyMap map, CommandService service, TypeInfo baseType)
        {
            object arg;
            if (targetType == typeof(IDependencyMap) || targetType == map.GetType())
                arg = map;
            else if (map == null || !map.TryGet(targetType, out arg))
            {
                if (targetType == typeof(CommandService))
                    arg = service;
                else
                    throw new InvalidOperationException($"Failed to create \"{baseType.FullName}\", dependency \"{targetType.Name}\" was not found.");
            }
            return arg;
        }
    }
}
