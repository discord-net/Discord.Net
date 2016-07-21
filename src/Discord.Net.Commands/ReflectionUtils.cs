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

            object[] arguments = null;

            ParameterInfo[] parameters = constructor.GetParameters();

            // TODO: can this logic be made better/cleaner?
            if (parameters.Length == 1)
            {
                if (parameters[0].ParameterType == typeof(IDependencyMap))
                {
                    if (map != null)
                        arguments = new object[] { map };
                    else
                        throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (an IDependencyMap is required)");
                }
            }
            else if (parameters.Length == 2)
            {
                if (parameters[0].ParameterType == typeof(CommandService) && parameters[1].ParameterType == typeof(IDependencyMap))
                    if (map != null)
                        arguments = new object[] { service, map };
                    else
                        throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (an IDependencyMap is required)");
            }

            if (arguments == null)
            {
                try
                {
                    // TODO: probably change this ternary into something sensible?
                    arguments = parameters.Select(x => x.ParameterType == typeof(CommandService) ? service : map.Get(x.ParameterType)).ToArray();
                }
                catch (KeyNotFoundException ex) // tried to inject an invalid dependency
                {
                    throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (could not provide parameter)", ex);
                }
                catch (NullReferenceException ex) // tried to find a dependency
                {
                    throw new InvalidOperationException($"Could not find a valid constructor for \"{typeInfo.FullName}\" (an IDependencyMap is required)", ex);
                }
            }

            try
            {
                return constructor.Invoke(arguments);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not create \"{typeInfo.FullName}\"", ex);
            }
        }
    }
}
