using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public static class IncrementalProviderExtensions
{
    public static IncrementalValuesProvider<T> WhereNonNull<T>(
        this IncrementalValuesProvider<T?> source
    ) where T : class
    {
        return source.Where(x => x is not null)!;
    }
    
    public static IncrementalValuesProvider<T> WhereNonNull<T>(
        this IncrementalValuesProvider<T?> source
    ) where T : struct
    {
        return source.Where(x => x.HasValue).Select((x, _) => x!.Value);
    }

    public static IncrementalValueProvider<T?> FirstOrDefault<T>(
        this IncrementalValuesProvider<T> source,
        Func<T, bool> predicate)
        where T : class
    {
        return source.Collect().Select((x, _) => x.FirstOrDefault(predicate));
    }
}