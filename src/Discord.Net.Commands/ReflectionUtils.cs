using System;
using System.Linq;
using System.Reflection;

namespace Discord.Commands
{
    internal class ReflectionUtils
    {
        internal static object CreateObject(TypeInfo typeInfo, CommandService commands, IDependencyMap map = null)
        {
            if (typeInfo.DeclaredConstructors.Count() > 1)
                throw new InvalidOperationException($"Found too many constructors for \"{typeInfo.FullName}\"");
            var constructor = typeInfo.DeclaredConstructors.FirstOrDefault();
            try
            {
                if (constructor.GetParameters().Length == 0)
                    return constructor.Invoke(null);
                else if (constructor.GetParameters().Length > 1)
                    throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (Found too many parameters)");
                var parameter = constructor.GetParameters().FirstOrDefault();
                if (parameter == null)
                    throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (No valid parameters)");
                if (parameter.ParameterType == typeof(CommandService))
                    return constructor.Invoke(new object[1] { commands });
                else if (parameter.ParameterType == typeof(IDependencyMap))
                {
                    if (map == null) throw new InvalidOperationException($"The constructor for \"{typeInfo.FullName}\" requires a Dependency Map.");
                    return constructor.Invoke(new object[1] { map });
                }
                else
                    throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (Invalid Parameter Type: \"{parameter.ParameterType.FullName}\")");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (Error invoking constructor)");
            }
        }
    }
}
