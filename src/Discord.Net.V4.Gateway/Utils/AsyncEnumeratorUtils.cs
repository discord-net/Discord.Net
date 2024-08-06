using System.Runtime.CompilerServices;

namespace Discord.Gateway;

internal static partial class AsyncEnumeratorUtils
{
    public static IAsyncEnumerable<IEnumerable<T>> JoinAsync<T, U>(
        U[] sources,
        [VariableFuncArgs] Func<U, CancellationToken, IAsyncEnumerable<IEnumerable<T>>> mapper,
        CancellationToken token)
    {
        switch (sources.Length)
        {
            case 0:
                return AsyncEnumerable.Empty<IEnumerable<T>>();
            case 1:
                return mapper(sources[0], token);
            default:
                var asyncEnumerables = new IAsyncEnumerable<IEnumerable<T>>[sources.Length];
                for (int i = 0; i < sources.Length; i++)
                    asyncEnumerables[i] = mapper(sources[i], token);
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

    private static async IAsyncEnumerable<IEnumerable<T>> JoinInternal<T>(
        [EnumeratorCancellation] CancellationToken token = default,
        params IAsyncEnumerable<IEnumerable<T>>[] sources)
    {
        var enumerators = new List<IAsyncEnumerator<IEnumerable<T>>>(sources.Length);

        for (var i = 0; i != sources.Length; i++)
            enumerators.Insert(i, sources[i].GetAsyncEnumerator(token));

        var window = new List<T>();

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

                    window.AddRange(enumerator.Current);
                }

                if (window.Count == 0)
                    yield break;

                yield return window;

                window.Clear();
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
