using System.Linq;

namespace System.Collections.Generic;

internal static class GenericCollectionExtensions
{
    public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> kvp, out T1 value1, out T2 value2)
    {
        value1 = kvp.Key;
        value2 = kvp.Value;
    }

    public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> kvp) =>
        kvp.ToDictionary(x => x.Key, x => x.Value);
}
