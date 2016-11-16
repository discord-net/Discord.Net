using System;
using System.Linq;
using System.Reflection;

namespace Discord.Commands
{
    internal class ReflectionUtils
    {
        internal static T CreateObject<T>(TypeInfo typeInfo, CommandService service, IDependencyMap map = null)
            => CreateBuilder<T>(typeInfo, service)(map);

        internal static Func<IDependencyMap, T> CreateBuilder<T>(TypeInfo typeInfo, CommandService service)
        {
            var constructors = typeInfo.DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
            if (constructors.Length == 0)
                throw new InvalidOperationException($"No constructor found for \"{typeInfo.FullName}\"");
            else if (constructors.Length > 1)
                throw new InvalidOperationException($"Multiple constructors found for \"{typeInfo.FullName}\"");

            var constructor = constructors[0];
            System.Reflection.ParameterInfo[] parameters = constructor.GetParameters();

            return (map) =>
            {
                object[] args = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    object arg;
                    if (map == null || !map.TryGet(parameter.ParameterType, out arg))
                    {
                        if (parameter.ParameterType == typeof(CommandService))
                            arg = service;
                        else if (parameter.ParameterType == typeof(IDependencyMap))
                            arg = map;
                        else
                            throw new InvalidOperationException($"Failed to create \"{typeInfo.FullName}\", dependency \"{parameter.ParameterType.Name}\" was not found.");
                    }
                    args[i] = arg;
                }

                try
                {
                    return (T)constructor.Invoke(args);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to create \"{typeInfo.FullName}\"", ex);
                }
            };
        }
    }
}
