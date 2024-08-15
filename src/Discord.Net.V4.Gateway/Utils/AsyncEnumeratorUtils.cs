using System.Runtime.CompilerServices;

namespace Discord.Gateway;

internal static partial class AsyncEnumeratorUtils
{
    public static IAsyncEnumerable<T> JoinAsync<T, U>(
        IEnumerable<U> sources,
        [VariableFuncArgs] Func<U, CancellationToken, IAsyncEnumerable<T>> mapper,
        CancellationToken token)
    {
        var sourcesArray = sources as U[] ?? sources.ToArray();
        
        switch (sourcesArray.Length)
        {
            case 0:
                return AsyncEnumerable.Empty<T>();
            case 1:
                return mapper(sourcesArray[0], token);
            default:
                var asyncEnumerables = new IAsyncEnumerable<T>[sourcesArray.Length];
                for (int i = 0; i < sourcesArray.Length; i++)
                    asyncEnumerables[i] = mapper(sourcesArray[i], token);
                return JoinInternal(token, asyncEnumerables);
        }
    }

    public static IAsyncEnumerable<IEnumerable<T>> JoinAsync<T>(
        CancellationToken token,
        params IAsyncEnumerable<IEnumerable<T>>[] sources)
    {
        return sources.Length switch
        {
            0 => AsyncEnumerable.Empty<IEnumerable<T>>(),
            1 => sources[0],
            _ => JoinInternal(token, sources)
        };
    }

    private static async IAsyncEnumerable<T> JoinInternal<T>(
        [EnumeratorCancellation] CancellationToken token = default,
        params IAsyncEnumerable<T>[] sources)
    {
        var enumerators = new List<IAsyncEnumerator<T>>(sources.Length);

        for (var i = 0; i != sources.Length; i++)
            enumerators.Insert(i, sources[i].GetAsyncEnumerator(token));

        try
        {
            while (enumerators.Count > 0)
            {
                for (var i = 0; i < enumerators.Count; i++)
                {
                    startLoop:
                    var enumerator = enumerators[i];

                    if (!await enumerator.MoveNextAsync(token))
                    {
                        enumerators.RemoveAt(i);
                        await enumerator.DisposeAsync();

                        if (i >= enumerators.Count)
                            break;

                        goto startLoop;
                    }

                    yield return enumerator.Current;
                }
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
                await enumerator.DisposeAsync();

            enumerators.Clear();
        }
    }
}
