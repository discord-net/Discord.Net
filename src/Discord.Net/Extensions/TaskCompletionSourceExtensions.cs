using System;
using System.Threading.Tasks;

namespace Discord
{
    internal static class TaskCompletionSourceExtensions
    {
        public static Task SetResultAsync<T>(this TaskCompletionSource<T> source, T result)
            => Task.Run(() => source.SetResult(result));
        public static Task<bool> TrySetResultAsync<T>(this TaskCompletionSource<T> source, T result)
            => Task.Run(() => source.TrySetResult(result));

        public static Task SetExceptionAsync<T>(this TaskCompletionSource<T> source, Exception ex)
            => Task.Run(() => source.SetException(ex));
        public static Task<bool> TrySetExceptionAsync<T>(this TaskCompletionSource<T> source, Exception ex)
            => Task.Run(() => source.TrySetException(ex));

        public static Task SetCanceledAsync<T>(this TaskCompletionSource<T> source)
            => Task.Run(() => source.SetCanceled());
        public static Task<bool> TrySetCanceledAsync<T>(this TaskCompletionSource<T> source)
            => Task.Run(() => source.TrySetCanceled());
    }
}
