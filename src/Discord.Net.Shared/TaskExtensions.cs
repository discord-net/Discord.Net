using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    internal static class TaskExtensions
    {
        public static async Task Timeout(this Task task, int milliseconds)
        {
            Task timeoutTask = Task.Delay(milliseconds);
            Task finishedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
            if (finishedTask == timeoutTask)
                throw new TimeoutException();
            else
                await task.ConfigureAwait(false);
        }
        public static async Task<T> Timeout<T>(this Task<T> task, int milliseconds)
        {
            Task timeoutTask = Task.Delay(milliseconds);
            Task finishedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
            if (finishedTask == timeoutTask)
                throw new TimeoutException();
            else
                return await task.ConfigureAwait(false);
        }
        public static async Task Timeout(this Task task, int milliseconds, CancellationTokenSource timeoutToken)
        {
            try
            {
                timeoutToken.CancelAfter(milliseconds);
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (timeoutToken.IsCancellationRequested)
                    throw new TimeoutException();
                throw;
            }
        }
        public static async Task<T> Timeout<T>(this Task<T> task, int milliseconds, CancellationTokenSource timeoutToken)
        {
            try
            {
                timeoutToken.CancelAfter(milliseconds);
                return await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (timeoutToken.IsCancellationRequested)
                    throw new TimeoutException();
                throw;
            }
        }

        public static async Task Wait(this CancellationTokenSource tokenSource)
        {
            var token = tokenSource.Token;
            try { await Task.Delay(-1, token).ConfigureAwait(false); }
            catch (OperationCanceledException) { } //Expected
        }
        public static async Task Wait(this CancellationToken token)
        {
            try { await Task.Delay(-1, token).ConfigureAwait(false); }
            catch (OperationCanceledException) { } //Expected
        }
    }
}
