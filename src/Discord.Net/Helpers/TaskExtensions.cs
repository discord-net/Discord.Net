using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    public static class TaskExtensions
	{
		public static async Task Timeout(this Task self, int milliseconds)
		{
			Task timeoutTask = Task.Delay(milliseconds);
			Task finishedTask = await Task.WhenAny(self, timeoutTask);
			if (finishedTask == timeoutTask)
				throw new TimeoutException();
			else
				await self;
		}
		public static async Task<T> Timeout<T>(this Task<T> self, int milliseconds)
		{
			Task timeoutTask = Task.Delay(milliseconds);
			Task finishedTask = await Task.WhenAny(self, timeoutTask).ConfigureAwait(false);
			if (finishedTask == timeoutTask)
				throw new TimeoutException();
			else
				return await self.ConfigureAwait(false);
		}
		public static async Task Timeout(this Task self, int milliseconds, CancellationTokenSource timeoutToken)
		{
			try
			{
				timeoutToken.CancelAfter(milliseconds);
				await self.ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				if (timeoutToken.IsCancellationRequested)
					throw new TimeoutException();
				throw;
			}
		}
		public static async Task<T> Timeout<T>(this Task<T> self, int milliseconds, CancellationTokenSource timeoutToken)
		{
			try
			{
				timeoutToken.CancelAfter(milliseconds);
				return await self.ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				if (timeoutToken.IsCancellationRequested)
					throw new TimeoutException();
				throw;
			}
		}
	}
}
