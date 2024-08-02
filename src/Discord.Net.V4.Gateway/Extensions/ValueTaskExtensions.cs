namespace Discord.Gateway;

internal static class ValueTaskExtensions
{
    public static async ValueTask<T?> CastUpAsync<T, U>(this ValueTask<U?> valueTask,  Template<T> template)
        where U : T
        => await valueTask;

    public static async ValueTask<T?> CastDownAsync<T, U>(this ValueTask<U?> valueTask, Template<T> template)
        where T : U
    {
        var result = await valueTask;

        if (result is not T expected)
            throw new InvalidCastException($"Expected {typeof(T)}, got {typeof(U)}");
        return expected;
    }
}
