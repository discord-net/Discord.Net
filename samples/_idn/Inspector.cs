using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Idn
{
    public static class Inspector
    {
        public static string Inspect(object value)
        {
            var builder = new StringBuilder();
            if (value != null)
            {
                var type = value.GetType().GetTypeInfo();
                builder.AppendLine($"[{type.Namespace}.{type.Name}]");
                builder.AppendLine($"{InspectProperty(value)}");

                if (value is IEnumerable)
                {
                    var items = (value as IEnumerable).Cast<object>().ToArray();
                    if (items.Length > 0)
                    {
                        builder.AppendLine();
                        foreach (var item in items)
                            builder.AppendLine($"- {InspectProperty(item)}");
                    }
                }
                else
                {
                    var groups = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(x => x.GetIndexParameters().Length == 0)
                            .GroupBy(x => x.Name)
                            .OrderBy(x => x.Key)
                            .ToArray();
                    if (groups.Length > 0)
                    {
                        builder.AppendLine();
                        int pad = groups.Max(x => x.Key.Length) + 1;
                        foreach (var group in groups)
                            builder.AppendLine($"{group.Key.PadRight(pad, ' ')}{InspectProperty(group.First().GetValue(value))}");
                    }
                }
            }
            else
                builder.AppendLine("null");
            return builder.ToString();
        }

        private static string InspectProperty(object obj)
        {
            if (obj == null)
                return "null";

            var type = obj.GetType();

            var debuggerDisplay = type.GetProperty("DebuggerDisplay", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (debuggerDisplay != null)
                return debuggerDisplay.GetValue(obj).ToString();

            var toString = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.Name == "ToString" && x.DeclaringType != typeof(object))
                .FirstOrDefault();
            if (toString != null)
                return obj.ToString();

            var count = type.GetProperty("Count", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (count != null)
                return $"[{count.GetValue(obj)} Items]";

            return obj.ToString();
        }
    }
}
