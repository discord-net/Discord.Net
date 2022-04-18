namespace System.Collections.Generic;

public static class GenericCollectionExtensions
{
    public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> kvp, out T1 value1, out T2 value2)
    {
        value1 = kvp.Key;
        value2 = kvp.Value;
    }
}
