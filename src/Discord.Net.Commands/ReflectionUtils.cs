using System;
using System.Linq;
using System.Reflection;

namespace Discord.Commands
{
    internal class ReflectionUtils
    {
        internal static object CreateObject(TypeInfo typeInfo)
        {
            var constructor = typeInfo.DeclaredConstructors.Where(x => x.GetParameters().Length == 0).FirstOrDefault();
            if (constructor == null)
                throw new InvalidOperationException($"Failed to find a valid constructor for \"{typeInfo.FullName}\"");
            try
            {
                return constructor.Invoke(null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create \"{typeInfo.FullName}\"", ex);
            }
        }
    }
}
