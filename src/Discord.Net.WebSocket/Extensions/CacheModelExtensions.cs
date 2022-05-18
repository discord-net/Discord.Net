using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Discord.WebSocket
{
    internal static class CacheModelExtensions
    {
        public static TDest ToSpecifiedModel<TId, TDest>(this IEntityModel<TId> source, TDest dest)
            where TId : IEquatable<TId>
            where TDest : IEntityModel<TId>
        {
            if (source == null || dest == null)
                throw new ArgumentNullException(source == null ? nameof(source) : nameof(dest));

            // get the shared model interface
            var sourceType = source.GetType();
            var destType = dest.GetType();

            if (sourceType == destType)
                return (TDest)source;

            List<Type> sharedInterfaceModels = new();

            foreach (var intf in sourceType.GetInterfaces())
            {
                if (destType.GetInterface(intf.Name) != null && intf.Name.Contains("Model"))
                    sharedInterfaceModels.Add(intf);
            }

            if (sharedInterfaceModels.Count == 0)
                throw new NotSupportedException($"cannot find common shared model interface between {sourceType.Name} and {destType.Name}");

            foreach (var interfaceType in sharedInterfaceModels)
            {
                var intfName = interfaceType.GenericTypeArguments.Length == 0 ? interfaceType.FullName :
                    $"{interfaceType.Namespace}.{Regex.Replace(interfaceType.Name, @"`\d+?$", "")}<{string.Join(", ", interfaceType.GenericTypeArguments.Select(x => x.FullName))}>";

                foreach (var prop in interfaceType.GetProperties())
                {
                    var sProp = sourceType.GetProperty($"{intfName}.{prop.Name}", BindingFlags.NonPublic | BindingFlags.Instance) ?? sourceType.GetProperty(prop.Name);
                    var dProp = destType.GetProperty($"{intfName}.{prop.Name}", BindingFlags.NonPublic | BindingFlags.Instance) ?? destType.GetProperty(prop.Name);

                    if (sProp == null || dProp == null)
                        throw new NotSupportedException($"Couldn't find common interface property {prop.Name}");

                    dProp.SetValue(dest, sProp.GetValue(source));
                }
            }

            return dest;
        }
    }
}
