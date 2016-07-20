using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Commands
{
    internal class ReflectionUtils
    {
        internal static object CreateObject(TypeInfo typeInfo, CommandService service, IDependencyMap map = null)
        {
            if (typeInfo.DeclaredConstructors.Count() > 1)
                throw new InvalidOperationException($"Found too many constructors for \"{typeInfo.FullName}\"");

            var constructor = typeInfo.DeclaredConstructors.FirstOrDefault();

            if (constructor == null)
                throw new InvalidOperationException($"Found no constructor for \"{typeInfo.FullName}\"");

            object[] parameters;
            try
            {
                // TODO: probably change this ternary into something sensible
                parameters = constructor.GetParameters()
                    .Select(x => x.ParameterType == typeof(CommandService) ? service : map.Get(x.ParameterType)).ToArray();
            }
            catch (KeyNotFoundException ex) // tried to inject an invalid dependency
            {
                throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (could not provide parameter)", ex);
            }
            catch (NullReferenceException ex) // tried to find a dependency
            {
                throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (type requires dependency injection)", ex);
            }

            try
            {
                return constructor.Invoke(parameters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (Error invoking constructor)", ex);
            }
        }
    }
}
